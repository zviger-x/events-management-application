using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.ReviewCommands
{
    public class ReviewUpdateCommand : IRequest
    {
        public Review Review { get; set; }
    }
}
