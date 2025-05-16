namespace Infrastructure.Validation.Messages
{
    internal static class SeatValidationMessages
    {
        public const string EventIdIsNull = "Event ID cannot be null.";
        public const string EventIdIsEmpty = "Event ID cannot be empty.";

        public const string RowIsInvalid = "Row number must be a positive integer.";
        public const string NumberIsInvalid = "Seat number must be a positive integer.";
        public const string PriceIsInvalid = "Seat price must be greater than zero.";

        public const string IsBoughtIsInvalid = "IsBought flag must be a boolean value.";
    }
}
