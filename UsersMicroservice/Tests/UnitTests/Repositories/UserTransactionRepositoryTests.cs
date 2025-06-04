using Bogus;
using DataAccess.Entities;
using DataAccess.Repositories;
using FluentAssertions;
using Tests.UnitTests.Repositories.Common;
using Xunit;

namespace Tests.UnitTests.Repositories
{
    public class UserTransactionRepositoryTests : BaseRepositoryTests<UserTransaction, UserTransactionRepository>
    {
        protected override Faker<UserTransaction> Faker { get; init; }

        public UserTransactionRepositoryTests()
        {
            Faker = new Faker<UserTransaction>()
                .RuleFor(t => t.Id, f => Guid.NewGuid())
                .RuleFor(t => t.UserId, f => Guid.NewGuid())
                .RuleFor(t => t.EventId, f => Guid.NewGuid())
                .RuleFor(t => t.EventName, f => f.Random.Word())
                .RuleFor(t => t.SeatRow, f => f.Random.Int(min: 1))
                .RuleFor(t => t.SeatNumber, f => f.Random.Int(min: 1))
                .RuleFor(t => t.Amount, f => f.Random.Float(min: 0.01f))
                .RuleFor(t => t.TransactionDate, f => f.Date.Past());
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnTransaction_WhenExists()
        {
            // Arrange
            var transaction = Faker.Generate();
            var data = new List<UserTransaction>
            {
                Faker.Generate(),
                transaction
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.UserTransactions).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var transactions = await repository.Object.GetByUserIdAsync(transaction.UserId);

            // Assert
            transactions.Should().NotBeNull();
            transactions.Should().AllSatisfy(t => t.UserId.Should().Be(transaction.UserId));
        }
    }
}
