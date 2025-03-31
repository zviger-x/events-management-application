namespace Application.Exceptions
{
    public class ServiceValidationException : Exception
    {
        public string ErrorCode { get; }
        public string PropertyName { get; }

        public ServiceValidationException(string errorCode, string errorMessage)
            : this(errorCode, errorMessage, null!)
        {
        }

        public ServiceValidationException(string errorCode, string errorMessage, string propertyName)
            : base(errorMessage)
        {
            ErrorCode = errorCode;
            PropertyName = propertyName;
        }

        public override string ToString()
        {
            return $"ErrorCode: {ErrorCode ?? "null"}, PropertyName: {PropertyName ?? "null"}, Message: {Message ?? "null"}";
        }
    }
}
