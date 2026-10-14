namespace LibraryManagement.Models.ViewModels.Common;

/// <summary>
/// مدل خطای عمومی سامانه جهت نمایش جزئیات ردیابی در صفحات خطا
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
