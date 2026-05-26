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
		/// Transcript archive headers table.
		/// </summary>
		public DbSet<Transcript> Transcripts => Set<Transcript>();

		/// <summary>
		/// Archived transaction rows for each transcript.
		/// </summary>
		public DbSet<TranscriptTransaction> TranscriptTransactions => Set<TranscriptTransaction>();

		/// <summary>
		/// Categories table.
		/// </summary>
		public DbSet<Category> Categories => Set<Category>();

		// Note: IdentityDbContext already exposes Users, Roles, etc. Do not redeclare Users here.

		/// <summary>
		/// Configure EF Core model mappings and property precision/column types.
		/// </summary>
		/// <param name="modelBuilder">Model builder instance used to configure entity mappings.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Ensure the Amount column has sufficient precision to avoid truncation warnings.
			modelBuilder.Entity<Transaction>()
				.Property(t => t.Amount)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Transcript>()
				.Property(t => t.DateCreated)
				.HasColumnType("datetime2");

			modelBuilder.Entity<Transcript>()
				.Property(t => t.LastAccessed)
				.HasColumnType("datetime2");

			modelBuilder.Entity<TranscriptTransaction>()
				.Property(t => t.Amount)
				.HasPrecision(18, 2);
		}
	}
}
