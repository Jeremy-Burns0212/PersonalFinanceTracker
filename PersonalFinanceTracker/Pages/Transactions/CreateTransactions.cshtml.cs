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
	public class CreateTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public CreateTransactionsModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[BindProperty]
		public Transaction Transaction { get; set; } = new() { Date = DateOnly.FromDateTime(DateTime.UtcNow) };

		public SelectList CategoryOptions { get; set; } = default!;

		public async Task OnGetAsync()
		{
			await EnsureDefaultCategoryAsync();
			await LoadCategoryOptionsAsync();
		}

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
