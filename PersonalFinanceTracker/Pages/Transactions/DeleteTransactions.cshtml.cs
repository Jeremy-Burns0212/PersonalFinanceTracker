using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	/// <summary>
	/// Page model for deleting a transaction.
	/// </summary>
	public class DeleteTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of <see cref="DeleteTransactionsModel"/>.
		/// </summary>
		public DeleteTransactionsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// The transaction selected for deletion.
		/// </summary>
		[BindProperty]
		public Transaction Transaction { get; set; } = default!;

		/// <summary>
		/// GET handler that loads the transaction to confirm deletion.
		/// </summary>
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

		/// <summary>
		/// POST handler that performs the deletion and redirects to the index.
		/// </summary>
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
