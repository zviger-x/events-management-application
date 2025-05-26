namespace BusinessLogic.Exceptions
{
    /// <summary>
    /// Error due to null arguments.
    /// </summary>
    public class ParameterNullException : ParameterException
    {
        public ParameterNullException(string propertyName, Exception innerException)
            : base("Argument cannot be null.", propertyName, innerException)
        {
        }

        public ParameterNullException(string propertyName)
            : this(propertyName, null)
        {
        }

        public ParameterNullException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public ParameterNullException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
        {
        }

        public ParameterNullException(ArgumentNullException argumentNullException)
            : base(argumentNullException.Message, argumentNullException.ParamName, argumentNullException)
        {
        }
    }
}
