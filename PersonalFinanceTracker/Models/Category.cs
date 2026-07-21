using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
	/// <summary>
	/// Represents a category used to group transactions (for example: Groceries, Salary, Utilities).
	/// </summary>
	public class Category
	{
		/// <summary>
		/// Primary key.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The display name of the category.
		/// </summary>
		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Navigation property: transactions assigned to this category.
		/// </summary>
		public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
	}
}
