namespace BusinessLogic.Validation.Messages
{
    internal static class UserTransactionValidationMessages
    {
        public const string AmountIsNull = "Amount cannot be null.";
        public const string AmountIsLessThanZero = "Amount must be greater than or equal to zero.";

        public const string DateIsNull = "Date and time cannot be null.";

        public const string UserIdIsNull = "User ID cannot be null.";
        public const string UserIdIsEmpty = "User ID cannot be empty.";

        public const string EventIdIsNull = "Event ID cannot be null.";
        public const string EventIdIsEmpty = "Event ID cannot be empty.";

        public const string SeatRowIsNull = "Seat row cannot be null.";
        public const string SeatRowIsLessThanOrEqualToZero = "Seat row must be greater than zero.";

        public const string SeatNumberIsNull = "Seat number cannot be null.";
        public const string SeatNumberIsLessThanOrEqualToZero = "Seat number must be greater than zero.";
    }
}
