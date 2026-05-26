using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Page model to allow a signed-in user to delete their account.
    /// Removes the Identity user and signs the user out.
    /// </summary>
    [Authorize]
    public class DeleteAccountModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of <see cref="DeleteAccountModel"/>.
        /// </summary>
        /// <param name="userManager">User manager for Identity operations.</param>
        /// <param name="signInManager">Sign-in manager used to sign the user out after deletion.</param>
        /// <param name="dbContext">Application DB context for optional cleanup of related data.</param>
        public DeleteAccountModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        /// <summary>
        /// POST handler that deletes the current user's account and signs them out.
        /// </summary>
        /// <remarks>
        /// Before deleting the Identity user, consider removing or archiving related application data
        /// (for example, transactions or profile data). Perform that cleanup here if required.
        /// Deleting the Identity user will remove the account from the authentication store.
        /// </remarks>
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            // Optional: remove related application data here (e.g., transactions or profiles owned by the user).
            // If you rely on cascade deletes or manual cleanup, perform that work before deleting the Identity user.

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }
    }
}
