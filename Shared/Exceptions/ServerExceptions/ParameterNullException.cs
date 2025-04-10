namespace Shared.Exceptions.ServerExceptions
{
    /// <summary>
    /// Error due to null arguments.
    /// </summary>
    public class ParameterNullException : ParameterException
    {
        public ParameterNullException(string propertyName)
            : base("Argument cannot be null.", propertyName)
        {
        }

        public ParameterNullException(string errorMessage, string propertyName = null)
            : base(errorMessage, propertyName)
        {
        }

        public ParameterNullException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorCode, errorMessage, propertyName)
        {
        }
    }
}
