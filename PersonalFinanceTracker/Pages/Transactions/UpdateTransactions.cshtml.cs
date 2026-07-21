using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	[Authorize]
	/// <summary>
	/// Page model for updating an existing transaction.
	/// </summary>
	public class UpdateTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of <see cref="UpdateTransactionsModel"/>.
		/// </summary>
		public UpdateTransactionsModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		/// <summary>
		/// The transaction being edited. Bound on POST.
		/// </summary>
		[BindProperty]
		public Transaction Transaction { get; set; } = new() { Date = DateOnly.FromDateTime(DateTime.UtcNow) };

		/// <summary>
		/// Options used to populate the category dropdown.
		/// </summary>
		public SelectList CategoryOptions { get; set; } = default!;

		/// <summary>
		/// GET handler. Loads the transaction for the provided id and category options.
		/// </summary>
		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			await EnsureDefaultCategoryAsync();

			var userId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(userId))
			{
				return Challenge();
			}

			var transaction = await _context.Transactions
				.Include(t => t.Category)
				.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
			if (transaction is null)
			{
				return NotFound();
			}

			Transaction = transaction;
			await LoadCategoryOptionsAsync();
			return Page();
		}

		/// <summary>
		/// POST handler that applies changes and saves the transaction.
		/// </summary>
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				await LoadCategoryOptionsAsync();
				return Page();
			}

			var userId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(userId))
			{
				return Challenge();
			}

			var transactionToUpdate = await _context.Transactions
				.FirstOrDefaultAsync(t => t.Id == Transaction.Id && t.UserId == userId);
			if (transactionToUpdate is null)
			{
				return NotFound();
			}

			transactionToUpdate.Amount = Transaction.Amount;
			transactionToUpdate.Date = Transaction.Date;
			transactionToUpdate.Description = Transaction.Description;
			transactionToUpdate.Type = Transaction.Type;
			transactionToUpdate.CategoryId = Transaction.CategoryId;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await TransactionExistsAsync(Transaction.Id, userId))
				{
					return NotFound();
				}

				throw;
			}

			return RedirectToPage("./Index");
		}

		private async Task<bool> TransactionExistsAsync(int id, string userId)
			=> await _context.Transactions.AnyAsync(e => e.Id == id && e.UserId == userId);

		private async Task EnsureDefaultCategoryAsync()
		{
			if (!await _context.Categories.AnyAsync())
			{
				_context.Categories.Add(new Category { Name = "General" });
				await _context.SaveChangesAsync();
			}
		}

		private async Task LoadCategoryOptionsAsync()
		{
			var categories = await _context.Categories
				.OrderBy(c => c.Name)
				.ToListAsync();

			CategoryOptions = new SelectList(categories, nameof(Category.Id), nameof(Category.Name));
		}
	}
}
