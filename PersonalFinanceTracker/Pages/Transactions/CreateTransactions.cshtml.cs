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
	/// Page model for creating a new transaction.
	/// </summary>
	public class CreateTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;
    
		/// <summary>
		/// Initializes a new instance of <see cref="CreateTransactionsModel"/>.
		/// </summary>
		/// <param name="context">The application database context.</param>
		public CreateTransactionsModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		public CreateTransactionsModel(ApplicationDbContext context)
		{
			_context = context;
			_userManager = userManager;
		}
    
		[BindProperty]
		/// <summary>
		/// The transaction being created. Bound on POST.
		/// </summary>
		public Transaction Transaction { get; set; } = new() { Date = DateOnly.FromDateTime(DateTime.UtcNow) };

		/// <summary>
		/// Options used to populate the category dropdown.
		/// </summary>
		public SelectList CategoryOptions { get; set; } = default!;

		/// <summary>
		/// GET handler. Ensures at least one category exists and loads the category options.
		/// </summary>
		public async Task OnGetAsync()
		{
			await EnsureDefaultCategoryAsync();
			await LoadCategoryOptionsAsync();
		}

		/// <summary>
		/// POST handler that validates and persists the new transaction.
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

			Transaction.UserId = userId;
			_context.Transactions.Add(Transaction);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}

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
