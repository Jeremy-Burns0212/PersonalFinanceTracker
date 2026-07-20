using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transactions
{
	[Authorize]
	public class DetailsTransactionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public DetailsTransactionsModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public Transaction Transaction { get; set; } = default!;

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

			var transaction = await _context.Transactions
				.Include(t => t.Category)
				.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

			if (transaction is null)
			{
				return NotFound();
			}

			Transaction = transaction;
			return Page();
		}
	}
}
