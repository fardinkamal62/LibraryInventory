namespace LibraryInventory.Core.Common;

public enum ErrorType
{
    None,
    NotFound,
    Conflict,
    Validation,
    RuleViolation
}

public class Result<T>
{
    private Result(bool ok, string message, T? data, ErrorType error)
    {
        IsSuccess = ok;
        Message = message;
        Data = data;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Message { get; }
    public T? Data { get; }
    public ErrorType Error { get; }

    public static Result<T> Success(T data, string? msg = null)
    {
        return new Result<T>(true, msg ?? "", data, ErrorType.None);
    }

    public static Result<T> Failure(ErrorType error, string msg)
    {
        return new Result<T>(false, msg, default, error);
    }

    public static Result<T> NotFound(string msg)
    {
        return Failure(ErrorType.NotFound, msg);
    }

    public static Result<T> Conflict(string msg)
    {
        return Failure(ErrorType.Conflict, msg);
    }

    public static Result<T> Validation(string msg)
    {
        return Failure(ErrorType.Validation, msg);
    }

    public static Result<T> RuleViolation(string msg)
    {
        return Failure(ErrorType.RuleViolation, msg);
    }
}

public class OperationResult
{
    private OperationResult(bool ok, string message, ErrorType error)
    {
        IsSuccess = ok;
        Message = message;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Message { get; }
    public ErrorType Error { get; }

    public static OperationResult Success(string msg = "")
    {
        return new OperationResult(true, msg, ErrorType.None);
    }

    public static OperationResult Failure(ErrorType e, string msg)
    {
        return new OperationResult(false, msg, e);
    }

    public static OperationResult NotFound(string m)
    {
        return Failure(ErrorType.NotFound, m);
    }

    public static OperationResult Conflict(string m)
    {
        return Failure(ErrorType.Conflict, m);
    }

    public static OperationResult RuleViolation(string m)
    {
        return Failure(ErrorType.RuleViolation, m);
    }
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);

public static class Paging
{
    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        return (Math.Max(1, page), Math.Clamp(pageSize, 1, 100));
    }
}