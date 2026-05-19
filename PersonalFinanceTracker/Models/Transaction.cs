using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
	public class Transaction
	{
		public int Id { get; set; }

		[Required]
		[Range(0.01, double.MaxValue)]
		public decimal Amount { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime Date { get; set; } = DateTime.UtcNow.Date;

		[Required]
		[StringLength(200)]
		public string Description { get; set; } = string.Empty;

		[Required]
		public TransactionType Type { get; set; }

		[Required]
		public int CategoryId { get; set; }

		public Category? Category { get; set; }
	}

	public enum TransactionType
	{
		Income = 1,
		Expense = 2
	}
}
