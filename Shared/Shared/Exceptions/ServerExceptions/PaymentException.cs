namespace Shared.Exceptions.ServerExceptions
{
    public class PaymentException : ServerException
    {
        public PaymentException(string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorMessage, propertyName, innerException)
        {
        }

        public PaymentException(string errorCode, string errorMessage, string propertyName = null, Exception innerException = null)
            : base(errorCode, errorMessage, propertyName, innerException)
        {
        }
    }
}
