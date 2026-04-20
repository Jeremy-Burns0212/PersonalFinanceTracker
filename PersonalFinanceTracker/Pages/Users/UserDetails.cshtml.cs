using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	/// <summary>
	/// Page model for viewing details of a specific application user.
	/// Uses <see cref="UserManager{TUser}"/> to retrieve the user from the Identity store.
	/// </summary>
	public class DetailsUserModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of <see cref="DetailsUserModel"/>.
		/// </summary>
		/// <param name="userManager">The Identity user manager.</param>
		public DetailsUserModel(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		/// <summary>
		/// The application user being displayed. Hides <see cref="PageModel.User"/> (ClaimsPrincipal).
		/// </summary>
		public new AppUser User { get; set; } = default!;

		/// <summary>
		/// Handles HTTP GET requests to display a user's details.
		/// </summary>
		/// <param name="id">The string identifier of the user to display.</param>
		/// <returns>
		/// A <see cref="PageResult"/> with the user details when found; otherwise a <see cref="NotFoundResult"/>.
		/// </returns>
		public async Task<IActionResult> OnGetAsync(string? id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var existing = await _userManager.FindByIdAsync(id);
			if (existing is null)
			{
				return NotFound();
			}

			User = new AppUser
			{
				Id = existing.Id,
				UserName = existing.UserName,
				Email = existing.Email,
				FullName = existing.FullName
			};

			return Page();
		}
	}
}
