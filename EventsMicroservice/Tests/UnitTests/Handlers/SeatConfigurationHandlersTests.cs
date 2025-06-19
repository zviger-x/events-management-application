using Application.Contracts;
using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.MediatR.Handlers.SeatConfigurationHandlers;
using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.Repositories.Interfaces;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Shared.Common;
using Shared.Exceptions.ServerExceptions;
using Xunit;

namespace Tests.UnitTests.Handlers
{
    public class SeatConfigurationHandlersTests
    {
        private readonly Faker _faker = new();
        private readonly Faker<SeatConfiguration> _configurationFaker;

        public SeatConfigurationHandlersTests()
        {
            _configurationFaker = new Faker<SeatConfiguration>()
                .RuleFor(r => r.Id, _ => Guid.NewGuid())
                .RuleFor(r => r.DefaultPrice, f => f.Random.Float(.1f) * 10f)
                .RuleFor(r => r.Name, f => f.Random.Word())
                .RuleFor(r => r.Rows, f => f.Random.Digits(10, 1, 9).ToList());
        }

        [Fact]
        public async Task HandleCreateCommand_ShouldCreateConfiguration()
        {
            // Arrange
            var request = new SeatConfigurationCreateCommand
            {
                SeatConfiguration = new CreateSeatConfigurationDto
                {
                    DefaultPrice = _faker.Random.Float(.1f) * 10f,
                    Name = _faker.Random.Word(),
                    Rows = _faker.Random.Digits(10, 1, 9).ToList()
                }
            };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.CreateAsync(It.IsAny<SeatConfiguration>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Guid.NewGuid()))
                .Verifiable();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var mapperFunc = GetMappingFuncCreateDtoToEntity();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<SeatConfiguration>(It.IsAny<CreateSeatConfigurationDto>())).Returns(mapperFunc);

            var handler = new Mock<SeatConfigurationCreateCommandHandler>(unitOfWorkMock.Object, mapperMock.Object, null);

            // Act
            var id = await handler.Object.Handle(request, default);

