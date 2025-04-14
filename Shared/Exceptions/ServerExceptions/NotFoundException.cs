namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error that is thrown if the object was not found.
    /// </summary>
    public class NotFoundException : ServerException
    {
        public NotFoundException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public NotFoundException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
        {
        }
    }
}
