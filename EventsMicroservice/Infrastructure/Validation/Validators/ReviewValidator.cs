using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class ReviewValidator : BaseValidator<Review>, IReviewValidator
    {
        public ReviewValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            #warning TODO: Добавить проверку на наличие ивента
            RuleFor(r => r.EventId)
                .NotNull()
                    .WithMessage(ReviewValidationMessages.EventIdIsNull)
                    .WithErrorCode(ReviewValidationErrorCodes.EventIdIsNull)
                .NotEmpty()
                    .WithMessage(ReviewValidationMessages.EventIdIsEmpty)
                    .WithErrorCode(ReviewValidationErrorCodes.EventIdIsEmpty);

            #warning TODO: Добавить проверку на наличие пользователя (gRPC)
            RuleFor(r => r.UserId)
                .NotNull()
                    .WithMessage(ReviewValidationMessages.UserIdIsNull)
                    .WithErrorCode(ReviewValidationErrorCodes.UserIdIsNull)
                .NotEmpty()
                    .WithMessage(ReviewValidationMessages.UserIdIsEmpty)
                    .WithErrorCode(ReviewValidationErrorCodes.UserIdIsEmpty);

            RuleFor(r => r.Text)
                .NotNull()
                    .WithMessage(ReviewValidationMessages.TextIsNull)
                    .WithErrorCode(ReviewValidationErrorCodes.TextIsNull)
                .NotEmpty()
                    .WithMessage(ReviewValidationMessages.TextIsEmpty)
                    .WithErrorCode(ReviewValidationErrorCodes.TextIsEmpty)
                .MaximumLength(1000)
                    .WithMessage(ReviewValidationMessages.TextIsTooLong)
                    .WithErrorCode(ReviewValidationErrorCodes.TextIsTooLong);

            RuleFor(r => r.CreationTime)
                .GreaterThan(DateTime.MinValue)
                    .WithMessage(ReviewValidationMessages.CreationTimeIsInvalid)
                    .WithErrorCode(ReviewValidationErrorCodes.CreationTimeIsInvalid);
        }
    }
}
