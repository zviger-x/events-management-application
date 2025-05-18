using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.EventQueries
{
    public class EventGetAllQuery : IRequest<IEnumerable<Event>>
    {

    }
}
