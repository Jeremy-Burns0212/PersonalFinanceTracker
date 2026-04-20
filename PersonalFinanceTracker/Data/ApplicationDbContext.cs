using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Data
{
	/// <summary>
	/// Application database context configured for ASP.NET Core Identity and application entities.
	/// Inherits from <see cref="IdentityDbContext{TUser}"/> so Identity stores are available.
	/// </summary>
	public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ApplicationDbContext"/>.
		/// </summary>
		/// <param name="options">The options used by a <see cref="DbContext"/>.</param>
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		/// <summary>
		/// Transactions table.
		/// </summary>
		public DbSet<Transaction> Transactions => Set<Transaction>();

		/// <summary>
		/// Categories table.
		/// </summary>
		public DbSet<Category> Categories => Set<Category>();

		// Note: IdentityDbContext already exposes Users, Roles, etc. Do not redeclare Users here.
	}
}
