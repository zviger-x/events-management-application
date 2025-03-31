using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.EventQueries
{
    public class EventGetByIdQuery : IRequest<Event>
    {
        public Guid Id { get; set; }
    }
}
