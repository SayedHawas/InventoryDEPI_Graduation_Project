//using smERP.SharedKernel.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Net;
//using System.Text.Json.Serialization;
//using System.Threading;
//using System.Threading.Tasks;

//namespace smERP.SharedKernel.Bases;

//public struct Result : IResult
//{
//    public bool IsSuccess { get; set; }

//    [JsonIgnore]
//    public bool IsFailed => !IsSuccess;

//    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
//    public Error[]? Errors { get; set; }

//    public string Message { get; set; }

//    public HttpStatusCode? StatusCode { get; set; }

//    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
//    public Dictionary<string, string>? InvalidObject { get; set; }

//    [JsonIgnore]
//    public bool IsInvalid
//    {
//        get
//        {
//            if (!IsSuccess)
//            {
//                return InvalidObject?.Any() ?? false;
//            }

//            return false;
//        }
//    }

//    internal Result(bool isSuccess, Error[]? errors, Dictionary<string, string>? invalidObject)
//    {
//        IsSuccess = isSuccess;
//        Errors = errors;
//        InvalidObject = invalidObject;
//    }

//    public Result(bool isSuccess, HttpStatusCode statusCode, string? message, Error[]? errors)
//    {
//        IsSuccess = isSuccess;
//        Errors = errors;
//        Message = message;
//        StatusCode = statusCode;
//    }

//    internal Result(Exception exception)
//        : this(isSuccess: false, new Error[1] { Error.FromException(exception) }, null)
//    {
//    }

//    public void AddError(Error error)
//    {
//        if (Errors == null)
//        {
//            Errors = new Error[1] { error };
//        }
//        else
//        {
//            List<Error> list = Errors.ToList();
//            list.Add(error);
//            Errors = list.ToArray();
//        }
//    }

//    public void ThrowIfFail()
//    {
//        if ((!(Errors?.Any())) ?? true)
//        {
//            if (IsFailed)
//            {
//                throw new Exception("IsFailed");
//            }

//            return;
//        }

//        IEnumerable<Exception> enumerable = Errors.Select((Error s) => s.Exception() ?? new Exception(StringExtension.JoinFilter(';', s.ErrorCode, s.Message)));
//        if (Errors.Length == 1)
//        {
//            throw enumerable.First();
//        }

//        throw new AggregateException(enumerable);
//    }

//    public TEnum? ErrorCodeAs<TEnum>() where TEnum : Enum
//    {
//        if (!GetError().HasValue)
//        {
//            return default(TEnum);
//        }

//        return GetError().Value.ErrorCodeAs<TEnum>();
//    }

//    public bool IsErrorCode(Enum value)
//    {
//        return GetError()?.IsErrorCode(value) ?? false;
//    }

//    public bool IsNotErrorCode(Enum value)
//    {
//        return GetError()?.IsNotErrorCode(value) ?? false;
//    }

//    public Error? GetError()
//    {
//        if (Errors == null || Errors.Length == 0)
//        {
//            return null;
//        }

//        return Errors[0];
//    }

//    public void AddInvalidMessage(string message)
//    {
//        if (InvalidObject == null)
//        {
//            Dictionary<string, string> dictionary2 = (InvalidObject = new Dictionary<string, string>());
//        }

//        InvalidObject["message"] = message;
//    }

//    public void AddInvalidMessage(string key, string value)
//    {
//        if (InvalidObject == null)
//        {
//            Dictionary<string, string> dictionary2 = (InvalidObject = new Dictionary<string, string>());
//        }

//        InvalidObject[key] = value;
//    }

//    public static Result Fail()
//    {
//        return new Result(isSuccess: false, null, null);
//    }

//    public static Result Fail(string message)
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(message) }, null);
//    }

//    public static Result Fail<TEnum>(TEnum code) where TEnum : Enum
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(code) }, null);
//    }

//    public static Result Fail<TEnum>(string message, TEnum code) where TEnum : Enum
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(message, code) }, null);
//    }

//    public static Result Fail<TEnum>(TEnum code, string message) where TEnum : Enum
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(message, code) }, null);
//    }

//    public static Result Fail(Error error)
//    {
//        return new Result(isSuccess: false, new Error[1] { error }, null);
//    }

