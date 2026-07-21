using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models;

/// <summary>
/// Application entry point for the PersonalFinanceTracker web application.
/// </summary>
public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServerOptions =>
                sqlServerOptions.EnableRetryOnFailure())
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

        // Add Identity with password rules: min 8 chars, digit, uppercase, lowercase, non-alphanumeric.
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            // Sign-in options
            options.SignIn.RequireConfirmedAccount = false;

            // Password rules
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            // Ensure username uniqueness is enforced by Identity store/index (database migration).
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var app = builder.Build();

// Apply any pending EF Core migrations on startup to ensure the database schema matches the model.
// This avoids runtime SQL errors such as "Invalid column name 'UserId'" when migrations haven't been applied.
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var db = services.GetRequiredService<ApplicationDbContext>();
	db.Database.Migrate();

	// Repair older LocalDB instances that were created before the Transactions.UserId migration.
	db.Database.ExecuteSqlRaw("""
		IF COL_LENGTH('dbo.Transactions', 'UserId') IS NULL
		BEGIN
			ALTER TABLE [dbo].[Transactions]
			ADD [UserId] nvarchar(450) NOT NULL CONSTRAINT [DF_Transactions_UserId] DEFAULT('');
		END

		IF NOT EXISTS (
			SELECT 1
			FROM sys.indexes
			WHERE name = 'IX_Transactions_UserId'
			  AND object_id = OBJECT_ID('dbo.Transactions')
		)
		BEGIN
			CREATE INDEX [IX_Transactions_UserId] ON [dbo].[Transactions] ([UserId]);
		END
		""");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

        // Important: authentication must come before authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapRazorPages().WithStaticAssets();

        using (var scope = app.Services.CreateScope())
        {
            // Apply pending migrations on startup so the LocalDB database exists before the app queries it.
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        app.Run();
    }
}