namespace BusinessLogic.Exceptions
{
    /// <summary>
    /// Error that occurs when attempting to access without permissions
    /// </summary>
    public class ForbiddenAccessException : ServerException
    {
        public ForbiddenAccessException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public ForbiddenAccessException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
        {
        }
    }
}
