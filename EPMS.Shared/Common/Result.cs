using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EPMS.Shared.Common;

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new List<string>();

    [JsonConstructor]
    public Result()
    {
    }

    protected Result(bool isSuccess, string message, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static Result Success(string message = "") => new(true, message);
    public static Result Failure(string message, List<string>? errors = null) => new(false, message, errors);
    public static Result Failure(string error) => new(false, string.Empty, new List<string> { error });
}

public class Result<T> : Result
{
    public T? Value { get; set; }

    [JsonConstructor]
    public Result()
    {
    }

    private Result(bool isSuccess, T? value, string message, List<string>? errors = null)
        : base(isSuccess, message, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value, string message = "") => new(true, value, message);
    public static new Result<T> Failure(string message, List<string>? errors = null) => new(false, default, message, errors);
    public static new Result<T> Failure(string error) => new(false, default, string.Empty, new List<string> { error });
}
