using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	public class DeleteUserModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public DeleteUserModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
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

		public async Task<IActionResult> OnPostAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var user = await _context.Users.FindAsync(id);
			if (user is not null)
			{
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}
