namespace Infrastructure.Validation.ErrorCodes
{
    internal static class CreateUserTransactionDtoValidationErrorCodes
    {
        public const string EventNameIsNull = "eventNameIsNull";
        public const string EventNameIsEmpty = "eventNameIsEmpty";

        public const string SeatRowIsInvalid = "seatRowIsInvalid";

        public const string SeatNumberIsInvalid = "seatNumberIsInvalid";

        public const string AmountIsInvalid = "amountIsInvalid";
    }
}
