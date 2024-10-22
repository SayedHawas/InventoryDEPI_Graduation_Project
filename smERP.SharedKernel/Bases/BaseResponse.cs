using System.Net;

namespace smERP.SharedKernel.Bases;

public class BaseResponse<T>
{
    public HttpStatusCode StatusCode { get; }
    public bool Succeeded => StatusCode >= HttpStatusCode.OK && StatusCode < HttpStatusCode.BadRequest;
    public string Message { get; }
    public IReadOnlyList<string> Errors { get; }
    public T Data { get; }
    public object Meta { get; }

    private BaseResponse(HttpStatusCode statusCode, string message, IEnumerable<string> errors, T data, object meta)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Data = data;
        Meta = meta;
    }

    public static BaseResponse<T> Success(T data, string message = null, object meta = null)
        => new(HttpStatusCode.OK, message, null, data, meta);

    public static BaseResponse<T> Failure(HttpStatusCode statusCode, string message, IEnumerable<string> errors = null)
        => new(statusCode, message, errors, default, null);

    public static BaseResponse<T> Created(T data, string message = null, object meta = null)
        => new(HttpStatusCode.Created, message, null, data, meta);

    public BaseResponse<T> AddError(string error, HttpStatusCode? newStatusCode = null)
    {
        var newErrors = new List<string>(Errors) { error };
        return new BaseResponse<T>(newStatusCode ?? StatusCode, Message, newErrors, Data, Meta);
    }

    public BaseResponse<T> AddErrors(IEnumerable<string> errors, HttpStatusCode? newStatusCode = null)
    {
        var newErrors = new List<string>(Errors);
        newErrors.AddRange(errors);
        return new BaseResponse<T>(newStatusCode ?? StatusCode, Message, newErrors, Data, Meta);
    }
}