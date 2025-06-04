using Bogus;
using DataAccess.Contexts;
using DataAccess.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Moq.Protected;
using Shared.Entities.Interfaces;
using Shared.Repositories.Interfaces;
using Xunit;

namespace Tests.UnitTests.Repositories.Common
{
    public abstract class BaseRepositoryTests<TEntity, TRepository>
        where TEntity : class, IEntity
        where TRepository : BaseRepository<TEntity>, IRepository<TEntity>
    {
        protected abstract Faker<TEntity> Faker { get; init; }

        [Fact]
        public virtual async Task CreateAsync_ShouldAddEntityAndReturnId()
        {
            // Arrange
            var entity = Faker.Generate();
            var mockContext = CreateMockContext(CreateMockDbSet([]));

            mockContext.Setup(c => c.AddAsync(entity, It.IsAny<CancellationToken>()))
                .Verifiable();

            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Verifiable();

            var mockRepository = CreateMockRepository(mockContext);
            SetupProtectedDetachMethod(mockRepository, entity);

            // Act
            var result = await mockRepository.Object.CreateAsync(entity);

            // Assert
            result.Should().Be(entity.Id);
            mockContext.Verify(c => c.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            VerifyProtectedDetachMethod(mockRepository, entity);
        }

        [Fact]
        public virtual async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var entity = Faker.Generate();
            var mockContext = CreateMockContext(CreateMockDbSet([]));

            mockContext.Setup(c => c.Update(entity))
                .Verifiable();

            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Verifiable();

            var mockRepository = CreateMockRepository(mockContext);
            SetupProtectedDetachMethod(mockRepository, entity);

            // Act
            await mockRepository.Object.UpdateAsync(entity);

            // Assert
            mockContext.Verify(c => c.Update(entity), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            VerifyProtectedDetachMethod(mockRepository, entity);
        }

        [Fact]
        public virtual async Task DeleteAsync_ShouldRemoveEntity()
        {
            // Arrange
            var entity = Faker.Generate();
            var data = new List<TEntity> { entity };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);

            mockSet.Setup(s => s.Remove(entity))
                .Verifiable();

            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Verifiable();

            var mockRepository = CreateMockRepository(mockContext);

            // Act
            await mockRepository.Object.DeleteAsync(entity);

            // Assert
            mockSet.Verify(s => s.Remove(entity), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public virtual async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
        {
            // Arrange
            var entity = Faker.Generate();
            var data = new List<TEntity>
            {
                Faker.Generate(),
                entity
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);

            var mockRepository = CreateMockRepository(mockContext);

            // Act
            var receivedEntity = await mockRepository.Object.GetByIdAsync(entity.Id);

            // Assert
            receivedEntity.Should().NotBeNull();
            receivedEntity.Id.Should().Be(entity.Id);
        }

        [Fact]
        public virtual async Task GetAllAsync_ShouldReturnArrayOfEntities()
        {
            // Arrange
            var entities = new List<TEntity>
            {
                Faker.Generate(),
                Faker.Generate(),
                Faker.Generate()
            };

            var mockSet = CreateMockDbSet(entities);
            var mockContext = CreateMockContext(mockSet);

            var mockRepository = CreateMockRepository(mockContext);

            // Act
            var receivedEntities = await mockRepository.Object.GetAllAsync();

            // Assert
            receivedEntities.Should().BeEquivalentTo(entities);
        }

        [Theory]
        [InlineData(1, 10, 30)]
        [InlineData(2, 10, 30)]
        [InlineData(3, 10, 30)]
        [InlineData(1, 5, 12)]
        [InlineData(3, 5, 12)]
        [InlineData(1, 10, 0)]
        public virtual async Task GetPagedAsync_ShouldReturnPagedArrayOfUsers(int pageNumber, int pageSize, int totalEntities)
        {
            // Arrange
            var entities = Faker.Generate(totalEntities);

            var mockSet = CreateMockDbSet(entities);
            var mockContext = CreateMockContext(mockSet);

            var mockRepository = CreateMockRepository(mockContext);

            // Act
            var result = await mockRepository.Object.GetPagedAsync(pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.CurrentPage.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);

            var expectedPageCount = (int)Math.Ceiling(totalEntities / (float)pageSize);
            result.TotalPages.Should().Be(expectedPageCount);

            var expectedItemCount = Math.Min(pageSize, Math.Max(0, totalEntities - (pageNumber - 1) * pageSize));
            result.Items.Should().HaveCount(expectedItemCount);

            var expectedFirstIndex = (pageNumber - 1) * pageSize;
            if (expectedItemCount > 0)
                result.Items.First().Id.Should().Be(entities[expectedFirstIndex].Id);
        }

        protected virtual Mock<DbSet<TEntity>> CreateMockDbSet(IEnumerable<TEntity> data)
        {
            return data.BuildMock().BuildMockDbSet();
        }

        protected virtual Mock<UserDbContext> CreateMockContext(Mock<DbSet<TEntity>> mockSet)
        {
            var options = new DbContextOptions<UserDbContext>();
            var mockContext = new Mock<UserDbContext>(options);
            mockContext.Setup(c => c.Set<TEntity>()).Returns(mockSet.Object);
            return mockContext;
        }

        protected virtual Mock<TRepository> CreateMockRepository(Mock<UserDbContext> mockContext)
        {
            var mockRepository = new Mock<TRepository>(mockContext.Object) { CallBase = true };

            return mockRepository;
        }

        protected void SetupProtectedDetachMethod(Mock<TRepository> mockRepository, TEntity entity)
        {
            mockRepository.Protected()
                .Setup("DetachEntity", entity)
                .Verifiable();
        }

        protected void VerifyProtectedDetachMethod(Mock<TRepository> mockRepository, TEntity entity)
        {
            mockRepository.Protected().Verify("DetachEntity", Times.Once(), entity);
        }
    }
}
