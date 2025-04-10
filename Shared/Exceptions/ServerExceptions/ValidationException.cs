namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Reflects an error at the data validation level
    /// </summary>
    public class ValidationException : ServerException
    {
        public ValidationException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public ValidationException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }

        public ValidationException(FluentValidation.Results.ValidationFailure validationFailure)
            : base(validationFailure.ErrorCode, validationFailure.ErrorMessage, validationFailure.PropertyName)
        {
        }

        public static IEnumerable<ValidationException> GetValidationExceptions(FluentValidation.ValidationException validationException)
        {
            return validationException.Errors.Select(e => new ValidationException(e));
        }
    }
}
