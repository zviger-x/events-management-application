using FluentValidation.Results;

namespace BusinessLogic.Validation.Results
{
    public class ValidationResultDictionary
    {
        public ValidationResultDictionary()
        {
            Errors = new();
        }

        public ValidationResultDictionary(ValidationResult result)
            : this(result.ToDictionary())
        {
        }

        public ValidationResultDictionary(IDictionary<string, string[]> errors)
        {
            Errors = (Dictionary<string, string[]>)errors;
        }

        public Dictionary<string, string[]> Errors { get; private set; }

        public bool IsValid => Errors.Count == 0;
    }
}
