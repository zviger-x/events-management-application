namespace Infrastructure.Validation.Messages
{
    internal static class ReviewValidationMessages
    {
        public const string EventIdIsNull = "Event ID cannot be null.";
        public const string EventIdIsEmpty = "Event ID cannot be empty.";

        public const string UserIdIsNull = "User ID cannot be null.";
        public const string UserIdIsEmpty = "User ID cannot be empty.";

        public const string TextIsNull = "Text cannot be null.";
        public const string TextIsEmpty = "Text cannot be empty.";
        public const string TextIsTooLong = "Text cannot be longer than 1000 characters.";

        public const string CreationTimeIsInvalid = "Creation time must be a valid date.";
    }
}
