using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.ReviewCommands
{
    public class ReviewCreateCommand : IRequest<Guid>
    {
        public Review Review { get; set; }
    }
}
