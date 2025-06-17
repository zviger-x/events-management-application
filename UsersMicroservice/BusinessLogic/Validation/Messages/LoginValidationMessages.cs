namespace BusinessLogic.Validation.Messages
{
    internal static class LoginValidationMessages
    {
        public const string EmailIsNull = "Email cannot be null.";
        public const string EmailIsEmpty = "Email cannot be empty.";
        public const string EmailIsInvalid = "Email format is invalid.";

        public const string EmailOrPasswordIsInvalid = "Invalid email or password.";

        public const string PasswordIsNull = "Password cannot be null.";
        public const string PasswordIsEmpty = "Password cannot be empty.";
    }
}