//    public static Result Fail(Error[]? errors)
//    {
//        return new Result(isSuccess: false, errors, null);
//    }

//    public static Result Fail(Exception? exception)
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.FromException(exception) }, null);
//    }

//    //public static Result<T> Fail<T>()
//    //{
//    //    return Result<T>.Fail();
//    //}

//    public static Result<T> Fail<T>(string message)
//    {
//        return Result<T>.Fail(message);
//    }

//    public static Result<T> Fail<T, TEnum>(TEnum code) where TEnum : Enum
//    {
//        return Result<T>.Fail(code);
//    }

//    public static Result<T> Fail<T, TEnum>(TEnum code, T value) where TEnum : Enum
//    {
//        return Result<T>.Fail(code, value);
//    }

//    public static Result<T> Fail<T, TEnum>(string message, TEnum code) where TEnum : Enum
//    {
//        return Result<T>.Fail(message, code);
//    }

//    public static Result<T> Fail<T, TEnum>(string message, TEnum code, T value) where TEnum : Enum
//    {
//        return Result<T>.Fail(message, code, value);
//    }

//    public static Result<T> Fail<T, TEnum>(TEnum code, string message) where TEnum : Enum
//    {
//        return Result<T>.Fail(code, message);
//    }

//    public static Result<T> Fail<T, TEnum>(TEnum code, string message, T value) where TEnum : Enum
//    {
//        return Result<T>.Fail(code, message, value);
//    }

//    public static Result<T> Fail<T>(Error error)
//    {
//        return Result<T>.Fail(error);
//    }

//    public static Result<T> Fail<T>(Error? error)
//    {
//        if (error.HasValue)
//        {
//            return new Result<T>(isSuccess: false, default(T), new Error[1] { error.Value }, null);
//        }

//        return new Result<T>(isSuccess: false, default(T), null, null);
//    }

//    public static Result<T> Fail<T>(Error[] errors)
//    {
//        return Result<T>.Fail(errors);
//    }

//    public static Result<T> Fail<T>(Exception? exception)
//    {
//        return Result<T>.Fail(exception);
//    }

//    public static Result<T> Fail<T>(Exception? exception, T value)
//    {
//        return Result<T>.Fail(exception, value);
//    }

//    public static Result From(Action action)
//    {
//        try
//        {
//            action();
//            return Succeed();
//        }
//        catch (Exception exception)
//        {
//            return Fail(Error.FromException(exception));
//        }
//    }

//    public static Result From(Func<Result> func)
//    {
//        try
//        {
//            return func();
//        }
//        catch (Exception exception)
//        {
//            return Fail(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result> From(Task task)
//    {
//        try
//        {
//            if (task.IsCompleted)
//            {
//                return Succeed();
//            }

//            if (task.IsCanceled || task.IsFaulted)
//            {
//                return Fail(Error.FromException(task.Exception));
//            }

//            await task;
//            return Succeed();
//        }
//        catch (Exception exception)
//        {
//            return Fail(Error.FromException(exception));
//        }
//    }

//    public static Result From(Result result)
//    {
//        if ((bool)result)
//        {
//            return result;
//        }

//        return Fail(result.Errors);
//    }

//    public static Result From<T>(T value)
//    {
//        try
//        {
//            if (value is Result resultValue)
//                return resultValue;

//            return Succeed();
//        }
//        catch (Exception exception)
//        {
//            return Fail(Error.FromException(exception));
//        }
//    }

//    public static Result From<T>(Result<T> result)
//    {
//        if ((bool)result)
//        {
//            return Succeed();
//        }

//        return Fail(result.Errors);
//    }

//    public static async Task<Result> From(Func<Task> task, CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            await Task.Run(task, cancellationToken);
//            return Succeed();
//        }
//        catch (Exception exception)
//        {
//            return Fail(Error.FromException(exception));
//        }
//    }

//    public static async ValueTask<Result> From(ValueTask valueTask)
//    {
//        try
//        {
//            if (valueTask.IsCompleted)
//            {
//                return Succeed();
//            }

