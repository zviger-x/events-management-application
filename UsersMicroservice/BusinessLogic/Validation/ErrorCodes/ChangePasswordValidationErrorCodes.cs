namespace BusinessLogic.Validation.ErrorCodes
{
    internal static class ChangePasswordValidationErrorCodes
    {
        public const string CurrentPasswordIsNull = "currentPasswordIsNull";
        public const string CurrentPasswordIsEmpty = "currentPasswordIsEmpty";
        public const string CurrentPasswordIsInvalid = "currentPasswordIsInvalid";

        public const string PasswordIsNull = "passwordIsNull";
        public const string PasswordIsEmpty = "passwordIsEmpty";

        public const string PasswordsDoNotMatch = "passwordsDoNotMatch";
    }
}
