namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error due to invalid arguments.
    /// </summary>
    public class ArgumentException : ServerException
    {
        public ArgumentException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public ArgumentException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
