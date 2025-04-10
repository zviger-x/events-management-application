namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error that occurs when attempting to access without permissions
    /// </summary>
    public class ForbiddenAccessException : ServerException
    {
        public ForbiddenAccessException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public ForbiddenAccessException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
