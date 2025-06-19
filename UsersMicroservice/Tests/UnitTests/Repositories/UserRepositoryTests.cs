using Bogus;
using DataAccess.Entities;
using DataAccess.Repositories;
using FluentAssertions;
using Shared.Enums;
using System.Text;
using Tests.UnitTests.Repositories.Common;
using Xunit;

namespace Tests.UnitTests.Repositories
{
    public class UserRepositoryTests : BaseRepositoryTests<User, UserRepository>
    {
        protected override Faker<User> Faker { get; init; }

        public UserRepositoryTests()
        {
            Faker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, f => Encoding.UTF8.GetString(f.Random.Bytes(64)))
                .RuleFor(u => u.Role, f => f.PickRandom<UserRoles>());
        }

        [Fact]
        public override async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
        {
            // Arrange
            var user = Faker.Generate();
            var data = new List<User>
            {
                Faker.Generate(),
                user
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var entity = await repository.Object.GetByIdAsync(user.Id);

            // Assert
            entity.Should().NotBeNull();
            entity.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task ContainsEmailAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var user = Faker.Generate();
            var data = new List<User>
            {
                Faker.Generate(),
                user
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var exists = await repository.Object.ContainsEmailAsync(user.Email);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ContainsEmailAsync_ShouldReturnFalse_WhenEmailNotExists()
        {
            // Arrange
            var fakeEmail = new Faker().Internet.Email();
            var data = new List<User>
            {
                Faker.Generate(),
                Faker.Generate()
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var exists = await repository.Object.ContainsEmailAsync(fakeEmail);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            var user = Faker.Generate();
            var data = new List<User>
            {
                Faker.Generate(),
                user
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var result = await repository.Object.GetByEmailAsync(user.Email);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailNotExists()
        {
            // Arrange
            var fakeEmail = new Faker().Internet.Email();
            var data = new List<User>
            {
                Faker.Generate(),
                Faker.Generate()
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var result = await repository.Object.GetByEmailAsync(fakeEmail);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task IsExists_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var user = Faker.Generate();
            var data = new List<User>
            {
                user,
                Faker.Generate()
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var exists = await repository.Object.IsExistsAsync(user.Id);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task IsExists_ShouldReturnFalse_WhenUserNotExists()
        {
            // Arrange
            var data = new List<User>
            {
                Faker.Generate(),
                Faker.Generate()
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var exists = await repository.Object.IsExistsAsync(Guid.NewGuid());

            // Assert
            exists.Should().BeFalse();
        }
    }
}
