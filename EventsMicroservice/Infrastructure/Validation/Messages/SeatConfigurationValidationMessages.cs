namespace Infrastructure.Validation.Messages
{
    internal static class SeatConfigurationValidationMessages
    {
        public const string NameIsNull = "Name cannot be null.";
        public const string NameIsEmpty = "Name cannot be empty.";

        public const string DefaultPriceIsInvalid = "Default price must be greater than 0.";

        public const string RowsIsNull = "Rows cannot be null.";
        public const string RowsIsEmpty = "Rows cannot be empty.";
        public const string RowsAreInvalid = "Each row value must be greater than 0.";
    }
}
