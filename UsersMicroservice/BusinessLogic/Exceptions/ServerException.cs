namespace BusinessLogic.Exceptions
{
    public class ServerException : Exception
    {
        public string ErrorCode { get; }
        public string PropertyName { get; }

        public ServerException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, innerException)
        {
            ErrorCode = errorCode ?? GenerateErrorCode();
            PropertyName = propertyName;
        }

        public ServerException(string errorMessage, string propertyName = null, Exception innerException = null)
            : this(null, errorMessage, propertyName, innerException)
        {
        }

        public override string ToString()
        {
            return $"Exception body: ErrorCode: {ErrorCode ?? "null"}, PropertyName: {PropertyName ?? "null"}, Message: {Message ?? "null"}{Environment.NewLine}" +
                   $"Default body:   {base.ToString()}";
        }

        private string GenerateErrorCode()
        {
            var name = GetType().Name;

            if (string.IsNullOrEmpty(name))
                return name;

            var chars = name.ToCharArray();
            chars[0] = char.ToLowerInvariant(chars[0]);

            return new string(chars);
        }
    }
}
