namespace Infrastructure.Validation.Messages
{
    internal static class CreateUserTransactionDtoValidationMessages
    {
        public const string EventNameIsNull = "Event name cannot be null.";
        public const string EventNameIsEmpty = "Event name cannot be empty.";

        public const string SeatRowIsInvalid = "Seat row must be greater than zero.";

        public const string SeatNumberIsInvalid = "Seat number must be greater than zero.";

        public const string AmountIsInvalid = "Amount must be greater than zero.";
    }
}
