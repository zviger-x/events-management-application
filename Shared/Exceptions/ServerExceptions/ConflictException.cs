namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// An error that occurs when there is a data conflict, such as during creation.
    /// </summary>
    public class ConflictException : ServerException
    {
        public ConflictException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public ConflictException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
