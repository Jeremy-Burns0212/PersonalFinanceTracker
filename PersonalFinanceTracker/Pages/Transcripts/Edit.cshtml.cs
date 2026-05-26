using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	public class EditModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public EditModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Transcript Transcript { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id is null)
			{
				return NotFound();
			}

			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == id);

			if (transcript is null)
			{
				return NotFound();
			}

			Transcript = transcript;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				await LoadTranscriptTransactionsAsync();
				return Page();
			}

			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == Transcript.Id);

			if (transcript is null)
			{
				return NotFound();
			}

			transcript.Name = Transcript.Name.Trim();
			transcript.Description = Transcript.Description?.Trim();
			transcript.LastAccessed = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return RedirectToPage("./Edit", new { id = transcript.Id });
		}

		private async Task LoadTranscriptTransactionsAsync()
		{
			var transcript = await _context.Transcripts
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == Transcript.Id);

			if (transcript is not null)
			{
				Transcript.Transactions = transcript.Transactions;
			}
		}
	}
}