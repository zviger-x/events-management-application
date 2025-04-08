using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.EventUserQueries
{
#warning TODO: Добавить модель дто и валидировать её. ВЕЗДЕ.
    public class EventUserGetPagedQuery : IRequest<PagedCollection<EventUser>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
