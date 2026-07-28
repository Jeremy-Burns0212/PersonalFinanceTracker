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
	/// Page model for deleting a transcript archive header.
	/// </summary>
	[Authorize]
	public class DeleteModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of <see cref="DeleteModel"/>.
		/// </summary>
		/// <param name="context">Application DB context for accessing transcripts.</param>
		/// <param name="userManager">The user manager for accessing the current user.</param>
		public DeleteModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		/// <summary>
		/// Bound transcript selected for deletion.
		/// </summary>
		[BindProperty]
		public Transcript Transcript { get; set; } = default!;

		/// <summary>
		/// GET handler that loads the transcript to confirm deletion.
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
				.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

			if (transcript is null)
			{
				return NotFound();
			}

			Transcript = transcript;
			return Page();
		}

		/// <summary>
		/// POST handler that deletes the transcript and redirects to the index.
		/// </summary>
		public async Task<IActionResult> OnPostAsync(int? id)
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

			var transcript = await _context.Transcripts.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);
			if (transcript is not null)
			{
				_context.Transcripts.Remove(transcript);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}