//            if (valueTask.IsCanceled || valueTask.IsFaulted)
//            {
//                return Fail();
//            }

//            await valueTask;
//            return Succeed();
//        }
//        catch (Exception exception)
//        {
//            return Fail(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result> From(Func<ValueTask> valueTask)
//    {
//        try
//        {
//            await valueTask();
//            return Succeed();
//        }
//        catch (Exception exception)
//        {
//            return Fail(Error.FromException(exception));
//        }
//    }

//    public static Result From(bool condition)
//    {
//        if (!condition)
//        {
//            return Fail();
//        }

//        return Succeed();
//    }

//    public static Result From(bool condition, Error error)
//    {
//        if (!condition)
//        {
//            return Fail(error);
//        }

//        return Succeed();
//    }

//    public static Result From(Func<bool> condition)
//    {
//        if (!condition())
//        {
//            return Fail();
//        }

//        return Succeed();
//    }

//    public static Result From(Func<bool> condition, Error error)
//    {
//        if (!condition())
//        {
//            return Fail(error);
//        }

//        return Succeed();
//    }

//    public static Result<T> From<T>(Func<T> func)
//    {
//        try
//        {
//            return Result<T>.Succeed(func());
//        }
//        catch (Exception exception)
//        {
//            return Result<T>.Fail(Error.FromException(exception));
//        }
//    }

//    //public static Result<T> From<T>(Func<T> func)
//    //{
//    //    try
//    //    {
//    //        return Succeed(func());
//    //    }
//    //    catch (Exception exception)
//    //    {
//    //        return Fail<T>(Error.FromException(exception));
//    //    }
//    //}

//    public static Result<T> From<T>(Func<Result<T>> func)
//    {
//        try
//        {
//            return func();
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result<T>> From<T>(Task<T> task)
//    {
//        try
//        {
//            return Succeed(await task);
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result<T>> From<T>(Task<Result<T>> task)
//    {
//        try
//        {
//            return await task;
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result<T>> From<T>(Func<Task<T>> task, CancellationToken cancellationToken = default(CancellationToken))
//    {
//        try
//        {
//            return Succeed(await Task.Run(task, cancellationToken));
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result<T>> From<T>(Func<Task<Result<T>>> task, CancellationToken cancellationToken = default(CancellationToken))
//    {
//        try
//        {
//            return await Task.Run(task, cancellationToken);
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async ValueTask<Result<T>> From<T>(ValueTask<T> valueTask)
//    {
//        try
//        {
//            return Succeed(await valueTask);
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async ValueTask<Result<T>> From<T>(ValueTask<Result<T>> valueTask)
//    {
//        try
//        {
//            return await valueTask;
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result<T>> From<T>(Func<ValueTask<T>> valueTask)
//    {
//        try
//        {
//            return Succeed(await valueTask());
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static async Task<Result<T>> From<T>(Func<ValueTask<Result<T>>> valueTask)
//    {
//        try
//        {
//            return await valueTask();
//        }
//        catch (Exception exception)
//        {
//            return Fail<T>(Error.FromException(exception));
//        }
//    }

//    public static Result Invalid()
//    {
//        return new Result(isSuccess: false, null, new Dictionary<string, string> { { "message", "Invalid" } });
//    }

//    public static Result Invalid<TEnum>(TEnum code) where TEnum : Enum
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(code) }, new Dictionary<string, string> {
//        {
//            "code",
//            Enum.GetName(code.GetType(), code) ?? string.Empty
//        } });
//    }

//    public static Result Invalid(string message)
//    {
//        return new Result(isSuccess: false, null, new Dictionary<string, string> { { "message", message } });
//    }

//    public static Result Invalid<TEnum>(TEnum code, string message) where TEnum : Enum
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(code) }, new Dictionary<string, string> { { "message", message } });
//    }

//    public static Result Invalid(string key, string value)
//    {
//        return new Result(isSuccess: false, null, new Dictionary<string, string> { { key, value } });
//    }

//    public static Result Invalid<TEnum>(TEnum code, string key, string value) where TEnum : Enum
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(code) }, new Dictionary<string, string> { { key, value } });
//    }

//    public static Result Invalid(Dictionary<string, string> values)
//    {
//        return new Result(isSuccess: false, null, values);
//    }

