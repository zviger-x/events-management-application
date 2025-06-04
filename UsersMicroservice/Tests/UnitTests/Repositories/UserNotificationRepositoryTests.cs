using Bogus;
using DataAccess.Entities;
using DataAccess.Repositories;
using FluentAssertions;
using Shared.Enums;
using Tests.UnitTests.Repositories.Common;
using Xunit;

namespace Tests.UnitTests.Repositories
{
    public class UserNotificationRepositoryTests : BaseRepositoryTests<UserNotification, UserNotificationRepository>
    {
        protected override Faker<UserNotification> Faker { get; init; }

        public UserNotificationRepositoryTests()
        {
            Faker = new Faker<UserNotification>()
                .RuleFor(n => n.Id, f => Guid.NewGuid())
                .RuleFor(n => n.UserId, f => Guid.NewGuid())
                .RuleFor(n => n.Message, f => f.Random.Word())
                .RuleFor(n => n.DateTime, f => f.Date.Past())
                .RuleFor(n => n.Status, f => f.PickRandom<NotificationStatuses>());
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnNotification_WhenExists()
        {
            // Arrange
            var notification = Faker.Generate();
            var data = new List<UserNotification>
            {
                Faker.Generate(),
                notification
            };

            var mockSet = CreateMockDbSet(data);
            var mockContext = CreateMockContext(mockSet);
            mockContext.Setup(c => c.UserNotifications).Returns(mockSet.Object);

            var repository = CreateMockRepository(mockContext);

            // Act
            var notifications = await repository.Object.GetByUserIdAsync(notification.UserId);

            // Assert
            notifications.Should().NotBeNull();
            notifications.Should().AllSatisfy(t => t.UserId.Should().Be(notification.UserId));
        }
    }
}
