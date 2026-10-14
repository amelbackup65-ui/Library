using LibraryManagement.Models.ViewModels.Loans;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers.Api;

[ApiController]
[Route("api/loans")]
[Produces("application/json")]
public class LoansApiController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansApiController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    /// <summary>
    /// دریافت فهرست امانات با فیلتر وضعیت و جستجو
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLoans([FromQuery] string? search = null, [FromQuery] string? status = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _loanService.GetLoansPagedAsync(page, pageSize, search, status);
        return Ok(new
        {
            total = result.TotalCount,
            page = result.PageIndex,
            totalPages = result.TotalPages,
            items = result.Items
        });
    }

    /// <summary>
    /// ثبت امانت جدید کتاب
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateLoan([FromBody] CreateLoanViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _loanService.IssueLoanAsync(model);
        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(new { message = "امانت کتاب با موفقیت ثبت شد." });
    }

    /// <summary>
    /// تحویل و بازگشت کتاب به مخزن
    /// </summary>
    [HttpPost("{id}/return")]
    public async Task<IActionResult> ReturnLoan(int id)
    {
        var result = await _loanService.ReturnLoanAsync(id);
        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(new { message = "کتاب تحویل داده شد و موجودی به‌روز گردید." });
    }

    /// <summary>
    /// تمدید مهلت امانت کتاب
    /// </summary>
    [HttpPost("{id}/extend")]
    public async Task<IActionResult> ExtendLoan(int id, [FromQuery] int days = 7)
    {
        var result = await _loanService.ExtendLoanAsync(id, days);
        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(new { message = $"مهلت امانت به مدت {days} روز تمدید گردید." });
    }
}