//    public static Result Invalid<TEnum>(TEnum code, Dictionary<string, string> values) where TEnum : Enum
//    {
//        return new Result(isSuccess: false, new Error[1] { Error.Create(code) }, values);
//    }

//    public static Result<T> Invalid<T>()
//    {
//        return new Result<T>(isSuccess: false, default(T), null, new Dictionary<string, string> { { "message", "Invalid" } });
//    }

//    public static Result<T> Invalid<T, TEnum>(TEnum code) where TEnum : Enum
//    {
//        return new Result<T>(isSuccess: false, default(T), new Error[1] { Error.Create(code) }, new Dictionary<string, string> { { "message", "Invalid" } });
//    }

//    public static Result<T> Invalid<T>(string message)
//    {
//        return new Result<T>(isSuccess: false, default(T), null, new Dictionary<string, string> { { "message", message } });
//    }

//    public static Result<T> Invalid<T, TEnum>(TEnum code, string message) where TEnum : Enum
//    {
//        return new Result<T>(isSuccess: false, default(T), new Error[1] { Error.Create(code) }, new Dictionary<string, string> { { "message", message } });
//    }

//    public static Result<T> Invalid<T>(string key, string value)
//    {
//        return new Result<T>(isSuccess: false, default(T), null, new Dictionary<string, string> { { key, value } });
//    }

//    public static Result<T> Invalid<T, TEnum>(TEnum code, string key, string value) where TEnum : Enum
//    {
//        return new Result<T>(isSuccess: false, default(T), new Error[1] { Error.Create(code) }, new Dictionary<string, string> { { key, value } });
//    }

//    public static Result<T> Invalid<T>(Dictionary<string, string> values)
//    {
//        return new Result<T>(isSuccess: false, default(T), null, values);
//    }

//    public static Result<T> Invalid<T, TEnum>(TEnum code, Dictionary<string, string> values) where TEnum : Enum
//    {
//        return new Result<T>(isSuccess: false, default(T), new Error[1] { Error.Create(code) }, values);
//    }

//    public bool Equals(Result other)
//    {
//        if (IsSuccess == other.IsSuccess && GetError()?.Message == other.GetError()?.Message)
//        {
//            return GetError()?.ErrorCode == other.GetError()?.ErrorCode;
//        }

//        return false;
//    }

//    public override bool Equals(object? obj)
//    {
//        if (obj is Result other)
//        {
//            return Equals(other);
//        }

//        return false;
//    }

//    public override int GetHashCode()
//    {
//        int value = Errors?.Aggregate(0, (int current, Error error) => HashCode.Combine(current, error.GetHashCode())) ?? 0;
//        return HashCode.Combine(IsSuccess, value);
//    }

//    public static bool operator ==(Result obj1, bool obj2)
//    {
//        return obj1.IsSuccess == obj2;
//    }

//    public static bool operator !=(Result obj1, bool obj2)
//    {
//        return obj1.IsSuccess != obj2;
//    }


//    public static implicit operator bool(Result result)
//    {
//        return result.IsSuccess;
//    }

//    public static implicit operator Exception?(Result result)
//    {
//        return result.GetError()?.Exception();
//    }

//    public static implicit operator Result(Error error)
//    {
//        return Fail(error);
//    }

//    public static implicit operator Result(Error[]? errors)
//    {
//        return Fail(errors);
//    }

//    public static implicit operator Result(Exception? exception)
//    {
//        return Fail(Error.FromException(exception));
//    }

//    public static Result Succeed()
//    {
//        return new Result(isSuccess: true, null, null);
//    }

//    public static Result<T> Succeed<T>(T value)
//    {
//        return Result<T>.Succeed(value);
//    }

//    public static Result<T> Succeed<T>(Action<T> action) where T : new()
//    {
//        T val = new T();
//        action?.Invoke(val);
//        return Result<T>.Succeed(val);
//    }

//    public Task<Result> AsTask()
//    {
//        return Task.FromResult(this);
//    }

//    public ValueTask<Result> AsValueTask()
//    {
//        return ValueTask.FromResult(this);
//    }
//}

