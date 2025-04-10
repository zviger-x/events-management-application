namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error that occurs when attempting to access without authorization
    /// </summary>
    public class UnauthorizedAccessException : ServerException
    {
        public UnauthorizedAccessException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public UnauthorizedAccessException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
