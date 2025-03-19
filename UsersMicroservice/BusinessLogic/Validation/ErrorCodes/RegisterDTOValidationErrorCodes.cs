namespace BusinessLogic.Validation.ErrorCodes
{
    internal static class RegisterDTOValidationErrorCodes
    {
        public const string NameIsNull = "nameIsNull";
        public const string NameIsEmpty = "nameIsEmpty";

        public const string SurnameIsNull = "surnameIsNull";
        public const string SurnameIsEmpty = "surnameIsEmpty";

        public const string EmailIsNull = "emailIsNull";
        public const string EmailIsEmpty = "emailIsEmpty";
        public const string EmailIsInvalid = "emailIsInvalid";
        public const string EmailIsNotUnique = "emailIsNotUnique";

        public const string EmailOrPasswordIsInvalid = "emailOrPasswordIsInvalid";

        public const string PasswordIsNull = "passwordIsNull";
        public const string PasswordIsEmpty = "passwordIsEmpty";

        public const string PasswordsDoNotMatch = "passwordsDoNotMatch";
    }
}
