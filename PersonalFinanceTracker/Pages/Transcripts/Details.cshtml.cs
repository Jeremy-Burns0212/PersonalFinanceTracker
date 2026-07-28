using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	// staged: add transcripts CRUD files
	/// <summary>
	/// Page model for viewing a transcript and its archived transactions.
	/// </summary>
	[Authorize]
	public class DetailsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of <see cref="DetailsModel"/>.
		/// </summary>
		/// <param name="context">Application DB context for loading transcripts and transactions.</param>
		/// <param name="userManager">The user manager for accessing the current user.</param>
		public DetailsModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		/// <summary>
		/// The transcript displayed on the details page.
		/// </summary>
		public Transcript Transcript { get; private set; } = default!;

		/// <summary>
		/// GET handler that loads the transcript header and its transactions for display.
		/// </summary>
		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var userId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(userId))
			{
				return Challenge();
			}

			var transcript = await _context.Transcripts
				.AsNoTracking()
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

			if (transcript is null)
			{
				return NotFound();
			}

			Transcript = transcript;
			return Page();
		}
	}
}