using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
	public class Category
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;

		public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
	}
}
