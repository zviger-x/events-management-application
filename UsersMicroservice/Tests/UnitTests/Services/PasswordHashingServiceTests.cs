using Bogus;
using BusinessLogic.Services;
using FluentAssertions;
using Xunit;

namespace Tests.UnitTests.Services
{
    public class PasswordHashingServiceTests
    {
        private readonly Faker _faker;
        private readonly PasswordHashingService _service;

        public PasswordHashingServiceTests()
        {
            _faker = new Faker();
            _service = new PasswordHashingService();
        }

        [Fact]
        public void HashPassword_ShouldReturnNonEmptyHash()
        {
            // Arrange
            var password = _faker.Internet.Password();

            // Act
            var hash = _service.HashPassword(password);

            // Assert
            hash.Should().NotBeNullOrWhiteSpace();
            hash.Should().NotBe(password);
        }

        [Fact]
        public void HashPassword_ShouldReturnDifferentHashesForSamePassword()
        {
            // Arrange
            var password = _faker.Internet.Password();

            // Act
            var hash1 = _service.HashPassword(password);
            var hash2 = _service.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrueForValidPassword()
        {
            // Arrange
            var password = _faker.Internet.Password();
            var hash = _service.HashPassword(password);

            // Act
            var result = _service.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalseForInvalidPassword()
        {
            // Arrange
            var correctPassword = _faker.Internet.Password();
            var wrongPassword = _faker.Internet.Password();
            var hash = _service.HashPassword(correctPassword);

            // Act
            var result = _service.VerifyPassword(wrongPassword, hash);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalseForEmptyHash()
        {
            // Arrange
            var password = _faker.Internet.Password();

            // Act
            var result = _service.VerifyPassword(password, "");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HashPassword_ShouldThrowException_WhenPasswordIsNull()
        {
            // Act
            var act = () => _service.HashPassword(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void VerifyPassword_ShouldThrowException_WhenPasswordIsNull()
        {
            // Arrange
            var password = _faker.Internet.Password();
            var hash = _service.HashPassword(password);

            // Act
            var act = () => _service.VerifyPassword(null, hash);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void VerifyPassword_ShouldThrowException_WhenHashIsNull()
        {
            // Arrange
            var password = _faker.Internet.Password();

            // Act
            var act = () => _service.VerifyPassword(password, null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
