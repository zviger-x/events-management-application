namespace Infrastructure.Validation.ErrorCodes
{
    internal static class EventValidationErrorCodes
    {
        public const string NameIsNull = "nameIsNull";
        public const string NameIsEmpty = "nameIsEmpty";

        public const string DescriptionIsNull = "descriptionIsNull";
        public const string DescriptionIsEmpty = "descriptionIsEmpty";

        public const string StartDateInvalid = "startDateInvalid";
        public const string EndDateInvalid = "endDateInvalid";

        public const string LocationIsNull = "locationIsNull";
        public const string LocationIsEmpty = "locationIsEmpty";

        public const string PurchaseDeadlineInvalid = "purchaseDeadlineInvalid";

        public const string SeatConfigurationIdIsEmpty = "eventIdIsEmpty";
    }
}
