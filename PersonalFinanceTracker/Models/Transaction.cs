using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
	/// <summary>
	/// Represents a financial transaction (income or expense).
	/// </summary>
	public class Transaction
	{
		/// <summary>
		/// Primary key.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// the currency amount of the transaction. Must be positive.
		/// </summary>
		[Required]
		[Range(0.01, double.MaxValue)]
		public decimal Amount { get; set; }

		/// <summary>
		/// Date of the transaction.
		/// </summary>
		[Required]
		[DataType(DataType.Date)]
		public required DateOnly Date { get; set; }

		/// <summary>
		/// Short description or memo for the transaction.
		/// </summary>
		[Required]
		[StringLength(200)]
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Whether the transaction is an income or an expense.
		/// </summary>
		[Required]
		public TransactionType Type { get; set; }

		/// <summary>
		/// Foreign key to the assigned category.
		/// </summary>
		[Required]
		public int CategoryId { get; set; }

		/// <summary>
		/// Navigation property to the category assigned to this transaction.
		/// </summary>
		public Category? Category { get; set; }

		[StringLength(450)]
		public string UserId { get; set; } = string.Empty;

		public AppUser? User { get; set; }
	}

	/// <summary>
	/// Classification for a transaction indicating whether it is income or expense.
	/// </summary>
	public enum TransactionType
	{
		/// <summary>
		/// its an income.
		/// </summary>
		Income = 1,

		/// <summary>
		/// its an expense.
		/// </summary>
		Expense = 2
	}
}
