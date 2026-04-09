using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	public class UpdateTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public UpdateTransactionsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Transaction Transaction { get; set; } = new();

		public SelectList CategoryOptions { get; set; } = default!;

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
