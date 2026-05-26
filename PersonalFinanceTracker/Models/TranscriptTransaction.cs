using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
	/// <summary>
	/// Represents a copied transaction row stored inside a transcript archive.
	/// </summary>
	public class TranscriptTransaction
	{
		/// <summary>
		/// Primary key.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Owning transcript.
		/// </summary>
		public int TranscriptId { get; set; }

		/// <summary>
		/// Navigation to the transcript header.
		/// </summary>
		public Transcript? Transcript { get; set; }

		/// <summary>
		/// Source transaction identifier from the working table.
		/// </summary>
		public int TransactionId { get; set; }

		/// <summary>
		/// Transaction date at the time of archiving.
		/// </summary>
		[Required]
		public DateOnly Date { get; set; }

		/// <summary>
		/// Transaction description at the time of archiving.
		/// </summary>
		[Required]
		[StringLength(200)]
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Income or expense classification.
		/// </summary>
		[Required]
		public TransactionType Type { get; set; }

		/// <summary>
		/// Category name snapshot taken when the transcript was created.
		/// </summary>
		[StringLength(100)]
		public string CategoryName { get; set; } = string.Empty;

		/// <summary>
		/// Transaction amount at the time of archiving.
		/// </summary>
		[Required]
		[Range(0.01, double.MaxValue)]
		public decimal Amount { get; set; }
	}
}