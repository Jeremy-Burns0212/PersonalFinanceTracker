using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	public class UpdateUserModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public UpdateUserModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public AppUser User { get; set; } = new();

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var user = await _context.Users.FindAsync(id);
			if (user is null)
			{
				return NotFound();
			}

			User = user;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			_context.Attach(User).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await UserExistsAsync(User.Id))
				{
					return NotFound();
				}

				throw;
			}

			return RedirectToPage("./Index");
		}

		private async Task<bool> UserExistsAsync(int id)
			=> await _context.Users.AnyAsync(e => e.Id == id);
	}
}
