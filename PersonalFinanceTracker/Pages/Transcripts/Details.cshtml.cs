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
	public class DetailsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of <see cref="DetailsModel"/>.
		/// </summary>
		/// <param name="context">Application DB context for loading transcripts and transactions.</param>
		public DetailsModel(ApplicationDbContext context)
		{
			_context = context;
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

			var transcript = await _context.Transcripts
				.AsNoTracking()
				.Include(item => item.Transactions)
				.FirstOrDefaultAsync(item => item.Id == id);

			if (transcript is null)
			{
				return NotFound();
			}

			Transcript = transcript;
			return Page();
		}
	}
}