//public struct Error
//{
//    public string? ErrorCode { get; set; }

//    public string Message { get; set; }

//    public object? ExceptionObject { get; set; }

//    internal Error(string message, string? errorCode = null)
//    {
//        ExceptionObject = null;
//        Message = message;
//        ErrorCode = errorCode;
//    }

//    internal Error(Exception? exception, string? errorCode = null)
//    {
//        ExceptionObject = exception;
//        ErrorCode = errorCode;
//        Message = exception?.Message ?? string.Empty;
//    }

//    internal Error(Exception? exception, string message, string? errorCode = null)
//    {
//        ExceptionObject = exception;
//        ErrorCode = errorCode;
//        Message = message;
//    }

//    public Exception? Exception()
//    {
//        if (ExceptionObject is Exception result)
//        {
//            return result;
//        }

//        return ExceptionObject as Exception;
//    }

//    public T? Exception<T>() where T : class
//    {
//        if (ExceptionObject is T result)
//        {
//            return result;
//        }

//        return ExceptionObject as T;
//    }

//    public TEnum? ErrorCodeAs<TEnum>() where TEnum : Enum
//    {
//        if (ErrorCode == null)
//        {
//            return default(TEnum);
//        }

//        return (TEnum)Enum.Parse(typeof(TEnum), ErrorCode);
//    }

//    public bool IsErrorCode(Enum value)
//    {
//        return Enum.GetName(value.GetType(), value) == ErrorCode;
//    }

//    public bool IsNotErrorCode(Enum value)
//    {
//        return Enum.GetName(value.GetType(), value) != ErrorCode;
//    }

//    public bool Equals(Error other)
//    {
//        string text = Exception()?.Message;
//        string text2 = other.Exception()?.Message;
//        if (Message == other.Message && ErrorCode == other.ErrorCode)
//        {
//            return text == text2;
//        }

//        return false;
//    }

//    public override bool Equals(object? obj)
//    {
//        if (obj is Error other)
//        {
//            return Equals(other);
//        }

//        return false;
//    }

//    public override int GetHashCode()
//    {
//        return HashCode.Combine(Message, ErrorCode);
//    }

//    public static Error FromException(Exception? exception, string? errorCode = null)
//    {
//        return new Error(exception, errorCode);
//    }

//    public static bool operator ==(Error? error, Enum errorCode)
//    {
//        if (error.HasValue)
//        {
//            return error.Value.ErrorCode == Enum.GetName(errorCode.GetType(), errorCode);
//        }

//        return false;
//    }

//    public static bool operator !=(Error? error, Enum errorCode)
//    {
//        if (error.HasValue)
//        {
//            return error.Value.ErrorCode == Enum.GetName(errorCode.GetType(), errorCode);
//        }

//        return true;
//    }

//    public static bool operator ==(Enum errorCode, Error? error)
//    {
//        if (error.HasValue)
//        {
//            return error.Value.ErrorCode == Enum.GetName(errorCode.GetType(), errorCode);
//        }

//        return false;
//    }

//    public static bool operator !=(Enum errorCode, Error? error)
//    {
//        if (error.HasValue)
//        {
//            return error.Value.ErrorCode == Enum.GetName(errorCode.GetType(), errorCode);
//        }

//        return true;
//    }

//    public static Error Create(string message)
//    {
//        return new Error(message);
//    }

//    public static Error[] Create(string[]? errorsMessages)
//    {
//        if (errorsMessages == null)
//        {
//            return [];
//        }

//        var errors = new Error[errorsMessages.Length];
//        for (int i = 0; i < errorsMessages.Length; i++)
//        {
//            errors[i] = new Error(errorsMessages[i]);
//        }
//        return errors;
//    }


//    public static Error Create<TEnum>(TEnum errorCode) where TEnum : Enum
//    {
//        return new Error(string.Empty, Enum.GetName(typeof(TEnum), errorCode));
//    }

//    public static Error Create<TEnum>(string message, TEnum errorCode) where TEnum : Enum
//    {
//        return new Error(message, Enum.GetName(typeof(TEnum), errorCode));
//    }

