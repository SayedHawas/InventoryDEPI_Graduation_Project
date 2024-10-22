using smERP.SharedKernel.Responses;
using System.Text.Json.Serialization;

namespace smERP.WebApi;

public record ApiResult
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsSuccess { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int StatusCode { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Message { get; init; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? ErrorMessages { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? SuccessMessages { get; init; }
}

public record ApiResult<T> : ApiResult
{
    public T? Value { get; init; }
}

public static class ResultExtensions
{
    public static ApiResult ToApiResult(this IResultBase result)
    {
        if (result is IResult<int> intResult)
        {
            return intResult.ToApiResult();
        }

        if (result is IResult<object> genericResult)
        {
            return genericResult.ToApiResult();
        }

        return new ApiResult
        {
            StatusCode = (int)result.StatusCode,
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorMessages = result.Errors.Count > 0 ? result.Errors.Select(e => e.Message).ToList() : default,
            SuccessMessages = result.Successes.Count > 0 ? result.Successes.Select(e => e.Message).ToList() : default
        };
    }

    public static ApiResult<T> ToApiResult<T>(this IResult<T> result)
    {
        return new ApiResult<T>
        {
            StatusCode = (int)result.StatusCode,
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorMessages = result.Errors.Count > 0 ? result.Errors.Select(e => e.Message).ToList() : default,
            SuccessMessages = result.Successes.Count > 0 ? result.Successes.Select(e => e.Message).ToList() : default,
            Value = result.IsSuccess ? result.Value : default
        };
    }

}