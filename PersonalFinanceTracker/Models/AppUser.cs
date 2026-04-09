using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models
{
	public class AppUser
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string FullName { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[StringLength(150)]
		public string Email { get; set; } = string.Empty;
	}
}
