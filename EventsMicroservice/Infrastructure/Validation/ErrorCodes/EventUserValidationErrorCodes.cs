namespace Infrastructure.Validation.ErrorCodes
{
    internal static class EventUserValidationErrorCodes
    {
        public const string EventIdIsNull = "eventIdIsNull";
        public const string EventIdIsEmpty = "eventIdIsEmpty";

        public const string UserIdIsNull = "userIdIsNull";
        public const string UserIdIsEmpty = "userIdIsEmpty";

        public const string SeatIdIsNull = "seatIdIsNull";
        public const string SeatIdIsEmpty = "seatIdIsEmpty";

        public const string RegistrationTimeIsInvalid = "registrationTimeIsInvalid";
    }
}
