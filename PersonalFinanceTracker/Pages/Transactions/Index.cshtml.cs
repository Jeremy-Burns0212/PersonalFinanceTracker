using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace PersonalFinanceTracker.Pages.Transactions
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public IList<Transaction> Transactions { get; set; } = new List<Transaction>();

		[BindProperty]
		public string TranscriptName { get; set; } = string.Empty;

		public async Task OnGetAsync()
		{
			Transactions = await _context.Transactions
				.Include(t => t.Category)
				.OrderByDescending(t => t.Date)
				.ToListAsync();
		}

		public async Task<IActionResult> OnPostArchiveAsync()
		{
			var currentTransactions = await _context.Transactions
				.Include(transaction => transaction.Category)
				.OrderByDescending(transaction => transaction.Date)
				.ToListAsync();

			if (!currentTransactions.Any())
			{
				ModelState.AddModelError(string.Empty, "Add transactions before saving a transcript.");
				Transactions = currentTransactions;
				return Page();
			}

			if (string.IsNullOrWhiteSpace(TranscriptName))
			{
				ModelState.AddModelError(nameof(TranscriptName), "Transcript name is required.");
				Transactions = currentTransactions;
				return Page();
			}

			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser is null)
			{
				ModelState.AddModelError(string.Empty, "You must be signed in to save a transcript.");
				Transactions = currentTransactions;
				return Page();
			}

			var executionStrategy = _context.Database.CreateExecutionStrategy();

			await executionStrategy.ExecuteAsync(async () =>
			{
				await using var databaseTransaction = await _context.Database.BeginTransactionAsync();

				var transcript = new Transcript
				{
					Name = TranscriptName.Trim(),
					Description = $"Archived {currentTransactions.Count} transaction(s).",
					DateCreated = DateTime.UtcNow,
					LastAccessed = DateTime.UtcNow,
					UserId = currentUser.Id
				};

				_context.Transcripts.Add(transcript);
				await _context.SaveChangesAsync();

				_context.TranscriptTransactions.AddRange(currentTransactions.Select(transaction => new TranscriptTransaction
				{
					TranscriptId = transcript.Id,
					TransactionId = transaction.Id,
					Date = transaction.Date,
					Description = transaction.Description,
					Type = transaction.Type,
					CategoryName = transaction.Category?.Name ?? string.Empty,
					Amount = transaction.Amount,
				}));

				_context.Transactions.RemoveRange(currentTransactions);
				await _context.SaveChangesAsync();
				await databaseTransaction.CommitAsync();
			});

			return RedirectToPage("/Transcripts/Index");
		}
	}
}
