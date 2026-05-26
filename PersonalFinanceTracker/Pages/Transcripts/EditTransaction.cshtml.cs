using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	// staged: add transcripts CRUD files
	public class EditTransactionModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public EditTransactionModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public TranscriptTransaction TranscriptTransaction { get; set; } = default!;

		public Transcript Transcript { get; set; } = default!;

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