//    public static Error Create<TEnum>(Exception? exception) where TEnum : Enum
//    {
//        return new Error(exception, string.Empty);
//    }

//    public static Error Create<TEnum>(Exception? exception, TEnum errorCode) where TEnum : Enum
//    {
//        return new Error(exception, Enum.GetName(typeof(TEnum), errorCode));
//    }

//    public static Error Create<TEnum>(string message, Exception exception, TEnum errorCode) where TEnum : Enum
//    {
//        return new Error(exception, message, Enum.GetName(typeof(TEnum), errorCode));
//    }
//}

///// <summary>
///// Represents a result from an operation that can either succeed or fail.
///// </summary>
///// <typeparam name="T">The type of the result value.</typeparam>
//[Serializable]
//[DebuggerDisplay("IsSuccess: {IsSuccess}; {GetError().HasValue ? \" Error code: \" + GetError()!.Value.ErrorCode : string.Empty}")]
//public partial struct Result<T> : IResult<T>
//{
//    /// <summary>
//    /// Initializes a new instance of the Result struct.
//    /// </summary>
//    internal Result(bool isSuccess, T? value, Error[]? errors, Dictionary<string, string>? invalidObject)
//    {
//        IsSuccess = isSuccess;
//        Value = value;
//        Errors = errors;
//        InvalidObject = invalidObject;
//    }

//    internal Result(bool isSuccess, T? value, HttpStatusCode statusCode, string? message, Error[]? errors)
//    {
//        IsSuccess = isSuccess;
//        Value = value;
//        Errors = errors;
//        Message = message;
//        StatusCode = statusCode;
//    }

//    public Result(bool isSuccess, HttpStatusCode statusCode, string? message, Error[]? errors)
//    {
//        IsSuccess = isSuccess;
//        Value = default;
//        Errors = errors;
//        Message = message;
//        StatusCode = statusCode;
//    }

//    /// <summary>
//    /// Initializes a new instance of the Result struct with an exception.
//    /// </summary>
//    internal Result(Exception exception) : this(false, default, new[] { Error.FromException(exception) }, default)
//    {
//    }

//    /// <summary>
//    /// Adds an error to the result.
//    /// </summary>
//    public void AddError(Error error)
//    {
//        if (Errors == null)
//        {
//            Errors = new[] { error };
//        }
//        else
//        {
//            var list = Errors.ToList();
//            list.Add(error);
//            Errors = list.ToArray();
//        }
//    }

//    /// <summary>
//    /// Throws an exception if the result is a failure.
//    /// </summary>
//    public void ThrowIfFail()
//    {
//        if (Errors?.Any() is not true)
//        {
//            if (IsFailed)
//                throw new Exception(nameof(IsFailed));

//            return;
//        }

//        var exceptions = Errors.Select(s => s.Exception() ?? new Exception(StringExtension.JoinFilter(';', s.ErrorCode, s.Message)));

//        if (Errors.Length == 1)
//            throw exceptions.First();

//        throw new AggregateException(exceptions);
//    }

//    /// <summary>
//    /// Gets a value indicating whether the result is a success.
//    /// </summary>
//    [MemberNotNullWhen(true, nameof(Value))]
//    public bool IsSuccess { get; set; }

//    /// <summary>
//    /// Gets a value indicating whether the result is empty.
//    /// </summary>
//    [MemberNotNullWhen(false, nameof(Value))]
//    public bool IsEmpty => Value is null;

//    /// <summary>
//    /// Gets a value indicating whether the result is a failure.
//    /// </summary>
//    [JsonIgnore]
//    public bool IsFailed => !IsSuccess;

//    /// <summary>
//    /// Gets or sets the value of the result.
//    /// </summary>
//    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
//    public T? Value { get; set; }

//    /// <summary>
//    /// Gets the first error of the result.
//    /// </summary>
//    public Error? GetError()
//    {
//        if (Errors == null || Errors.Length == 0)
//            return null;

//        return Errors[0];
//    }

//    /// <summary>
//    /// Gets or sets the errors of the result.
//    /// </summary>
//    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
//    public Error[]? Errors { get; set; }

