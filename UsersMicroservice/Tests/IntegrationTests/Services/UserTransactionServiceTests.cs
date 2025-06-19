using AutoMapper;
using Bogus;
using BusinessLogic.Contracts;
using BusinessLogic.Services;
using BusinessLogic.Validation.Validators;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork;
using DataAccess.UnitOfWork.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common;
using Shared.Enums;
using Shared.Exceptions.ServerExceptions;
using Shared.Validation.Validators;
using System.Text;
using Xunit;

namespace Tests.IntegrationTests.Services
{
    public class UserTransactionServiceTests
    {
        private readonly IValidator<CreateUserTransactionDto> _createUserTransactionDtoValidator;
        private readonly IValidator<UpdateUserTransactionDto> _updateUserTransactionDtoValidator;
        private readonly IValidator<PageParameters> _pageParametersValidator;

        private readonly Faker _faker = new();
        private readonly Faker<User> _userFaker;
        private readonly Faker<UserTransaction> _transactionFaker;
        private readonly Faker<CreateUserTransactionDto> _createTransactionDtoFaker;
        private readonly Faker<UpdateUserTransactionDto> _updateTransactionDtoFaker;

        public UserTransactionServiceTests()
        {
            _createUserTransactionDtoValidator = new CreateUserTransactionDtoValidator();
            _updateUserTransactionDtoValidator = new UpdateUserTransactionDtoValidator();
            _pageParametersValidator = new PageParametersValidator();

            _userFaker = new Faker<User>()
                .RuleFor(r => r.Id, _ => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FirstName())
                .RuleFor(r => r.Surname, f => f.Name.LastName())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.PasswordHash, f => Encoding.UTF8.GetString(f.Random.Bytes(64)))
                .RuleFor(r => r.Role, f => f.PickRandom<UserRoles>());

            _transactionFaker = new Faker<UserTransaction>()
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.EventId, _ => Guid.NewGuid())
                .RuleFor(x => x.EventName, f => f.Random.Word())
                .RuleFor(x => x.SeatRow, f => f.Random.Int(1, 30))
                .RuleFor(x => x.SeatNumber, f => f.Random.Int(1, 100))
                .RuleFor(x => x.Amount, f => f.Random.Float(1, 10));

