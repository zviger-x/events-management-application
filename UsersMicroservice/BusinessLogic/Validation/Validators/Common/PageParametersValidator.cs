using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using DataAccess.Common;
using FluentValidation;

namespace BusinessLogic.Validation.Validators.Common
{
    public class PageParametersValidator : AbstractValidator<PageParameters>
    {
        private const int MaxPageSize = 100;

        public PageParametersValidator()
        {
            RuleFor(p => p.PageNumber)
                .GreaterThan(0)
                    .WithMessage(PageParametersValidationMessages.PageNumberIsInvalid)
                    .WithErrorCode(PageParametersValidationErrorCodes.PageNumberIsInvalid);

            RuleFor(p => p.PageSize)
                .GreaterThan(0)
                    .WithMessage(PageParametersValidationMessages.PageSizeIsTooSmall)
                    .WithErrorCode(PageParametersValidationErrorCodes.PageSizeIsTooSmall)
                .LessThanOrEqualTo(MaxPageSize)
                    .WithMessage(string.Format(PageParametersValidationMessages.PageSizeIsTooLarge, MaxPageSize))
                    .WithErrorCode(PageParametersValidationErrorCodes.PageSizeIsTooLarge);
        }
    }
}