//    /// <summary>
//    /// Gets or sets the invalid object of the result.
//    /// </summary>
//    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
//    public Dictionary<string, string>? InvalidObject { get; set; }

//    public string? Message { get; set; }

//    public HttpStatusCode? StatusCode { get; set; }

//    /// <summary>
//    /// Gets a value indicating whether the result is invalid.
//    /// </summary>
//    [JsonIgnore]
//    public bool IsInvalid => !IsSuccess || InvalidObject?.Any() is true;

//    /// <summary>
//    /// Gets the error code as a specific enum type.
//    /// </summary>
//    public TEnum? ErrorCodeAs<TEnum>() where TEnum : Enum
//    {
//        return GetError().HasValue ? GetError()!.Value.ErrorCodeAs<TEnum>() : default;
//    }

//    /// <summary>
//    /// Checks if the error code is a specific value.
//    /// </summary>
//    public bool IsErrorCode(Enum value)
//    {
//        return GetError()?.IsErrorCode(value) ?? false;
//    }

//    /// <summary>
//    /// Checks if the error code is not a specific value.
//    /// </summary>
//    public bool IsNotErrorCode(Enum value)
//    {
//        return GetError()?.IsNotErrorCode(value) ?? false;
//    }

//    /// <summary>
//    /// Adds an invalid message to the result.
//    /// </summary>
//    public void AddInvalidMessage(string message)
//    {
//        InvalidObject ??= new Dictionary<string, string>();
//        InvalidObject[nameof(message)] = message;
//    }

//    /// <summary>
//    /// Adds an invalid message with a specific key to the result.
//    /// </summary>
//    public void AddInvalidMessage(string key, string value)
//    {
//        InvalidObject ??= new Dictionary<string, string>();
//        InvalidObject[key] = value;
//    }

//    public static Result Succeed()
//    {
//        return new Result(true, default, default);
//    }

//    public static Result<T> Succeed<T>(T value)
//    {
//        return Result<T>.Succeed(value);
//    }

//    public static Result<T> Succeed<T>(T value, HttpStatusCode statusCode, string message)
//    {
//        return new Result<T>(true, value, statusCode, message, default);
//    }

//    public static Result<T> Created<T>(T value, string message)
//    {
//        return new Result<T>(true, value, HttpStatusCode.Created, message, default);
//    }

//    public static Result<T> Accepted<T>(T value, string message)
//    {
//        return new Result<T>(true, value, HttpStatusCode.Created, message, default);
//    }

//    public static Result<T> NoContent<T>(T value, string message)
//    {
//        return new Result<T>(true, value, HttpStatusCode.NoContent, message, default);
//    }

//    public static Result<T> BadRequest<T>(string message, string[]? errors)
//    {
//        return new Result<T>(false, default, HttpStatusCode.BadRequest, message, Error.Create(errors));
//    }

//    public static Result<T> NotFound<T>(string message, string[]? errors)
//    {
//        return new Result<T>(false, default, HttpStatusCode.NotFound, message, Error.Create(errors));
//    }

//    public static Result<T> Succeed<T>(Action<T> action) where T : new()
//    {
//        var result = new T();
//        action?.Invoke(result);
//        return Result<T>.Succeed(result);
//    }

//    /// <summary>
//    /// Creates a failed result.
//    /// </summary>
//    //public static Result<T> Fail()
//    //{
//    //    return new Result<T>(false, default, null, default);
//    //}

//    /// <summary>
//    /// Creates a failed result with a specific value.
//    /// </summary>
//    public static Result<T> Fail(T value)
//    {
//        return new Result<T>(false, value, null, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific error code.
//    /// </summary>
//    public static Result<T> Fail<TEnum>(TEnum code) where TEnum : Enum
//    {
//        return new Result<T>(false, default, new[] { Error.Create(code) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific error code and value.
//    /// </summary>
//    public static Result<T> Fail<TEnum>(TEnum code, T value) where TEnum : Enum
//    {
//        return new Result<T>(false, value, new[] { Error.Create(code) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific message.
//    /// </summary>
//    public static Result<T> Fail(string message)
//    {
//        return new Result<T>(false, default, new[] { Error.Create(message) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific message and value.
//    /// </summary>
//    public static Result<T> Fail(string message, T value)
//    {
//        return new Result<T>(false, value, new[] { Error.Create(message) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific message and error code.
//    /// </summary>
//    public static Result<T> Fail<TEnum>(string message, TEnum code) where TEnum : Enum
//    {
//        return new Result<T>(false, default, new[] { Error.Create(message, code) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific message, error code, and value.
//    /// </summary>
//    public static Result<T> Fail(HttpStatusCode code, string message, string[] errors)
//    {
//        return new Result<T>(false, default, code, message, Error.Create(errors));
//    }

