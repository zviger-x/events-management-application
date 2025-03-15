namespace BusinessLogic.Validation.ErrorCodes
{
    internal static class UserValidationErrorCodes
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
    }
}
