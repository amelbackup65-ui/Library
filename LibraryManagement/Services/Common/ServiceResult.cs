namespace LibraryManagement.Services.Common;

/// <summary>
/// الگوی استاندارد کپسوله‌سازی نتیجه عملیات سرویس‌های کسب‌وکار
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; } = string.Empty;

    public static ServiceResult Ok(string message = "عملیات با موفقیت انجام شد.") =>
        new() { IsSuccess = true, Message = message };

    public static ServiceResult Fail(string message) =>
        new() { IsSuccess = false, Message = message };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Ok(T data, string message = "عملیات با موفقیت انجام شد.") =>
        new() { IsSuccess = true, Data = data, Message = message };

    public new static ServiceResult<T> Fail(string message) =>
        new() { IsSuccess = false, Data = default, Message = message };
}