            _createTransactionDtoFaker = new Faker<CreateUserTransactionDto>()
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.EventId, _ => Guid.NewGuid())
                .RuleFor(x => x.EventName, f => f.Random.Word())
                .RuleFor(x => x.SeatRow, f => f.Random.Int(1, 30))
                .RuleFor(x => x.SeatNumber, f => f.Random.Int(1, 100))
                .RuleFor(x => x.Amount, f => f.Random.Float(1, 10));

            _updateTransactionDtoFaker = new Faker<UpdateUserTransactionDto>()
                .RuleFor(x => x.SeatRow, f => f.Random.Int(1, 30))
                .RuleFor(x => x.SeatNumber, f => f.Random.Int(1, 100))
                .RuleFor(x => x.Amount, f => f.Random.Float(1, 10));
        }

        [Fact]
        public async Task CreateUserTransaction_ShouldAppearInUserTransactions()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();

            var serviceProvider = CreateProvider(dbContext);

            var unitOfWork = new UnitOfWork(dbContext, serviceProvider);
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            var user = _userFaker.Generate();
            await userRepository.CreateAsync(user);

            var transactionDto = _createTransactionDtoFaker.Generate();
            transactionDto.UserId = user.Id;

            var transactionService = CreateService(unitOfWork, mapper);

            // Act
            var transactionId = await transactionService.CreateAsync(transactionDto);
            var updatedUser = await userRepository.GetByIdAsync(user.Id);

            // Assert
            updatedUser.Should().NotBeNull();
            updatedUser.Transactions.Should().ContainSingle(t => t.Id == transactionId);
        }

        [Fact]
        public async Task CreateUserTransaction_ShouldThrow_ValidationException_WhenInvalidUserId()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();

            var serviceProvider = CreateProvider(dbContext);

            var unitOfWork = new UnitOfWork(dbContext, serviceProvider);
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            var user = _userFaker.Generate();
            await userRepository.CreateAsync(user);

            var transactionDto = _createTransactionDtoFaker.Generate();

            var transactionService = CreateService(unitOfWork, mapper);

            // Act
            var act = async () => await transactionService.CreateAsync(transactionDto);

            // Assert
            await act.Should().ThrowAsync<Shared.Exceptions.ServerExceptions.ValidationException>();
        }

        [Fact]
        public async Task UpdateUserTransaction_ShouldUpdateTransaction()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();

            var serviceProvider = CreateProvider(dbContext);

            var unitOfWork = new UnitOfWork(dbContext, serviceProvider);
            var transactionRepository = serviceProvider.GetRequiredService<IUserTransactionRepository>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            var user = _userFaker.Generate();
            await userRepository.CreateAsync(user);

            var originalTransaction = _transactionFaker.Generate();
            originalTransaction.UserId = user.Id;

            await transactionRepository.CreateAsync(originalTransaction);

            var updateDto = _updateTransactionDtoFaker.Generate();

            var transactionService = CreateService(unitOfWork, mapper);

            // Act
            await transactionService.UpdateAsync(originalTransaction.Id, updateDto);

            var updatedTransaction = await transactionRepository.GetByIdAsync(originalTransaction.Id);

            // Assert
            updatedTransaction.Should().NotBeNull();
            updatedTransaction.SeatRow.Should().Be(updateDto.SeatRow);
            updatedTransaction.SeatNumber.Should().Be(updateDto.SeatNumber);
            updatedTransaction.Amount.Should().Be(updateDto.Amount);
        }

        [Fact]
        public async Task UpdateUserTransaction_ShouldThrow_NotFoundException_WhenTransactionDoesNotExists()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();

            var serviceProvider = CreateProvider(dbContext);

            var unitOfWork = new UnitOfWork(dbContext, serviceProvider);
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            var user = _userFaker.Generate();
            await userRepository.CreateAsync(user);

            var updateDto = _updateTransactionDtoFaker.Generate();

            var transactionService = CreateService(unitOfWork, mapper);

            // Act
            var act = async () => await transactionService.UpdateAsync(Guid.Empty, updateDto);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteUserTransaction_ShouldRemoveTransactionFromDatabase()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();

            var serviceProvider = CreateProvider(dbContext);

            var unitOfWork = new UnitOfWork(dbContext, serviceProvider);
            var transactionRepository = serviceProvider.GetRequiredService<IUserTransactionRepository>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            var user = _userFaker.Generate();
            await userRepository.CreateAsync(user);

            var transaction = _transactionFaker.Generate();
            transaction.UserId = user.Id;

            await transactionRepository.CreateAsync(transaction);

            var transactionService = CreateService(unitOfWork, mapper);

            // Act
            await transactionService.DeleteAsync(transaction.Id);

            var receivedUser = await userRepository.GetByIdAsync(user.Id);
            var deletedTransaction = await transactionRepository.GetByIdAsync(transaction.Id);

            // Assert
            receivedUser.Should().NotBeNull();
            receivedUser.Transactions.Should().NotContain(t => t.Id == deletedTransaction.Id);
            deletedTransaction.Should().BeNull();
        }

        private UserDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new UserDbContext(options);

            context.Database.EnsureCreated();

            return context;
        }

        private UserTransactionService CreateService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            return new UserTransactionService(
                unitOfWork,
                mapper,
                _createUserTransactionDtoValidator,
                _updateUserTransactionDtoValidator,
                _pageParametersValidator);
        }

        private IServiceProvider CreateProvider(UserDbContext dbContext)
        {
            return new ServiceCollection()
                .AddScoped(_ => dbContext)
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserTransactionRepository, UserTransactionRepository>()
                .AddAutoMapper(typeof(UserTransactionService).Assembly)
                .BuildServiceProvider();
        }
    }
}
