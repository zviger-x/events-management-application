using AutoMapper;
using Bogus;
using BusinessLogic.Caching.Constants;
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
using Shared.Caching.Services.Interfaces;
using Shared.Common;
using Shared.Enums;
using Shared.Exceptions.ServerExceptions;
using Shared.Validation.Validators;
using System.Text;
using Xunit;

namespace Tests.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly IValidator<UpdateUserDto> _updateUserValidator;
        private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
        private readonly IValidator<ChangeUserRoleDto> _changeUserRoleValidator;
        private readonly IValidator<PageParameters> _pageParametersValidator;

        private readonly Faker _faker = new();
        private readonly Faker<User> _userFaker;
        private readonly Faker<UpdateUserDto> _updateUserFaker;
        private readonly Faker<ChangePasswordDto> _changePasswordFaker;
        private readonly Faker<ChangeUserRoleDto> _changeRoleFaker;

        public UserServiceTests()
        {
            _updateUserValidator = new UpdateUserDtoValidator();
            _changePasswordValidator = new ChangePasswordDtoValidator();
            _changeUserRoleValidator = new ChangeUserRoleDtoValidator();
            _pageParametersValidator = new PageParametersValidator();

            _userFaker = new Faker<User>()
                .RuleFor(r => r.Id, _ => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FirstName())
                .RuleFor(r => r.Surname, f => f.Name.LastName())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.PasswordHash, f => Encoding.UTF8.GetString(f.Random.Bytes(64)))
                .RuleFor(r => r.Role, f => f.PickRandom<UserRoles>());

            _updateUserFaker = new Faker<UpdateUserDto>()
                .RuleFor(r => r.Name, f => f.Name.FirstName())
                .RuleFor(r => r.Surname, f => f.Name.LastName());

            var newPass = _faker.Internet.Password();
            _changePasswordFaker = new Faker<ChangePasswordDto>()
                .RuleFor(r => r.CurrentPassword, f => f.Internet.Password())
                .RuleFor(r => r.NewPassword, _ => newPass)
                .RuleFor(r => r.ConfirmPassword, _ => newPass);

            _changeRoleFaker = new Faker<ChangeUserRoleDto>()
                .RuleFor(r => r.Role, f => f.PickRandom<UserRoles>());
        }

        [Theory]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", false)]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", true)]
        public async Task DeleteAsync_ShouldBeValid_WhenSelfOrAdmin(string targetIdAsString, string currentIdAsString, bool isAdmin)
        {
            // Arrange
            var targetId = Guid.Parse(targetIdAsString);
            var currentId = Guid.Parse(currentIdAsString);

            var user = new User { Id = targetId };

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(targetId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            repository.Setup(r => r.DeleteAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);
            unitOfWorkMock.Setup(u => u.InvokeWithTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
               .Returns<Func<CancellationToken, Task>, CancellationToken>((func, token) => func(token));

            // Act
            await service.DeleteAsync(targetId, currentId, isAdmin);

            // Assert
            repository.Verify(r => r.DeleteAsync(It.Is<User>(u => u.Id == targetId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_ForbiddenAccessException_WhenNotCurrentUser_AndNotAdmin()
        {
            // Arrange
            var targetUserId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var service = CreateService();

            // Act
            var act = () => service.DeleteAsync(targetUserId, currentUserId, isAdmin: false);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_NotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(User));

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);
            unitOfWorkMock.Setup(u => u.InvokeWithTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
               .Returns<Func<CancellationToken, Task>, CancellationToken>((func, token) => func(token));

            // Act
            var act = () => service.DeleteAsync(userId, userId, isAdmin: false);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnArrayOfUsers()
        {
            // Arrange
            var users = _userFaker.Generate(3);

            var mapUserFunc = GetMapperFunctionUserToDto();
            var mapListFunc = GetMapperFunctionListToList(mapUserFunc);

            var dbSetMock = users.BuildMock().BuildMockDbSet();
            var dbContextMock = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            dbContextMock.Setup(c => c.Set<User>()).Returns(dbSetMock.Object);
            dbContextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

            var repository = new UserRepository(dbContextMock.Object);
            var service = CreateService(out var unitOfWorkMock, out var mapperMock, out _, out var cacheServiceMock);

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository);

            mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(mapUserFunc);
            mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(It.IsAny<IEnumerable<User>>())).Returns(mapListFunc);

            cacheServiceMock.Setup(c => c.GetAsync<IEnumerable<UserDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<UserDto>)null);

            // Act
            var receivedUsers = await service.GetAllAsync();

            // Assert
            var expectedDtos = users.Select(mapUserFunc);
            receivedUsers.Should().BeEquivalentTo(expectedDtos, opt => opt.ExcludingMissingMembers().WithStrictOrdering());
        }

        [Theory]
        [InlineData(3, 1, 2)]
        [InlineData(3, 2, 2)]
        [InlineData(5, 1, 5)]
        [InlineData(7, 3, 3)]
        [InlineData(10, 5, 2)]
        public async Task GetPagedAsync_ShouldReturnPagedUsers(int totalUsers, int pageNumber, int pageSize)
        {
            // Arrange
            var users = _userFaker.Generate(totalUsers);

            var pageParameters = new PageParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var mapUserFunc = GetMapperFunctionUserToDto();
            var mapPageFunc = GetMapperFunctionPageToPage(mapUserFunc);

            var dbSetMock = users.BuildMock().BuildMockDbSet();
            var dbContextMock = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            dbContextMock.Setup(c => c.Set<User>()).Returns(dbSetMock.Object);
            dbContextMock.Setup(c => c.Users).Returns(dbSetMock.Object);

            var repository = new UserRepository(dbContextMock.Object);
            var service = CreateService(out var unitOfWorkMock, out var mapperMock, out _, out var cacheServiceMock);

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository);
            mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(mapUserFunc);
            mapperMock.Setup(m => m.Map<PagedCollection<UserDto>>(It.IsAny<PagedCollection<User>>())).Returns(mapPageFunc);

            cacheServiceMock.Setup(c => c.GetAsync<PagedCollection<UserDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PagedCollection<UserDto>)null);

            // Act
            var result = await service.GetPagedAsync(pageParameters);

            // Assert
            var expectedRawItems = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var expectedTotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            result.Should().NotBeNull();
            result.PageSize.Should().Be(pageSize);
            result.TotalPages.Should().Be(expectedTotalPages);
            result.Items.Count().Should().Be(expectedRawItems.Count);
            result.Items.Select(i => i.Id).Should().BeEquivalentTo(expectedRawItems.Select(u => u.Id));
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(1, 101)]
        public async Task GetPagedAsync_ShouldThrow_WhenInvalidPageParameters(int pageNumber, int pageSize)
        {
            // Arrange
            var users = _userFaker.Generate(3);

            var pageParameters = new PageParameters { PageNumber = pageNumber, PageSize = pageSize };

            var service = CreateService();

            // Act
            var act = async () => await service.GetPagedAsync(pageParameters);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Theory]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", false)]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", true)]
        public async Task GetByIdAsync_ShouldReturnUserDto_WhenSelfOrAdmin(string targetIdAsString, string currentIdAsString, bool isAdmin)
        {
            // Arrange
            var targetId = Guid.Parse(targetIdAsString);
            var currentId = Guid.Parse(currentIdAsString);

            var user = _userFaker.Generate();
            user.Id = currentId;

            var mapUserFunc = GetMapperFunctionUserToDto();
            var expectedUser = mapUserFunc(user);

            var service = CreateService(out var unitOfWorkMock, out var mapperMock, out _, out _);

            unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(user));

            mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(mapUserFunc);

            // Act
            var receivedUser = await service.GetByIdAsync(currentId, targetId, isAdmin);

            // Assert
            receivedUser.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_ForbiddenAccessException_WhenNotCurrentUser_AndNotAdmin()
        {
            // Arrange
            var currentId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var isAdmin = false;

            var service = CreateService();

            // Act
            var act = async () => await service.GetByIdAsync(currentId, targetId, isAdmin);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_NotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var user = _userFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(default(User)));

            // Act
            var act = async () => await service.GetByIdAsync(user.Id, user.Id, false);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldUpdateProfile()
        {
            // Arrange
            var user = _userFaker.Generate();
            var updateDto = new UpdateUserDto
            {
                Name = "UpdatedName",
                Surname = "UpdatedSurname"
            };

            var service = CreateService(out var unitOfWorkMock, out var mapperMock, out _, out var cacheServiceMock);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            repository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Verifiable();

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);
            unitOfWorkMock.Setup(u => u.InvokeWithTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

            mapperMock.Setup(m => m.Map(updateDto, user));

            cacheServiceMock.Setup(c => c.RemoveAsync(CacheKeys.UserById(user.Id), It.IsAny<CancellationToken>())).Verifiable();

            // Act
            await service.UpdateUserProfileAsync(user.Id, user.Id, false, updateDto, CancellationToken.None);

            // Assert
            repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map(updateDto, user), Times.Once);
            cacheServiceMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldThrow_NotFoundException_WhenUserNotFound()
        {
            // Arrange
            var currentId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var isAdmin = false;
            var updateDto = _updateUserFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            var repository = new Mock<IUserRepository>();
            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);

            // Act
            var act = async () => await service.UpdateUserProfileAsync(currentId, targetId, isAdmin, updateDto, default);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldThrow_ForbiddenAccessException_WhenNotCurrentUser_AndNotAdmin()
        {
            // Arrange
            var currentId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var isAdmin = false;
            var updateDto = _updateUserFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new User { Id = currentId }));

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);

            // Act
            var act = async () => await service.UpdateUserProfileAsync(currentId, targetId, isAdmin, updateDto, default);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldChangePassword()
        {
            // Arrange
            var currentPassword = _faker.Internet.Password();
            var newPassword = _faker.Internet.Password();

            var user = _userFaker.Generate();
            user.PasswordHash = currentPassword;

            var changePasswordDto = _changePasswordFaker.Generate();
            changePasswordDto.CurrentPassword = currentPassword;

            var service = CreateService(out var unitOfWorkMock, out _, out var passwordHashingServiceMock, out _);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            repository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Verifiable();

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);
            unitOfWorkMock.Setup(u => u.InvokeWithTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

            passwordHashingServiceMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string a, string b) => string.Equals(a, b))
                .Verifiable();

            // Act
            await service.ChangePasswordAsync(user.Id, user.Id, false, changePasswordDto, default);

            // Assert
            repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            passwordHashingServiceMock.Verify(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrow_NotFoundException_WhenUserNotFound()
        {
            // Arrange
            var currentId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var isAdmin = false;
            var changePasswordDto = _changePasswordFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            var repository = new Mock<IUserRepository>();
            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);

            // Act
            var act = async () => await service.ChangePasswordAsync(currentId, targetId, isAdmin, changePasswordDto, default);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrow_ForbiddenAccessException_WhenNotCurrentUser_AndNotAdmin()
        {
            // Arrange
            var currentId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var isAdmin = false;
            var changePasswordDto = _changePasswordFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new User { Id = currentId }));

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);

            // Act
            var act = async () => await service.ChangePasswordAsync(currentId, targetId, isAdmin, changePasswordDto, default);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Theory]
        [InlineData("", "new", "confirm")]
        [InlineData("current", "", "confirm")]
        [InlineData("current", "new", "")]
        [InlineData(null, "new", "confirm")]
        [InlineData("current", null, "confirm")]
        [InlineData("current", "new", null)]
        public async Task ChangePasswordAsync_ShouldThrow_ValidationException_WhenInvalidDto(
            string currentPassword, string newPassword, string confirmPassword)
        {
            // Arrange
            var userId = Guid.NewGuid();

            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword,
            };

            var service = CreateService();

            // Act
            var act = async () => await service.ChangePasswordAsync(userId, userId, false, changePasswordDto, default);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrow_ValidationException_WhenInvalidPassword()
        {
            // Arrange
            var currentPassword = _faker.Internet.Password();
            var newPassword = _faker.Internet.Password();

            var user = _userFaker.Generate();
            user.PasswordHash = currentPassword;

            var changePasswordDto = _changePasswordFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out var passwordHashingServiceMock, out _);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            repository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Verifiable();

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);
            unitOfWorkMock.Setup(u => u.InvokeWithTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

            passwordHashingServiceMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string a, string b) => string.Equals(a, b))
                .Verifiable();

            // Act
            var act = async () => await service.ChangePasswordAsync(user.Id, user.Id, false, changePasswordDto, default);

            // Assert
            await act.Should().ThrowAsync<Shared.Exceptions.ServerExceptions.ValidationException>();
        }

        [Fact]
        public async Task ChangeUserRoleAsync_ShouldChangeRole()
        {
            // Arrange
            var role = UserRoles.User;
            var changeRole = new ChangeUserRoleDto { Role = UserRoles.Admin };

            var user = _userFaker.Generate();
            user.Role = role;

            var service = CreateService(out var unitOfWorkMock, out _, out var passwordHashingServiceMock, out _);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            repository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Verifiable();

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);
            unitOfWorkMock.Setup(u => u.InvokeWithTransactionAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

            // Act
            await service.ChangeUserRoleAsync(user.Id, changeRole);

            // Assert
            user.Role.Should().Be(changeRole.Role);
        }

        [Fact]
        public async Task ChangeUserRoleAsync_ShouldThrow_ParameterException_WhenInvalidUserId()
        {
            // Arrange
            var user = _userFaker.Generate();
            var changeRole = _changeRoleFaker.Generate();

            var service = CreateService(out var unitOfWorkMock, out _, out var passwordHashingServiceMock, out _);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) => id == user.Id ? user : null);

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);

            // Act
            var act = async () => await service.ChangeUserRoleAsync(Guid.Empty, changeRole);

            // Assert
            await act.Should().ThrowAsync<ParameterException>();
        }

        [Theory]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", true)]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", false)]
        public async Task UserExists_ShouldReturnValidResult(string userIdAsString, string storedUserIdAsString, bool expected)
        {
            // Arrange
            var userId = Guid.Parse(userIdAsString);
            var storedUserId = Guid.Parse(storedUserIdAsString);

            var service = CreateService(out var unitOfWorkMock, out _, out _, out _);

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.IsExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(id == storedUserId));

            unitOfWorkMock.Setup(u => u.UserRepository).Returns(repository.Object);

            // Act
            var result = await service.UserExistsAsync(userId);

            // Assert
            result.Should().Be(expected);
        }

        private Func<User, UserDto> GetMapperFunctionUserToDto()
        {
            return new Func<User, UserDto>(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Role = u.Role,
                Notifications = u.Notifications,
                Transactions = u.Transactions
            });
        }

        private Func<IEnumerable<User>, IEnumerable<UserDto>> GetMapperFunctionListToList(Func<User, UserDto> mapUserFunc)
        {
            return new Func<IEnumerable<User>, IEnumerable<UserDto>>(c => c.Select(mapUserFunc));
        }

        private Func<PagedCollection<User>, PagedCollection<UserDto>> GetMapperFunctionPageToPage(Func<User, UserDto> mapUserFunc)
        {
            return new Func<PagedCollection<User>, PagedCollection<UserDto>>(c => new PagedCollection<UserDto>
            {
                CurrentPage = c.CurrentPage,
                PageSize = c.PageSize,
                TotalPages = c.TotalPages,
                Items = c.Items.Select(mapUserFunc)
            });
        }

        private UserService CreateService() => CreateService(out _, out _, out _, out _);

        private UserService CreateService(
            out Mock<IUnitOfWork> unitOfWorkMock,
            out Mock<IMapper> mapperMock,
            out Mock<IPasswordHashingService> passwordHashingServiceMock,
            out Mock<ICacheService> cacheServiceMock)
        {
            unitOfWorkMock = new();
            mapperMock = new();
            passwordHashingServiceMock = new();
            cacheServiceMock = new();

            return new(
                unitOfWorkMock.Object,
                mapperMock.Object,
                _updateUserValidator,
                _changePasswordValidator,
                _changeUserRoleValidator,
                _pageParametersValidator,
                passwordHashingServiceMock.Object,
                cacheServiceMock.Object);
        }
    }
}
