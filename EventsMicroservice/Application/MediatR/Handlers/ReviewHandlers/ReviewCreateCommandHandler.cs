using Application.MediatR.Commands.ReviewCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.MediatR.Handlers.ReviewHandlers
{
    public class ReviewCreateCommandHandler : BaseHandler<Review>, IRequestHandler<ReviewCreateCommand, Guid>
    {
        public ReviewCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IReviewValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<Guid> Handle(ReviewCreateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.Review);

            // TODO: Добавить проверку на то, является ли пользователь участником ивента

            return await _unitOfWork.ReviewRepository.CreateAsync(request.Review, cancellationToken).ConfigureAwait(false);
        }
    }
}
