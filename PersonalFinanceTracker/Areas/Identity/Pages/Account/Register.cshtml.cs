using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Page model for user registration and login.
    /// Handles both registration (OnPostAsync) and login (OnPostLoginAsync) functionality.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RegisterModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Input model bound to the Register form.
        /// </summary>
        public class RegisterInputModel
        {
            [Required]
            [StringLength(100)]
            [Display(Name = "Full name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; } = string.Empty;

            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        /// <summary>
        /// Input model for the login form.
        /// </summary>
        public class LoginInputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        [BindProperty]
        public LoginInputModel LoginInput { get; set; } = new LoginInputModel();

        // Preserve returnUrl if provided
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// GET handler.
        /// </summary>
        public void OnGet()
        {
            // No-op: page simply renders the form.
        }

        /// <summary>
        /// Handles POST to register a new user. Creates an AppUser and signs them in on success.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new AppUser
            {
                UserName = Input.UserName,
                Email = Input.Email ?? string.Empty,
                FullName = Input.FullName
            };

            // Create the user with the configured password rules (see Program.cs).
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                // Sign in the user immediately after registration.
                await _signInManager.SignInAsync(user, isPersistent: false);

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                return LocalRedirect("/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        /// <summary>
        /// Handles POST request to authenticate a user. Invoked when form uses handler name "Login".
        /// </summary>
        /// <returns>Redirects on success or redisplays the form on failure.</returns>
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(LoginInput.UserName, LoginInput.Password, LoginInput.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                return LocalRedirect("/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}