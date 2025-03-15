namespace BusinessLogic.Validation.ErrorCodes
{
    internal static class UserTransactionValidationErrorCodes
    {
        public const string AmountIsNull = "amountIsNull";
        public const string AmountIsLessThanZero = "amountIsLessThanZero";

        public const string DateIsNull = "dateIsNull";

        public const string UserIdIsNull = "userIdIsNull";
        public const string UserIdIsEmpty = "userIdIsEmpty";

        public const string EventIdIsNull = "eventIdIsNull";
        public const string EventIdIsEmpty = "eventIdIsEmpty";

        public const string SeatRowIsNull = "seatRowIsNull";
        public const string SeatRowIsInvalid = "seatRowIsInvalid";

        public const string SeatNumberIsNull = "seatNumberIsNull";
        public const string SeatNumberIsInvalid = "seatNumberIsInvalid";
    }
}
