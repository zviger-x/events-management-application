using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Validation.Messages;
using Infrastructure.Validation.ErrorCodes;

namespace Infrastructure.Validation.Validators
{
    public class EventValidator : BaseValidator<Event>, IEventValidator
    {
        public EventValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            RuleFor(e => e.Name)
                .NotNull()
                    .WithMessage(EventValidationMessages.NameIsNull)
                    .WithErrorCode(EventValidationErrorCodes.NameIsNull)
                .NotEmpty()
                    .WithMessage(EventValidationMessages.NameIsEmpty)
                    .WithErrorCode(EventValidationErrorCodes.NameIsEmpty);

            RuleFor(e => e.Description)
                .NotNull()
                    .WithMessage(EventValidationMessages.DescriptionIsNull)
                    .WithErrorCode(EventValidationErrorCodes.DescriptionIsNull)
                .NotEmpty()
                    .WithMessage(EventValidationMessages.DescriptionIsEmpty)
                    .WithErrorCode(EventValidationErrorCodes.DescriptionIsEmpty);

            RuleFor(e => e.StartDate)
                .GreaterThan(DateTime.Now)
                    .WithMessage(EventValidationMessages.StartDateInvalid)
                    .WithErrorCode(EventValidationErrorCodes.StartDateInvalid);

            RuleFor(e => e.EndDate)
                .GreaterThan(e => e.StartDate)
                    .WithMessage(EventValidationMessages.EndDateInvalid)
                    .WithErrorCode(EventValidationErrorCodes.EndDateInvalid);

            RuleFor(e => e.Location)
                .NotNull()
                    .WithMessage(EventValidationMessages.LocationIsNull)
                    .WithErrorCode(EventValidationErrorCodes.LocationIsNull)
                .NotEmpty()
                    .WithMessage(EventValidationMessages.LocationIsEmpty)
                    .WithErrorCode(EventValidationErrorCodes.LocationIsEmpty);

            RuleFor(e => e.PurchaseDeadline)
                .LessThan(e => e.StartDate)
                    .WithMessage(EventValidationMessages.PurchaseDeadlineInvalid)
                    .WithErrorCode(EventValidationErrorCodes.PurchaseDeadlineInvalid);
        }
    }
}
