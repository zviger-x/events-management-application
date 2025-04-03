using Application.MediatR.Commands.ReviewCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.ReviewHandlers
{
    public class ReviewDeleteCommandHandler : BaseHandler, IRequestHandler<ReviewDeleteCommand>
    {
        public ReviewDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task Handle(ReviewDeleteCommand request, CancellationToken cancellationToken)
        {
            var review = new Review() { Id = request.Id };

            await _unitOfWork.ReviewRepository.DeleteAsync(review, cancellationToken).ConfigureAwait(false);
        }
    }
}
