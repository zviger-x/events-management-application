namespace BusinessLogic.Exceptions
{
    /// <summary>
    /// Error that occurs when attempting to access without authorization
    /// </summary>
    public class UnauthorizedException : ServerException
    {
        public UnauthorizedException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public UnauthorizedException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
        {
        }
    }
}
