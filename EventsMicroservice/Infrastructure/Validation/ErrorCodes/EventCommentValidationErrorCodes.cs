namespace Infrastructure.Validation.ErrorCodes
{
    internal static class EventCommentValidationErrorCodes
    {
        public const string EventIdIsNull = "eventIdIsNull";
        public const string EventIdIsEmpty = "eventIdIsEmpty";

        public const string UserIdIsNull = "userIdIsNull";
        public const string UserIdIsEmpty = "userIdIsEmpty";

        public const string TextIsNull = "textIsNull";
        public const string TextIsEmpty = "textIsEmpty";
        public const string TextIsTooLong = "textIsTooLong";

        public const string CreationTimeIsInvalid = "creationTimeIsInvalid";
    }
}
