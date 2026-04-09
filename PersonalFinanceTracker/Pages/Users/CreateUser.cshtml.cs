using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	public class CreateUserModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public CreateUserModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public AppUser User { get; set; } = new();

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			_context.Users.Add(User);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
