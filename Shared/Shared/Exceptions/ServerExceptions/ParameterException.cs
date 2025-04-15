namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error due to invalid parameters.
    /// </summary>
    public class ParameterException : ServerException
    {
        public ParameterException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public ParameterException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
        {
        }

        public ParameterException(ArgumentException argumentException)
            : base(argumentException.Message, argumentException.ParamName, argumentException)
        {
        }
    }
}
