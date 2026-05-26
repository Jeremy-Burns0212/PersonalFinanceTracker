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
	public class DeleteModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of <see cref="DeleteModel"/>.
		/// </summary>
		/// <param name="context">Application DB context for accessing transcripts.</param>
		public DeleteModel(ApplicationDbContext context)
		{
			_context = context;
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

			var transcript = await _context.Transcripts
				.AsNoTracking()
				.FirstOrDefaultAsync(item => item.Id == id);

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

			var transcript = await _context.Transcripts.FindAsync(id);
			if (transcript is not null)
			{
				_context.Transcripts.Remove(transcript);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index");
		}
	}
}