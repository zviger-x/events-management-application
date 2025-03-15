using FluentValidation.Results;

namespace BusinessLogic.Models
{
    [Obsolete(message: "", error: true)]
    public class Response
    {
        public Response()
        {
            Errors = new();
        }

        public Response(ValidationResult result)
        {
            Errors = (Dictionary<string, string[]>)result.ToDictionary() ?? new();
        }

        public Dictionary<string, string[]> Errors { get; private set; }

        public bool HasErrors => Errors.Count > 0;

        public static Response Success() => new();
        public static Response<T> Success<T>(T data) => Response<T>.Success(data);
        public static Response Fail(ValidationResult validationResult) => new(validationResult);
    }

    [Obsolete(message: "", error: true)]
    public class Response<T> : Response
    {
        public Response(T dataTransferObject)
            : base()
        {
            DataTransferObject = dataTransferObject;
        }

        public Response(ValidationResult result)
            : base(result)
        {
        }

        public T? DataTransferObject { get; private set; }

        public static Response<T> Success(T dataTransferObject) => new(dataTransferObject);

        public static new Response<T> Fail(ValidationResult validationResult) => new(validationResult);
    }
}
