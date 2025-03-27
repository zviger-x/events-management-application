using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.UseCases.EventUseCases
{
    public class EventGetPagedUseCase : BaseUseCase<Event>, IGetPagedUseCaseAsync<Event>
    {
        public EventGetPagedUseCase(IUnitOfWork unitOfWork, IMapper mapper, IEventValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<PagedCollection<Event>> Execute(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _unitOfWork.EventRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
