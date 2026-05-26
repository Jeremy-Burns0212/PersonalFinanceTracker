using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	// staged: add transcripts CRUD files
	/// <summary>
	/// Page model for editing a transcript header (name/description) and loading contained transactions.
	/// </summary>
	public class EditModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of <see cref="EditModel"/>.
		/// </summary>
		/// <param name="context">Application DB context for editing transcripts.</param>
		public EditModel(ApplicationDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Bound transcript being edited.
		/// </summary>
		[BindProperty]
		public Transcript Transcript { get; set; } = default!;

		/// <summary>
		/// GET handler that loads the transcript and its transactions.
		/// </summary>
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

		/// <summary>
		/// POST handler that saves edited transcript header values.
		/// </summary>
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