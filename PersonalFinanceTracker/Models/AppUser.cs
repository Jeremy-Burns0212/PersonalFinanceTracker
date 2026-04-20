using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PersonalFinanceTracker.Models
{
	/// <summary>
	/// Represents an application user for Identity with application-specific properties.
	/// Inherits from <see cref="IdentityUser"/> so Identity APIs work correctly.
	/// </summary>
	public class AppUser : IdentityUser
	{
		/// <summary>
		/// Gets or sets the full name of the user.
		/// </summary>
		[Required]
		[StringLength(100)]
		public string FullName { get; set; } = string.Empty;
	}
}
