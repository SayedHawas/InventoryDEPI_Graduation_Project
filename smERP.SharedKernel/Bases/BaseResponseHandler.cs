using Microsoft.Extensions.Localization;
using System.Net;

namespace smERP.SharedKernel.Bases;

public class BaseResponseHandler
{
    private readonly IStringLocalizer<BaseResponseHandler> _localizer;

    public BaseResponseHandler(IStringLocalizer<BaseResponseHandler> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public BaseResponse<T> Success<T>(T data, string message = null, object meta = null)
        => BaseResponse<T>.Success(data, message ?? _localizer["Successfully"], meta);

    public BaseResponse<T> Created<T>(T data, object meta = null)
        => BaseResponse<T>.Created(data, _localizer["Created"], meta);

    public BaseResponse<T> Deleted<T>()
        => BaseResponse<T>.Success(default, _localizer["DeletedSuccessfully"]);

    public BaseResponse<T> NotFound<T>(string message = null)
        => BaseResponse<T>.Failure(HttpStatusCode.NotFound, message ?? _localizer["NotFound"]);

    public BaseResponse<T> BadRequest<T>(string message, IEnumerable<string> errors = null)
        => BaseResponse<T>.Failure(HttpStatusCode.BadRequest, message ?? _localizer["BadRequest"], errors);

    public BaseResponse<T> Unauthorized<T>()
        => BaseResponse<T>.Failure(HttpStatusCode.Unauthorized, _localizer["Unauthorized"]);

    public BaseResponse<T> Conflict<T>(string message = null)
        => BaseResponse<T>.Failure(HttpStatusCode.Conflict, message ?? _localizer["Conflict"]);

    public BaseResponse<T> UnprocessableEntity<T>(string message = null)
        => BaseResponse<T>.Failure(HttpStatusCode.UnprocessableEntity, message ?? _localizer["UnprocessableEntity"]);
}
