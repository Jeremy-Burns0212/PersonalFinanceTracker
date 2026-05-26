using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Page model that signs the current user out.
    /// </summary>
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of <see cref="LogoutModel"/>.
        /// </summary>
        /// <param name="signInManager">Sign-in manager used to sign the user out.</param>
        public LogoutModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// POST handler that signs out the current user and redirects to the site root.
        /// </summary>
        /// <remarks>
        /// Signs the current user out of the application and returns to the homepage.
        /// Use this when you need an explicit server-side sign-out (e.g., logout button POST).
        /// </remarks>
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }
    }
}
