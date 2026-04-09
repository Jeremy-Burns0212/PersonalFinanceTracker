using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public IList<AppUser> Users { get; set; } = new List<AppUser>();

		public async Task OnGetAsync()
		{
			Users = await _context.Users
				.OrderBy(u => u.FullName)
				.ToListAsync();
		}
	}
}
