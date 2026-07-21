using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinanceTracker.Pages.Transactions
{
	
	/// <summary>
	/// Page model for listing transactions and saving them as a transcript archive.
	/// </summary>
	[Authorize]
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexModel"/> class.
		/// </summary>
		/// <param name="context">The application database context.</param>
		/// <param name="userManager">The user manager for handling user information.</param>
		public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		/// <summary>
		/// Transactions shown in the list view.
		/// </summary>
		public IList<Transaction> Transactions { get; set; } = new List<Transaction>();

		/// <summary>
		/// Name for the transcript when archiving transactions.
		/// </summary>
		[BindProperty]
		public string TranscriptName { get; set; } = string.Empty;

		/// <summary>
		/// Loads the current transactions for display.
		/// </summary>
		public async Task OnGetAsync()
		{
			var userId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(userId))
			{
				Transactions = new List<Transaction>();
				return;
			}

			IOrderedQueryable<Transaction> transactions = _context.Transactions
				.Include(t => t.Category)
				.Where(t => t.UserId == userId)
				.OrderByDescending(t => t.Date);
			Transactions = await transactions
				.ToListAsync();
		}

		/// <summary>
		/// Archives the current transactions into a transcript and removes them from the working table.
		/// </summary>
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
