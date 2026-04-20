using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	/// <summary>
	/// Page model for deleting an application user via ASP.NET Core Identity.
	/// Uses <see cref="UserManager{TUser}"/> to remove users.
	/// </summary>
	public class DeleteUserModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of <see cref="DeleteUserModel"/>.
		/// </summary>
		/// <param name="userManager">The Identity user manager.</param>
		public DeleteUserModel(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		/// <summary>
		/// The user being deleted, bound to the page.
		/// </summary>
		[BindProperty]
		public new AppUser User { get; set; } = default!;

		/// <summary>
		/// Loads the user for confirmation before deleting.
		/// </summary>
		/// <param name="id">The string identifier of the user to delete.</param>
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
		/// Deletes the user from the Identity store.
		/// </summary>
		/// <param name="id">The string identifier of the user to delete.</param>
		/// <returns>Redirects to index after deletion or NotFound if id is invalid.</returns>
		public async Task<IActionResult> OnPostAsync(string? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user is not null)
			{
				await _userManager.DeleteAsync(user);
			}

			return RedirectToPage("./Index");
		}
	}
}