            // Assert
            id.Should().NotBe(Guid.Empty);
            repositoryMock.Verify(u => u.CreateAsync(It.IsAny<SeatConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleUpdateCommand_ShouldUpdateConfiguration()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new SeatConfigurationUpdateCommand
            {
                SeatConfigurationId = id,
                SeatConfiguration = new UpdateSeatConfigurationDto
                {
                    DefaultPrice = _faker.Random.Float(.1f) * 10f,
                    Name = _faker.Random.Word(),
                    Rows = _faker.Random.Digits(10, 1, 9).ToList()
                }
            };

            var storedConfiguration = new SeatConfiguration
            {
                Id = id,
                DefaultPrice = _faker.Random.Float(.1f) * 10f,
                Name = _faker.Random.Word(),
                Rows = _faker.Random.Digits(10, 1, 9).ToList()
            };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SeatConfiguration>(), It.IsAny<CancellationToken>())).Verifiable();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(id == storedConfiguration.Id ? storedConfiguration : null));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var mapperFunc = GetMappingFuncUpdateDtoToEntity();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<SeatConfiguration>(It.IsAny<UpdateSeatConfigurationDto>())).Returns(mapperFunc);

            var handler = new Mock<SeatConfigurationUpdateCommandHandler>(unitOfWorkMock.Object, mapperMock.Object, null);

            // Act
            await handler.Object.Handle(request, default);

            // Assert
            unitOfWorkMock.Verify(u => u.SeatConfigurationRepository.UpdateAsync(It.IsAny<SeatConfiguration>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleUpdateCommand_ShouldThrow_NotFoundException_WhenConfigurationDoesNotExists()
        {
            // Arrange
            var request = new SeatConfigurationUpdateCommand
            {
                SeatConfigurationId = Guid.Empty,
                SeatConfiguration = new()
            };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(default(SeatConfiguration)));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var handler = new Mock<SeatConfigurationUpdateCommandHandler>(unitOfWorkMock.Object, null, null);

            // Act
            var act = async () => await handler.Object.Handle(request, default);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HandleDeleteCommand_ShouldDeleteConfiguration()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new SeatConfigurationDeleteCommand { Id = id };
            var storedConfiguration = new SeatConfiguration { Id = id };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<SeatConfiguration>(), It.IsAny<CancellationToken>())).Verifiable();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(id == storedConfiguration.Id ? storedConfiguration : null));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var handler = new Mock<SeatConfigurationDeleteCommandHandler>(unitOfWorkMock.Object, null, null);

            // Act
            await handler.Object.Handle(request, default);

            // Assert
            repositoryMock.Verify(u => u.DeleteAsync(It.IsAny<SeatConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleDeleteCommand_ShouldThrow_NotFoundException_WhenConfigurationDoesNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new SeatConfigurationDeleteCommand { Id = id };
            var storedConfiguration = new SeatConfiguration { Id = Guid.NewGuid() };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(id == storedConfiguration.Id ? storedConfiguration : null));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var handler = new Mock<SeatConfigurationDeleteCommandHandler>(unitOfWorkMock.Object, null, null);

            // Act
            var act = async () => await handler.Object.Handle(request, default);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task HandleGetByIdQuery_ShouldReturnConfiguration()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new SeatConfigurationGetByIdQuery { Id = id };
            var storedConfiguration = new SeatConfiguration { Id = id };
            var expectedConfiguration = new SeatConfiguration { Id = id };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(id == storedConfiguration.Id ? storedConfiguration : null));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var handler = new Mock<SeatConfigurationGetByIdQueryHandler>(unitOfWorkMock.Object, null, null);

            // Act
            var receivedConfiguration = await handler.Object.Handle(request, default);

            // Assert
            receivedConfiguration.Should().BeEquivalentTo(expectedConfiguration);
        }

        [Fact]
        public async Task HandleGetByIdQuery_ShouldThrow_NotFoundException_WhenConfigurationDoesNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new SeatConfigurationGetByIdQuery { Id = id };
            var storedConfiguration = new SeatConfiguration { Id = Guid.NewGuid() };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) => Task.FromResult(id == storedConfiguration.Id ? storedConfiguration : null));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var handler = new Mock<SeatConfigurationGetByIdQueryHandler>(unitOfWorkMock.Object, null, null);

            // Act
            var act = async () => await handler.Object.Handle(request, default);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(1, 10, 30)]
        [InlineData(2, 10, 30)]
        [InlineData(3, 10, 30)]
        [InlineData(1, 5, 12)]
        [InlineData(3, 5, 12)]
        [InlineData(1, 10, 0)]
        public async Task HandleGetPagedQuery_ShouldReturnPagedArrayOfSeatConfigurations(int pageNumber, int pageSize, int totalEntities)
        {
            // Arrange
            var entities = _configurationFaker.Generate(totalEntities);
            var request = new SeatConfigurationGetPagedQuery
            {
                PageParameters = new()
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber
                }
            };

            var repositoryMock = new Mock<ISeatConfigurationRepository>();
            repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns((int pNum, int pSize, CancellationToken _) =>
                {
                    var totalCount = entities.Count;
                    var totalPages = (int)Math.Ceiling(totalCount / (float)pSize);
                    var page = entities.Skip((pNum - 1) * pSize).Take(pSize).ToList();

                    var pagedCollection = new PagedCollection<SeatConfiguration>
                    {
                        Items = page,
                        TotalPages = totalPages,
                        CurrentPage = pageNumber,
                        PageSize = pageSize
                    };

                    return Task.FromResult(pagedCollection);
                });

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SeatConfigurationRepository).Returns(repositoryMock.Object);

            var handler = new Mock<SeatConfigurationGetPagedQueryHandler>(unitOfWorkMock.Object, null, null);

            // Act
            var result = await handler.Object.Handle(request, default);

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

        private Func<CreateSeatConfigurationDto, SeatConfiguration> GetMappingFuncCreateDtoToEntity()
        {
            return new Func<CreateSeatConfigurationDto, SeatConfiguration>(dto => new SeatConfiguration
            {
                Name = dto.Name,
                DefaultPrice = dto.DefaultPrice,
                Rows = dto.Rows
            });
        }

        private Func<UpdateSeatConfigurationDto, SeatConfiguration> GetMappingFuncUpdateDtoToEntity()
        {
            return new Func<UpdateSeatConfigurationDto, SeatConfiguration>(dto => new SeatConfiguration
            {
                Name = dto.Name,
                DefaultPrice = dto.DefaultPrice,
                Rows = dto.Rows
            });
        }
    }
}