//    public static Result<T> Fail<TEnum>(string message, TEnum code, T value) where TEnum : Enum
//    {
//        return new Result<T>(false, value, new[] { Error.Create(message, code) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific error code and message.
//    /// </summary>
//    public static Result<T> Fail<TEnum>(TEnum code, string message) where TEnum : Enum
//    {
//        return new Result<T>(false, default, new[] { Error.Create(message, code) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific error code, message, and value.
//    /// </summary>
//    public static Result<T> Fail<TEnum>(TEnum code, string message, T value) where TEnum : Enum
//    {
//        return new Result<T>(false, value, new[] { Error.Create(message, code) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific error.
//    /// </summary>
//    public static Result<T> Fail(Error error)
//    {
//        return new Result<T>(false, default, new[] { error }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific error.
//    /// </summary>
//    //public static Result<T> Fail(Error? error)
//    //{
//    //    if (error.HasValue)
//    //        return new Result<T>(false, default, new[] { error.Value }, default);

//    //    return new Result<T>(false, default, default, default);
//    //}

//    /// <summary>
//    /// Creates a failed result with a specific array of errors.
//    /// </summary>
//    public static Result<T> Fail(Error[]? errors)
//    {
//        return new Result<T>(false, default, errors, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific exception.
//    /// </summary>
//    public static Result<T> Fail(Exception? exception)
//    {
//        return new Result<T>(false, default, new[] { Error.FromException(exception) }, default);
//    }

//    /// <summary>
//    /// Creates a failed result with a specific exception and value.
//    /// </summary>
//    public static Result<T> Fail(Exception? exception, T value)
//    {
//        return new Result<T>(false, value, new[] { Error.FromException(exception) }, default);
//    }

//    public static bool operator ==(Result<T> obj1, bool obj2)
//    {
//        return obj1.IsSuccess == obj2;
//    }

//    public static bool operator !=(Result<T> obj1, bool obj2)
//    {
//        return obj1.IsSuccess != obj2;
//    }

//    public static implicit operator bool(Result<T> result)
//    {
//        return result.IsSuccess;
//    }

//    // Implicit conversion from Result<T> to Result
//    public static implicit operator Result(Result<T> result)
//    {
//        return new Result();
//    }

//    // Implicit conversion from T to Result<T>
//    public static implicit operator Result<T>(T value)
//    {
//        return new Result<T>();
//    }
//}

//public interface IResult
//{
//    bool IsSuccess { get; set; }
//    bool IsFailed { get; }
//    Error[]? Errors { get; set; }
//    Dictionary<string, string>? InvalidObject { get; set; }
//    string? Message { get; set; }
//    HttpStatusCode? StatusCode { get; set; }
//    bool IsInvalid { get; }

//    void AddError(Error error);
//    void ThrowIfFail();
//    Error? GetError();
//    TEnum? ErrorCodeAs<TEnum>() where TEnum : Enum;
//    bool IsErrorCode(Enum value);
//    bool IsNotErrorCode(Enum value);
//    void AddInvalidMessage(string message);
//    void AddInvalidMessage(string key, string value);
//    static Result BadRequest(string message, string[]? errors)
//    {
//        return new Result(false, HttpStatusCode.BadRequest, message, Error.Create(errors));
//    }
//}

//public interface IResult<T> : IResult
//{
//    T? Value { get; set; }
//    bool IsEmpty { get; }

//    static new Result<T> BadRequest(string message, string[]? errors)
//    {
//        return new Result<T>(false, default,HttpStatusCode.BadRequest, message, Error.Create(errors));
//    }
//}

