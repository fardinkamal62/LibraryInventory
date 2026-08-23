namespace LibraryInventory.Core.Common;

public class Result<T>
  {
      public bool IsSuccess { get; }
      public string Message { get; }
      public T? Data { get; }
      public bool IsFailure => !IsSuccess;

      private Result(bool isSuccess, string message, T? data)
      {
          IsSuccess = isSuccess; Message = message; Data = data;
      }
      public static Result<T> Success(T data, string? message = null) => new(true, message ?? "", data);
      public static Result<T> Failure(string message) => new(false, message, default);
  }

  // Non-generic Result for operations that don't return a value.
  // Named OperationResult to avoid collision with ASP.NET Core's IActionResult.
  public class OperationResult
  {
      public bool IsSuccess { get; }
      public string Message { get; }
      public bool IsFailure => !IsSuccess;

      private OperationResult(bool isSuccess, string message)
      {
          IsSuccess = isSuccess;
          Message = message;
      }

      public static OperationResult Success(string message = "") =>
          new(true, message);

      public static OperationResult Failure(string message) =>
          new(false, message);
  }