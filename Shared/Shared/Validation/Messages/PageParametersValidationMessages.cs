namespace Shared.Validation.Messages
{
    internal static class PageParametersValidationMessages
    {
        public const string PageNumberIsInvalid = "Page number must be greater than zero.";
        public const string PageSizeIsTooSmall = "Page size must be greater than zero.";
        public const string PageSizeIsTooLarge = "Page size exceeded the maximum allowed limit ({0}).";
    }
}
