using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Page model for registering a new user account.
    /// Handles creating the user, assigning a default role, and signing the user in.
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<RegisterModel> logger) : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ILogger<RegisterModel> _logger = logger;

        /// <summary>
        /// Bound input model for the registration form.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = null!;

        /// <summary>
        /// Return URL after registration completes.
        /// </summary>
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Input model for the registration form.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// User's email address (also used as the username).
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = null!;

            /// <summary>
            /// Password for the new account.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = null!;

            /// <summary>
            /// Confirmation of the password entry.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = null!;

            /// <summary>
            /// Full name for display purposes.
            /// </summary>
            [Required]
            [Display(Name = "Full Name")]
            [StringLength(100)]
            public string FullName { get; set; } = null!;
        }

        /// <summary>
        /// GET handler to initialize the registration page.
        /// </summary>
        /// <remarks>
        /// Preserves the `returnUrl` so the UI can redirect after successful registration.
        /// This is commonly used to return users to the page they were on before registering.
        /// </remarks>
        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// POST handler that attempts to create the user, assign a default role, and sign the user in.
        /// Any Identity errors are added to the page's <see cref="Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates an <see cref="PersonalFinanceTracker.Models.AppUser"/> using the application's
        /// <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/> and persists it to the Identity store.
        /// Note: `EmailConfirmed` is set to true for developer convenience; consider requiring email confirmation in production.
        /// After creation the user is added to a default role ("User"). If the role does not exist it will be created.
        /// </remarks>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Log ModelState to diagnose binding issues
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors: {errors}", 
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create a new AppUser instance from the posted form values.
                    AppUser user = new() 
                    { 
                        UserName = Input.Email, 
                        Email = Input.Email, 
                        EmailConfirmed = true,
                        FullName = Input.FullName
                    };
                    IdentityResult result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
#pragma warning disable CA2254 // Template should be a static expression
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                        _logger.LogInformation("User created a new account with password.");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
#pragma warning restore CA2254

                        // Assign a default Identity role if desired (e.g., "User").
                        const string defaultRole = "User";
                        if (!await _roleManager.RoleExistsAsync(defaultRole))
                        {
                            IdentityResult roleCreateResult = await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                            if (!roleCreateResult.Succeeded)
                            {
                                _logger.LogError("Failed to create default role. Errors: {errors}", 
                                    string.Join("; ", roleCreateResult.Errors.Select(e => e.Description)));
                                foreach (IdentityError error in roleCreateResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                                return Page();
                            }
                        }
                        // Add the new user to the default role.
                        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, defaultRole);
                        if (!roleResult.Succeeded)
                        {
                            _logger.LogError("Failed to add user to role. Errors: {errors}", 
                                string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                            foreach (IdentityError error in roleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return Page();
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User signed in successfully. Redirecting to {returnUrl}", returnUrl);
                        return LocalRedirect(returnUrl);
                    }
                    // Add UserManager.CreateAsync errors to ModelState
                    _logger.LogError("Failed to create user. Errors: {errors}", 
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during registration");
                    ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
