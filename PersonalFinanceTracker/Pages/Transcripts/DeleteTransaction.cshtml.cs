using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	// staged: add transcripts CRUD files
	public class DeleteTransactionModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public DeleteTransactionModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public Transcript Transcript { get; set; } = default!;

		[BindProperty]
		public TranscriptTransaction TranscriptTransaction { get; set; } = default!;

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

		public async Task<IActionResult> OnPostAsync(int transcriptId, int id)
		{
			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == transcriptId);

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