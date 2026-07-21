using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Pages.Transcripts
{
	/// <summary>
	/// Backing page model for the transcripts summary view.
	/// Groups existing transactions into monthly transcript rows.
	/// </summary>
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexModel"/> class.
		/// </summary>
		/// <param name="context">The application database context.</param>
		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Saved transcript archive rows shown in the table.
		/// </summary>
		public IList<TranscriptSummary> Transcripts { get; private set; } = new List<TranscriptSummary>();

		/// <summary>
		/// Loads saved transcript summaries from the archive tables.
		/// </summary>
		public async Task OnGetAsync()
		{
			var archivedTranscripts = await _context.Transcripts
				.AsNoTracking()
				.Include(t => t.Transactions)
				.ToListAsync();

			Transcripts = archivedTranscripts
				.OrderByDescending(transcript => transcript.DateCreated)
				.Select(transcript => new TranscriptSummary
				{
					Id = transcript.Id,
					Name = transcript.Name,
					Description = transcript.Description,
					DateCreated = transcript.DateCreated,
					LastAccessed = transcript.LastAccessed,
					UserId = transcript.UserId,
					TransactionCount = transcript.Transactions.Count,
					TotalIncome = transcript.Transactions.Where(transaction => transaction.Type == TransactionType.Income)
						.Sum(transaction => transaction.Amount),
					TotalExpenses = transcript.Transactions.Where(transaction => transaction.Type == TransactionType.Expense)
						.Sum(transaction => transaction.Amount),
				})
				.ToList();
		}

		/// <summary>
		/// Represents a single transcript archive row.
		/// </summary>
		public sealed class TranscriptSummary
		{
			/// <summary>
			/// Transcript identifier.
			/// </summary>
			public int Id { get; init; }

			/// <summary>
			/// Saved transcript name.
			/// </summary>
			public required string Name { get; init; }

			/// <summary>
			/// Optional transcript description.
			/// </summary>
			public string? Description { get; init; }

			/// <summary>
			/// When the transcript was saved.
			/// </summary>
			public DateTime DateCreated { get; init; }

			/// <summary>
			/// When the transcript was last accessed.
			/// </summary>
			public DateTime LastAccessed { get; init; }

			/// <summary>
			/// Owning user identifier.
			/// </summary>
			public required string UserId { get; init; }

			/// <summary>
			/// The number of transactions included in the transcript.
			/// </summary>
			public int TransactionCount { get; init; }

			/// <summary>
			/// Total income for the transcript period.
			/// </summary>
			public decimal TotalIncome { get; init; }

			/// <summary>
			/// Total expenses for the transcript period.
			/// </summary>
			public decimal TotalExpenses { get; init; }

			/// <summary>
			/// Net amount for the transcript period.
			/// </summary>
			public decimal NetAmount => TotalIncome - TotalExpenses;

			/// <summary>
			/// A simple status label for the summary row.
			/// </summary>
			public string Status => "Ready";
		}
	}
}