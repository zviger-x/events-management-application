using AutoMapper;
using Bogus;
using BusinessLogic.Contracts;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Shared.Enums;
using System.Text;
using Xunit;

namespace Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RegisterDto> _registerValidator;

        private readonly Faker _faker = new();
        private readonly Faker<RegisterDto> _registerDtoFaker;
        private readonly Faker<LoginDto> _loginDtoFaker;
        private readonly Faker<RefreshToken> _refreshTokenFaker;

        public AuthServiceTests()
        {
            _loginValidator = new LoginDtoValidator();
            _registerValidator = new RegisterDtoValidator();

            var pass = _faker.Internet.Password();
            _registerDtoFaker = new Faker<RegisterDto>()
                .RuleFor(r => r.Name, f => f.Name.FirstName())
                .RuleFor(r => r.Surname, f => f.Name.LastName())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Password, _ => pass)
                .RuleFor(r => r.ConfirmPassword, _ => pass);

            _loginDtoFaker = new Faker<LoginDto>()
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Password, f => f.Internet.Password());

            _refreshTokenFaker = new Faker<RefreshToken>()
                .RuleFor(r => r.Id, _ => Guid.NewGuid())
                .RuleFor(r => r.UserId, _ => Guid.NewGuid())
                .RuleFor(r => r.Token, f => Encoding.UTF8.GetString(f.Random.Bytes(64)))
                .RuleFor(r => r.Expires, f => f.Date.Future());
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnTokens_WhenValidInput()
        {
            // Arrange
            var registerDto = _registerDtoFaker.Generate();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                Email = registerDto.Email,
                Role = UserRoles.User
            };

            var jwtToken = "jwt-token";

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = "refresh-token",
                Expires = DateTime.UtcNow.AddMinutes(60),
            };

            var service = CreateService(
                out var unitOfWorkMock,
                out var mapperMock,
                out var passwordHashingServiceMock,
                out var tokenServiceMock);

            mapperMock.Setup(m => m.Map<User>(registerDto)).Returns(user);

            passwordHashingServiceMock.Setup(p => p.HashPassword(registerDto.Password)).Returns("hashedPassword");

            tokenServiceMock.Setup(t => t.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role)).Returns(jwtToken);
            tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns((refreshToken.Token, refreshToken.Expires));

            unitOfWorkMock.Setup(u => u.UserRepository.GetByEmailAsync(registerDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(User));

            unitOfWorkMock.Setup(u => u.RefreshTokenRepository.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(refreshToken));

            unitOfWorkMock.Setup(u => u.InvokeWithTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((f, token) => f(token));

            // Act
            var tokenInfo = await service.RegisterAsync(registerDto);

            // Assert
            tokenInfo.jwtToken.Should().Be(jwtToken);
            tokenInfo.refreshToken.Should().Be(refreshToken.Token);
        }

        [Theory]
        [InlineData("", "surname", "email@gmail.com", "password123", "password123")]
        [InlineData("name", "", "email@gmail.com", "password123", "password123")]
        [InlineData("name", "surname", "", "password123", "password123")]
        [InlineData("name", "surname", "email@gmail.com", "", "password123")]
        [InlineData("name", "surname", "email@gmail.com", "password123", "")]
        [InlineData(null, "surname", "email@gmail.com", "password123", "password123")]
        [InlineData("name", null, "email@gmail.com", "password123", "password123")]
        [InlineData("name", "surname", null, "password123", "password123")]
        [InlineData("name", "surname", "email@gmail.com", null, "password123")]
        [InlineData("name", "surname", "email@gmail.com", "password123", null)]
        [InlineData("name", "surname", "email@gmail.com", "password123", "password123123")]
        [InlineData("name", "surname", "email123gmail123com", "password123", "password123")]
        public async Task RegisterAsync_ShouldThrowException_WhenInvalidInputFields(
            string name, string surname, string email, string password, string confirmPassword)
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = name,
                Surname = surname,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            // Act
            var act = async () => await CreateService().RegisterAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailIsNotUnique()
        {
            // Arrange
            var registerDto = _registerDtoFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            unitOfWorkMock.Setup(u => u.UserRepository.ContainsEmailAsync(new string(registerDto.Email), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // Act
            var act = async () => await service.RegisterAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<Shared.Exceptions.ServerExceptions.ValidationException>();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokens_WhenValidInput()
        {
            // Arrange
            var loginDto = _loginDtoFaker.Generate();

            var userByEmail = new User
            {
                Id = Guid.NewGuid(),
                Email = loginDto.Email
            };

            var jwtToken = "jwt-token";

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userByEmail.Id,
                Token = "refresh-token",
                Expires = DateTime.UtcNow.AddMinutes(60),
            };

            var service = CreateService(
                out var unitOfWorkMock,
                out var mapperMock,
                out var passwordHashingServiceMock,
                out var tokenServiceMock);

            unitOfWorkMock.Setup(u => u.UserRepository.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(userByEmail));

            unitOfWorkMock.Setup(u => u.RefreshTokenRepository.GetByUserIdAsync(userByEmail.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(refreshToken));

            passwordHashingServiceMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            tokenServiceMock.Setup(t => t.GenerateJwtToken(userByEmail.Id, userByEmail.Name, userByEmail.Email, userByEmail.Role)).Returns(jwtToken);
            tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns((refreshToken.Token, refreshToken.Expires));

            // Act
            var tokenInfo = await service.LoginAsync(loginDto);

            // Assert
            tokenInfo.jwtToken.Should().Be(jwtToken);
            tokenInfo.refreshToken.Should().Be(refreshToken.Token);
        }

        [Theory]
        [InlineData("admin@gmail.com", "")]
        [InlineData("", "admin")]
        [InlineData("admin@gmail.com", null)]
        [InlineData(null, "admin")]
        [InlineData("admin123gmail123com", "admin")]
        public async Task LoginAsync_ShouldThrowException_WhenInvalidInputFields(string email, string password)
        {
            // Arrange
            var loginDto = new LoginDto { Email = email, Password = password };

            var service = CreateService();

            // Act
            var act = async () => await service.LoginAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = _loginDtoFaker.Generate();
            var userByEmail = new User { Email = loginDto.Email };

            var service = CreateService(out var unitOfWorkMock, out _, out var passwordHashingServiceMock, out _);

            unitOfWorkMock.Setup(u => u.UserRepository.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(userByEmail));

            passwordHashingServiceMock.Setup(p => p.VerifyPassword(loginDto.Password, userByEmail.PasswordHash))
                .Returns(false);

            // Act
            var act = async () => await service.LoginAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<Shared.Exceptions.ServerExceptions.ValidationException>();
        }

        [Fact]
        public async Task LogoutAsync_ShouldDeleteRefreshToken()
        {
            // Arrange
            var token = _refreshTokenFaker.Generate();
            var tokens = new List<RefreshToken>
            {
                _refreshTokenFaker.Generate(),
                _refreshTokenFaker.Generate(),
                _refreshTokenFaker.Generate(),
                token
            };

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            var dbSetMock = tokens.BuildMock().BuildMockDbSet();
            dbSetMock.Setup(d => d.Remove(It.IsAny<RefreshToken>()))
                .Callback<RefreshToken>(entity => tokens.Remove(entity));

            var dbContextMock = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            dbContextMock.Setup(c => c.Set<RefreshToken>()).Returns(dbSetMock.Object);
            dbContextMock.Setup(c => c.RefreshTokens).Returns(dbSetMock.Object);

            var repository = new RefreshTokenRepository(dbContextMock.Object);
            unitOfWorkMock.Setup(u => u.RefreshTokenRepository).Returns(repository);

            // Act
            await service.LogoutAsync(token.UserId);

            // Assert
            var currentTokens = await repository.GetAllAsync();
            currentTokens.Should().AllSatisfy(t => t.Should().NotBeEquivalentTo(token));
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldTokenValue = "old-refresh-token";
            var newTokenValue = "new-refresh-token";
            var jwt = "new-jwt-token";

            var user = new User { Id = userId };

            var existingRefreshToken = new RefreshToken
            {
                UserId = userId,
                Token = oldTokenValue,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            var service = CreateService(out var unitOfWorkMock, out _, out _, out var tokenServiceMock);

            unitOfWorkMock.Setup(u => u.RefreshTokenRepository.GetByRefreshTokenAsync(oldTokenValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            unitOfWorkMock.Setup(u => u.RefreshTokenRepository.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            tokenServiceMock.Setup(t => t.GenerateRefreshToken())
                .Returns((newTokenValue, DateTime.UtcNow.AddMinutes(60)));

            tokenServiceMock.Setup(t => t.GenerateJwtToken(user.Id, user.Name, user.Email, user.Role))
                .Returns(jwt);

            // Act
            var result = await service.RefreshTokenAsync(oldTokenValue);

            // Assert
            result.jwtToken.Should().Be(jwt);
            result.refreshToken.Should().Be(newTokenValue);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenInvalidToken()
        {
            // Arrange
            var refreshToken = _refreshTokenFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            unitOfWorkMock.Setup(u => u.RefreshTokenRepository.GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(default(RefreshToken)));

            // Act
            var act = async () => await service.RefreshTokenAsync(refreshToken.Token);

            // Assert
            await act.Should().ThrowAsync<Shared.Exceptions.ServerExceptions.UnauthorizedException>();
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenUserAttachedToThisTokenNotFound()
        {
            // Arrange
            var refreshToken = _refreshTokenFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            unitOfWorkMock.Setup(u => u.RefreshTokenRepository.GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(refreshToken));

            unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(default(User)));

            // Act
            var act = async () => await service.RefreshTokenAsync(refreshToken.Token);

            // Assert
            await act.Should().ThrowAsync<Shared.Exceptions.ServerExceptions.UnauthorizedException>();
        }

        [Theory]
        [InlineData(null, "any", 1, false)]
        [InlineData("wrong", "expected", 1, false)]
        [InlineData("expected", "expected", -1, false)]
        [InlineData("expected", "expected", 1, true)]
        public async Task ValidateRefreshToken_Should_Return_Correct_Validity(
            string storedToken, string inputToken, int minutesOffset, bool expectedIsValid)
        {
            // Arrange
            var storedRefreshToken = storedToken == null ? null : new RefreshToken
            {
                Token = storedToken,
                UserId = Guid.NewGuid(),
                Expires = DateTime.UtcNow.AddMinutes(minutesOffset)
            };

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            var repository = new Mock<IRefreshTokenRepository>();
            repository.Setup(r => r.GetByRefreshTokenAsync(inputToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(storedRefreshToken);

            unitOfWorkMock.Setup(u => u.RefreshTokenRepository).Returns(repository.Object);

            // Act
            var result = await service.ValidateRefreshTokenAsync(inputToken);

            // Assert
            result.IsValid.Should().Be(expectedIsValid);
        }

        private AuthService CreateService() => CreateService(out _, out _, out _, out _);

        private AuthService CreateService(
            out Mock<IUnitOfWork> unitOfWorkMock,
            out Mock<IMapper> mapperMock,
            out Mock<IPasswordHashingService> passwordHashingServiceMock,
            out Mock<ITokenService> tokenServiceMock)
        {
            unitOfWorkMock = new();
            mapperMock = new();
            passwordHashingServiceMock = new();
            tokenServiceMock = new();

            return new(
                unitOfWorkMock.Object,
                mapperMock.Object,
                _loginValidator,
                _registerValidator,
                passwordHashingServiceMock.Object,
                tokenServiceMock.Object);
        }
    }
}
