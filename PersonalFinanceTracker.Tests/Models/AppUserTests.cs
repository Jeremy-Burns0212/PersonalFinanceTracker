using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PersonalFinanceTracker.Models;
using Xunit;

namespace PersonalFinanceTracker.Tests.Models
{
    public class AppUserTests
    {
        private static IList<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void ValidUser_PassesValidation()
        {
            var u = new AppUser
            {
                FullName = "Jane Doe",
                Email = "jane@example.com"
            };

            var results = Validate(u);
            Assert.Empty(results);
        }

        [Fact]
        public void MissingFullName_FailsValidation()
        {
            var u = new AppUser
            {
                FullName = string.Empty,
                Email = "jane@example.com"
            };

            var results = Validate(u);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AppUser.FullName)));
        }

        [Fact]
        public void MissingEmail_FailsValidation()
        {
            var u = new AppUser
            {
                FullName = "Jane Doe",
                Email = string.Empty
            };

            var results = Validate(u);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AppUser.Email)));
        }

        [Fact]
        public void InvalidEmail_FailsValidation()
        {
            var u = new AppUser
            {
                FullName = "Jane Doe",
                Email = "not-an-email"
            };

            var results = Validate(u);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AppUser.Email)));
        }
    }
}
