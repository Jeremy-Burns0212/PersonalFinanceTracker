using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	public class DeleteTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public DeleteTransactionsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Transaction Transaction { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions
				.Include(t => t.Category)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (transaction is null)
			{
				return NotFound();
			}

			Transaction = transaction;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions.FindAsync(id);
			if (transaction is not null)
			{
				_context.Transactions.Remove(transaction);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}
