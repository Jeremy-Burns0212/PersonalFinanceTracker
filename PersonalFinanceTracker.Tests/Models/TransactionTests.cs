using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PersonalFinanceTracker.Models;
using Xunit;

namespace PersonalFinanceTracker.Tests.Models
{
    public class TransactionTests
    {
        private static IList<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }

        [Fact]
        public void ValidTransaction_PassesValidation()
        {
            var t = new Transaction
            {
                Amount = 10.50m,
                Date = DateTime.UtcNow.Date,
                Description = "Test",
                Type = TransactionType.Income,
                CategoryId = 1
            };

            var results = Validate(t);
            Assert.Empty(results);
        }

        [Fact]
        public void NegativeAmount_FailsValidation()
        {
            var t = new Transaction
            {
                Amount = -5m,
                Date = DateTime.UtcNow.Date,
                Description = "Test",
                Type = TransactionType.Expense,
                CategoryId = 1
            };

            var results = Validate(t);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Transaction.Amount)));
        }

        [Fact]
        public void MissingDescription_FailsValidation()
        {
            var t = new Transaction
            {
                Amount = 5m,
                Date = DateTime.UtcNow.Date,
                Description = string.Empty,
                Type = TransactionType.Income,
                CategoryId = 1
            };

            var results = Validate(t);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Transaction.Description)));
        }
    }
}
