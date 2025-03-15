namespace BusinessLogic.Validation.Messages
{
    internal static class UserValidationMessages
    {
        public const string NameIsNull = "User name cannot be null.";
        public const string NameIsEmpty = "User name cannot be empty.";

        public const string SurnameIsNull = "User surname cannot be null.";
        public const string SurnameIsEmpty = "User surname cannot be empty.";

        public const string EmailIsNull = "Email cannot be null.";
        public const string EmailIsEmpty = "Email cannot be empty.";
        public const string EmailIsInvalid = "Email format is invalid.";
        public const string EmailIsNotUnique = "Email must be unique.";

        public const string EmailOrPasswordIsInvalid = "Invalid email or password.";
    }
}
