using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	/// <summary>
	/// Page model for updating an existing transaction.
	/// </summary>
	public class UpdateTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of <see cref="UpdateTransactionsModel"/>.
		/// </summary>
		public UpdateTransactionsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// The transaction being edited. Bound on POST.
		/// </summary>
		[BindProperty]
		public Transaction Transaction { get; set; } = default!;

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

			var transaction = await _context.Transactions.FindAsync(id);
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

			_context.Attach(Transaction).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await TransactionExistsAsync(Transaction.Id))
				{
					return NotFound();
				}

				throw;
			}

			return RedirectToPage("./Index");
		}

		private async Task<bool> TransactionExistsAsync(int id)
			=> await _context.Transactions.AnyAsync(e => e.Id == id);

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
