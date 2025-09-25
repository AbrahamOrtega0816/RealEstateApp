namespace RealEstateAPI.Common.DTOs;

/// <summary>
/// Generic service result wrapper for consistent API responses
/// </summary>
/// <typeparam name="T">Type of data returned</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Data returned from the operation
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Additional error details
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    /// <param name="data">Data to return</param>
    /// <returns>Successful service result</returns>
    public static ServiceResult<T> SuccessResult(T data)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed result with error message
    /// </summary>
    /// <param name="error">Error message</param>
    /// <returns>Failed service result</returns>
    public static ServiceResult<T> FailureResult(string error)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Error = error
        };
    }

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    /// <param name="errors">List of error messages</param>
    /// <returns>Failed service result</returns>
    public static ServiceResult<T> FailureResult(List<string> errors)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Errors = errors,
            Error = errors.FirstOrDefault()
        };
    }
}

/// <summary>
/// Non-generic service result for operations that don't return data
/// </summary>
public class ServiceResult : ServiceResult<object>
{
    /// <summary>
    /// Creates a successful result without data
    /// </summary>
    /// <returns>Successful service result</returns>
    public static ServiceResult SuccessResult()
    {
        return new ServiceResult
        {
            Success = true
        };
    }

    /// <summary>
    /// Creates a failed result with error message
    /// </summary>
    /// <param name="error">Error message</param>
    /// <returns>Failed service result</returns>
    public new static ServiceResult FailureResult(string error)
    {
        return new ServiceResult
        {
            Success = false,
            Error = error
        };
    }
}
