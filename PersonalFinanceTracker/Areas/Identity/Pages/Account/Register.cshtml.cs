using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Areas.Identity.Pages.Account
{
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

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = null!;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = null!;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = null!;

            [Required]
            [Display(Name = "Full Name")]
            [StringLength(100)]
            public string FullName { get; set; } = null!;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

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
