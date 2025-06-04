using Bogus;
using DataAccess.Entities;
using DataAccess.Repositories;
using FluentAssertions;
using System.Text;
using Tests.UnitTests.Repositories.Common;
using Xunit;

namespace Tests.UnitTests.Repositories
{
    public class RefreshTokenRepositoryTests : BaseRepositoryTests<RefreshToken, RefreshTokenRepository>
    {
        protected override Faker<RefreshToken> Faker { get; init; }

        public RefreshTokenRepositoryTests()
        {
            Faker = new Faker<RefreshToken>()
                .RuleFor(n => n.Id, f => Guid.NewGuid())
                .RuleFor(n => n.UserId, f => Guid.NewGuid())
                .RuleFor(n => n.Token, f => Encoding.UTF8.GetString(f.Random.Bytes(64)))
                .RuleFor(n => n.Expires, f => f.Date.Future());
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShouldReturnRefreshToken_WhenExists()
        {
            // Arrange
            var refreshToken = Faker.Generate();
            var data = new List<RefreshToken>
            {
                Faker.Generate(),
                refreshToken
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.RefreshTokens).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var receivedToken = await repository.Object.GetByRefreshTokenAsync(refreshToken.Token);

            // Assert
            receivedToken.Should().NotBeNull();
            receivedToken.Should().BeEquivalentTo(refreshToken);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnRefreshToken_WhenExists()
        {
            // Arrange
            var refreshToken = Faker.Generate();
            var data = new List<RefreshToken>
            {
                Faker.Generate(),
                refreshToken
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.RefreshTokens).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var receivedToken = await repository.Object.GetByUserIdAsync(refreshToken.UserId);

            // Assert
            receivedToken.Should().NotBeNull();
            receivedToken.Should().BeEquivalentTo(refreshToken);
        }
    }
}
