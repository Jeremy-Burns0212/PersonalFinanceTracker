using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	public class DetailsUserModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public DetailsUserModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public AppUser User { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
			if (user is null)
			{
				return NotFound();
			}

			User = user;
			return Page();
		}
	}
}
