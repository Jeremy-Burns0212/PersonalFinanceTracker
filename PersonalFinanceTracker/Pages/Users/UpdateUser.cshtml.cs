using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	/// <summary>
	/// Page model for updating an existing application user using ASP.NET Core Identity.
	/// Uses <see cref="UserManager{TUser}"/> to find and update users safely.
	/// </summary>
	public class UpdateUserModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of <see cref="UpdateUserModel"/>.
		/// </summary>
		/// <param name="userManager">The Identity user manager.</param>
		public UpdateUserModel(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		/// <summary>
		/// AppUser instance bound to the form.
		/// </summary>
		[BindProperty]
		public new AppUser User { get; set; } = default!;

		/// <summary>
		/// Handles HTTP GET to load the user to edit.
		/// </summary>
		/// <param name="id">The string identifier of the user to edit.</param>
		/// <returns>A <see cref="PageResult"/> if found; otherwise <see cref="NotFoundResult"/>.</returns>
		public async Task<IActionResult> OnGetAsync(string? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user is null)
			{
				return NotFound();
			}

			// Populate the bound model with values retrieved from Identity.
			User = new AppUser
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				FullName = user.FullName
			};

			return Page();
		}

		/// <summary>
		/// Handles HTTP POST to persist user updates.
		/// </summary>
		/// <returns>
		/// Redirects to index on success, returns the page with validation errors otherwise,
		/// or NotFound if the user no longer exists.
		/// </returns>
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			// Find the existing user in the Identity store.
			var existing = await _userManager.FindByIdAsync(User.Id);
			if (existing is null)
			{
				return NotFound();
			}

			// Update allowed fields. Avoid overwriting security-sensitive fields here.
			existing.FullName = User.FullName;
			existing.Email = User.Email;
			existing.UserName = User.Email; // keep username in sync with email if desired

			var result = await _userManager.UpdateAsync(existing);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					// Add Identity errors to ModelState so the page can show them.
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Page();
			}

			return RedirectToPage("./Index");
		}
	}
}
