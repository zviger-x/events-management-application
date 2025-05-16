namespace BusinessLogic.Validation.Messages
{
    internal static class ChangePasswordValidationMessages
    {
        public const string CurrentPasswordIsNull = "Current password cannot be null.";
        public const string CurrentPasswordIsEmpty = "Current password cannot be empty.";
        public const string CurrentPasswordIsInvalid = "Current password is invalid";

        public const string PasswordIsNull = "Password cannot be null.";
        public const string PasswordIsEmpty = "Password cannot be empty.";

        public const string PasswordsDoNotMatch = "Passwords do not match.";
    }
}
