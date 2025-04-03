using Application.MediatR.Commands.ReviewCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.MediatR.Handlers.ReviewHandlers
{
    public class ReviewUpdateCommandHandler : BaseHandler<Review>, IRequestHandler<ReviewUpdateCommand>
    {
        public ReviewUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IReviewValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Handle(ReviewUpdateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.Review);

            await _unitOfWork.ReviewRepository.UpdateAsync(request.Review, cancellationToken).ConfigureAwait(false);
        }
    }
}
