using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	// staged: add transcripts CRUD files
	/// <summary>
	/// Page model for editing a single transcript transaction (an archived transaction row).
	/// </summary>
	public class EditTransactionModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of <see cref="EditTransactionModel"/>.
		/// </summary>
		/// <param name="context">Application DB context for accessing transcript transactions.</param>
		public EditTransactionModel(ApplicationDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Bound transcript transaction being edited.
		/// </summary>
		[BindProperty]
		public TranscriptTransaction TranscriptTransaction { get; set; } = default!;

		/// <summary>
		/// Parent transcript header for context when editing.
		/// </summary>
		public Transcript Transcript { get; set; } = default!;

		/// <summary>
		/// GET handler that loads the transcript and the selected archived transaction.
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
		/// POST handler that applies edits to the archived transaction and updates the transcript's last accessed time.
		/// </summary>
		public async Task<IActionResult> OnPostAsync(int transcriptId)
		{
			if (!ModelState.IsValid)
			{
				await LoadTranscriptAsync(transcriptId);
				return Page();
			}

			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == transcriptId);

			if (transcript is null)
			{
				return NotFound();
			}

			var existingTransaction = transcript.Transactions.FirstOrDefault(item => item.Id == TranscriptTransaction.Id);
			if (existingTransaction is null)
			{
				return NotFound();
			}

			existingTransaction.Date = TranscriptTransaction.Date;
			existingTransaction.Description = TranscriptTransaction.Description.Trim();
			existingTransaction.Type = TranscriptTransaction.Type;
			existingTransaction.CategoryName = TranscriptTransaction.CategoryName?.Trim() ?? string.Empty;
			existingTransaction.Amount = TranscriptTransaction.Amount;
			transcript.LastAccessed = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return RedirectToPage("./Edit", new { id = transcriptId });
		}

		private async Task LoadTranscriptAsync(int transcriptId)
		{
			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == transcriptId);

			if (transcript is not null)
			{
				Transcript = transcript;
			}
		}
	}
}