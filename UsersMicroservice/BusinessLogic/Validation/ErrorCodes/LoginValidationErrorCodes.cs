namespace BusinessLogic.Validation.ErrorCodes
{
    internal static class LoginValidationErrorCodes
    {
        public const string EmailIsNull = "emailIsNull";
        public const string EmailIsEmpty = "emailIsEmpty";
        public const string EmailIsInvalid = "emailIsInvalid";

        public const string EmailOrPasswordIsInvalid = "emailOrPasswordIsInvalid";

        public const string PasswordIsNull = "passwordIsNull";
        public const string PasswordIsEmpty = "passwordIsEmpty";
    }
}
