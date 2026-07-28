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
	/// Page model for removing a single archived transaction from a transcript.
	/// </summary>
	[Authorize]
	public class DeleteTransactionModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of <see cref="DeleteTransactionModel"/>.
		/// </summary>
		/// <param name="context">Application DB context for accessing transcripts and archived transactions.</param>
		/// <param name="userManager">The user manager for accessing the current user.</param>
		public DeleteTransactionModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		/// <summary>
		/// Parent transcript header used for context in the confirmation view.
		/// </summary>
		public Transcript Transcript { get; set; } = default!;

		/// <summary>
		/// Bound archived transaction selected for deletion.
		/// </summary>
		[BindProperty]
		public TranscriptTransaction TranscriptTransaction { get; set; } = default!;

		/// <summary>
		/// GET handler that loads the transcript and archived transaction to confirm deletion.
		/// </summary>
		public async Task<IActionResult> OnGetAsync(int? transcriptId, int? id)
		{
			if (transcriptId is null || id is null)
			{
				return NotFound();
			}

			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == transcriptId);

			if (transcript is null)
			{
				return NotFound();
			}

			var transcriptTransaction = transcript.Transactions.FirstOrDefault(item => item.Id == id);
			if (transcriptTransaction is null)
			{
				return NotFound();
			}

			Transcript = transcript;
			TranscriptTransaction = transcriptTransaction;
			return Page();
		}

		/// <summary>
		/// POST handler that deletes the archived transaction and updates the transcript timestamp.
		/// </summary>
		public async Task<IActionResult> OnPostAsync(int transcriptId, int id)
		{
			var userId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(userId))
			{
				return Challenge();
			}

			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == transcriptId && item.UserId == userId);

			if (transcript is null)
			{
				return NotFound();
			}

			var transcriptTransaction = transcript.Transactions.FirstOrDefault(item => item.Id == id);
			if (transcriptTransaction is not null)
			{
				_context.TranscriptTransactions.Remove(transcriptTransaction);
				transcript.LastAccessed = DateTime.UtcNow;
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Edit", new { id = transcriptId });
		}
	}
}