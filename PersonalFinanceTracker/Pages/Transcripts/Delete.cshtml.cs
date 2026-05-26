using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	public class DeleteModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public DeleteModel(ApplicationDbContext context)
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
				.AsNoTracking()
				.FirstOrDefaultAsync(item => item.Id == id);

			if (transcript is null)
			{
				return NotFound();
			}

			Transcript = transcript;
			return Page();
		}

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