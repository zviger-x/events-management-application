using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.EventCommentQueries
{
    public class EventCommentGetByIdQuery : IRequest<EventComment>
    {
        public Guid Id { get; set; }
    }
}
