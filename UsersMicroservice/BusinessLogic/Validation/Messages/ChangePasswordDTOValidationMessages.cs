namespace BusinessLogic.Validation.Messages
{
    internal static class ChangePasswordDTOValidationMessages
    {
        public const string PasswordIsNull = "Password cannot be null.";
        public const string PasswordIsEmpty = "Password cannot be empty.";

        public const string PasswordsDoNotMatch = "Passwords do not match.";
    }
}
