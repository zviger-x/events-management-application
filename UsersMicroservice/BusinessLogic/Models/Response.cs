using BusinessLogic.Validation.Results;

namespace BusinessLogic.Models
{
    public class Response
    {
        public Response()
        {
            Errors = new();
        }

        public Response(ValidationResultDictionary errors)
        {
            Errors = errors ?? new();
        }

        public ValidationResultDictionary Errors { get; private set; }

        public bool HasErrors => !Errors.IsValid;

        public static Response Success() => new();
        public static Response<T> Success<T>(T data) => Response<T>.Success(data);
        public static Response Fail(ValidationResultDictionary validationResultDictionary) => new(validationResultDictionary);
    }

    public class Response<T> : Response
    {
        public Response(T dataTransferObject)
            : base()
        {
            DataTransferObject = dataTransferObject;
        }

        public Response(ValidationResultDictionary errors)
            : base(errors)
        {
        }

        public T? DataTransferObject { get; private set; }

        public static Response<T> Success(T dataTransferObject) => new(dataTransferObject);

        public static new Response<T> Fail(ValidationResultDictionary validationResultDictionary) => new(validationResultDictionary);
    }
}
