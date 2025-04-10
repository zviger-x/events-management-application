using Shared.Extensions;

namespace Shared.Exceptions.ServerExceptions
{
    public class ServerException : Exception
    {
        public string ErrorCode { get; }
        public string PropertyName { get; }

        public ServerException(string errorCode, string errorMessage, string propertyName = null)
            : base(errorMessage)
        {
            ErrorCode = errorCode ?? GenerateErrorCode();
            PropertyName = propertyName;
        }

        public ServerException(string errorMessage, string propertyName = null)
            : this(null, errorMessage, propertyName)
        {
        }

        public override string ToString()
        {
            return $"Exception body: ErrorCode: {ErrorCode ?? "null"}, PropertyName: {PropertyName ?? "null"}, Message: {Message ?? "null"}{Environment.NewLine}" +
                   $"Default body:   {base.ToString()}";
        }

        private string GenerateErrorCode()
        {
            return GetType().Name.ToLowerFirstChar();
        }
    }
}
