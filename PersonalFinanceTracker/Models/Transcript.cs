using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
	/// <summary>
	/// Represents a saved archive of a completed transaction batch.
	/// </summary>
	public class Transcript
	{
		/// <summary>
		/// Primary key.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// User-facing transcript name, such as "Taxes" or "Bills".
		/// </summary>
		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Optional description for the saved transcript.
		/// </summary>
		[StringLength(200)]
		public string? Description { get; set; }

		/// <summary>
		/// When the transcript was saved.
		/// </summary>
		public DateTime DateCreated { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// When the transcript was last opened or updated.
		/// </summary>
		public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// The owning user's identifier.
		/// </summary>
		[Required]
		public string UserId { get; set; } = string.Empty;

		/// <summary>
		/// Archived transaction rows captured in this transcript.
		/// </summary>
		public ICollection<TranscriptTransaction> Transactions { get; set; } = new List<TranscriptTransaction>();
	}
}