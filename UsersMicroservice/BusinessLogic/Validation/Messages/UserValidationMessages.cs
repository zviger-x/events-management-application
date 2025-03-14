namespace BusinessLogic.Validation.Messages
{
    internal static class UserValidationMessages
    {
        public const string NameNotNull = "User name cannot be null.";
        public const string NameNotEmpty = "User name cannot be empty.";

        public const string SurnameNotNull = "User surname cannot be null.";
        public const string SurnameNotEmpty = "User surname cannot be empty.";

        public const string EmailNotNull = "Email cannot be null.";
        public const string EmailNotEmpty = "Email cannot be empty.";
        public const string EmailInvalid = "Email format is invalid.";
        public const string EmailMustBeUnique = "Email must be unique.";

        public const string InvalidEmailOrPassword = "Invalid email or password";
    }

    internal static class UserNotificationValidationMessages
    {
        public const string MessageNotNull = "Message cannot be null.";
        public const string MessageNotEmpty = "Message cannot be empty.";

        public const string DateTimeNotNull = "Date and time cannot be null.";

        public const string UserIdNotNull = "User ID cannot be null.";
        public const string UserIdNotEmpty = "User ID cannot be empty.";
    }

    internal static class UserTransactionValidationMessages
    {
        public const string AmountNotNull = "Amount cannot be null.";
        public const string AmountLessThanZero = "Amount must be greater than or equals to zero.";

        public const string DateNotNull = "Date and time cannot be null.";

        public const string UserIdNotNull = "User ID cannot be null.";
        public const string UserIdNotEmpty = "User ID cannot be empty.";

        public const string EventIdNotNull = "Event ID cannot be null.";
        public const string EventIdNotEmpty = "Event ID cannot be empty.";

        public const string SeatRowNotNull = "Seat row cannot be null.";
        public const string SeatRowLessThanOrEqualsToZero = "Seat row must be greater than zero.";

        public const string SeatNumberNotNull = "Seat number cannot be null.";
        public const string SeatNumberLessThanOrEqualsToZero = "Seat number must be greater than zero.";
    }
}
