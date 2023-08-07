using Domain.Exceptions;

namespace Application.Common.Models;

public class AppResponse<T>
{
    private AppResponse()
    {
        Message = default;
        Data = default;
    }

    public static AppResponse<T> Success(T? data = default, string? message = null)
    {
        return new AppResponse<T>
        {
            Succeeded = true,
            Message = message,
            Data = data
        };
    }

    public static AppResponse<T> Fail(IEnumerable<string> errors, string? message = null)
    {
        return new AppResponse<T>
        {
            Succeeded = false,
            Message = message,
            Errors = errors
        };
    }

    public static AppResponse<T> Fail(AppException exception)
    {
        return new AppResponse<T>
        {
            Succeeded = false,
            Message = exception.Message,
            Errors = exception.Errors
        };
    }

    public bool Succeeded { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public IEnumerable<string>? Errors { get; init; }
}
