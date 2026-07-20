using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	[Authorize]
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public IList<Transaction> Transactions { get; set; } = new List<Transaction>();

		public async Task OnGetAsync()
		{
			var userId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(userId))
			{
				Transactions = new List<Transaction>();
				return;
			}

			IOrderedQueryable<Transaction> transactions = _context.Transactions
				.Include(t => t.Category)
				.Where(t => t.UserId == userId)
				.OrderByDescending(t => t.Date);
			Transactions = await transactions
				.ToListAsync();
		}
	}
}
