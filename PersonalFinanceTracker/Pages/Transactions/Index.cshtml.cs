using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public IList<Transaction> Transactions { get; set; } = new List<Transaction>();

		public async Task OnGetAsync()
		{
			Transactions = await _context.Transactions
				.Include(t => t.Category)
				.OrderByDescending(t => t.Date)
				.ToListAsync();
		}
	}
}
