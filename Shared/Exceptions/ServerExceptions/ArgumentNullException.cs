namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error due to null arguments.
    /// </summary>
    public class ArgumentNullException : ArgumentException
    {
        public ArgumentNullException(string propertyName)
            : base("Argument cannot be null.", propertyName)
        {
        }

        public ArgumentNullException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public ArgumentNullException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
