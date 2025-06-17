namespace BusinessLogic.Exceptions
{
    /// <summary>
    /// An error that occurs when there is a data conflict, such as during creation.
    /// </summary>
    public class ConflictException : ServerException
    {
        public ConflictException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public ConflictException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
        {
        }
    }
}
