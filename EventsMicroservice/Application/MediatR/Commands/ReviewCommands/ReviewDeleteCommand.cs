using MediatR;

namespace Application.MediatR.Commands.ReviewCommands
{
    public class ReviewDeleteCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
