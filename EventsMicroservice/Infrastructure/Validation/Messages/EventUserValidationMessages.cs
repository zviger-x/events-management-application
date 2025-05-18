namespace Infrastructure.Validation.Messages
{
    internal static class EventUserValidationMessages
    {
        public const string EventIdIsNull = "Event ID cannot be null.";
        public const string EventIdIsEmpty = "Event ID cannot be empty.";

        public const string UserIdIsNull = "User ID cannot be null.";
        public const string UserIdIsEmpty = "User ID cannot be empty.";

        public const string SeatIdIsNull = "Seat ID cannot be null.";
        public const string SeatIdIsEmpty = "Seat ID cannot be empty.";

        public const string RegistrationTimeIsInvalid = "Registration time cannot be in the future.";

        public const string TokenIsNull = "Token cannot be null.";
        public const string TokenIsEmpty = "Token cannot be empty.";
    }
}
