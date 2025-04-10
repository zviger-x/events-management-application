namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error that occurs when attempting to access without authorization
    /// </summary>
    public class UnauthorizedException : ServerException
    {
        public UnauthorizedException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public UnauthorizedException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
