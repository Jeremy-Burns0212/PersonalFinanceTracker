using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	/// <summary>
	/// Page model for viewing details of a single transaction.
	/// </summary>
	public class DetailsTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of <see cref="DetailsTransactionsModel"/>.
		/// </summary>
		public DetailsTransactionsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// The transaction shown on the details page.
		/// </summary>
		public Transaction Transaction { get; set; } = default!;

		/// <summary>
		/// GET handler that loads the transaction by id.
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
	}
}
