namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error due to invalid parameters.
    /// </summary>
    public class ParameterException : ServerException
    {
        public ParameterException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public ParameterException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
