using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Users
{
	/// <summary>
	/// Page model for creating a new application user using ASP.NET Core Identity.
	/// Uses UserManager to create the user (no hardcoded credentials).
	/// </summary>
	public class CreateUserModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;

		public CreateUserModel(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		[BindProperty]
		public CreateUserInput Input { get; set; } = new();

		public void OnGet()
		{
		}

		public class CreateUserInput
		{
			[Required]
			[StringLength(100)]
			public string FullName { get; set; } = string.Empty;

			[Required]
			[EmailAddress]
			[StringLength(150)]
			public string Email { get; set; } = string.Empty;

			[Required]
			[StringLength(100, MinimumLength = 6)]
			[DataType(DataType.Password)]
			public string Password { get; set; } = string.Empty;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = new AppUser
			{
				UserName = Input.Email,
				Email = Input.Email,
				FullName = Input.FullName
			};

			// Create user with the supplied password. Identity hashes the password for you.
			var result = await _userManager.CreateAsync(user, Input.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Page();
			}

			// Optionally sign in the user or send confirmation email here
			return RedirectToPage("./Index");
		}
	}
}
