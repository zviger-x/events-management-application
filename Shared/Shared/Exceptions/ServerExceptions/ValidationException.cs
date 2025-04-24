namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Reflects an error at the data validation level
    /// </summary>
    public class ValidationException : ServerException
    {
        public ValidationException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public ValidationException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
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
