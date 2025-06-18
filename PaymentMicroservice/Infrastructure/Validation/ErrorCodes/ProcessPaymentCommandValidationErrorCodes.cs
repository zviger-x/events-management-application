namespace Infrastructure.Validation.ErrorCodes
{
    internal static class ProcessPaymentCommandValidationErrorCodes
    {
        public const string EventNameIsNull = "eventNameIsNull";
        public const string EventNameIsEmpty = "eventNameIsEmpty";

        public const string TokenNameIsNull = "tokenIsNull";
        public const string TokenNameIsEmpty = "tokenIsEmpty";

        public const string AmountIsInvalid = "amountIsInvalid";

        public const string SeatRowIsInvalid = "seatRowIsInvalid";

        public const string SeatNumberIsInvalid = "seatNumberIsInvalid";
    }
}
