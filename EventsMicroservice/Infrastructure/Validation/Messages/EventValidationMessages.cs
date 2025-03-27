namespace Infrastructure.Validation.Messages
{
    internal static class EventValidationMessages
    {
        public const string NameIsNull = "Event name cannot be null.";
        public const string NameIsEmpty = "Event name cannot be empty.";

        public const string DescriptionIsNull = "Event description cannot be null.";
        public const string DescriptionIsEmpty = "Event description cannot be empty.";

        public const string StartDateInvalid = "Event start date must be in the future.";
        public const string EndDateInvalid = "Event end date must be later than the start date.";

        public const string LocationIsNull = "Event location cannot be null.";
        public const string LocationIsEmpty = "Event location cannot be empty.";

        public const string PurchaseDeadlineInvalid = "Purchase deadline must be before event start date.";
    }
}
