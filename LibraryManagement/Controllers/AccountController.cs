using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // در صورتی که کاربر قبلاً لاگین کرده باشد، مستقیم به صفحه اصلی هدایت می‌شود
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            // تلاش دوم با فیلد ایمیل در صورت درج ایمیل به جای نام کاربری
            user = await _userManager.FindByEmailAsync(model.UserName);
        }

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور وارد شده اشتباه است.");
            _logger.LogWarning("تلاش ناموفق برای ورود با نام کاربری ناموجود: {UserName}", model.UserName);
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "حساب کاربری شما توسط مدیر سامانه غیرفعال شده است.");
            _logger.LogWarning("تلاش ورود برای حساب کاربری مسدود شده: {UserName}", model.UserName);
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("کاربر {UserName} با موفقیت وارد سیستم شد.", user.UserName);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "حساب کاربری به دلیل تکرار ورود ناموفق موقتاً مسدود گردید. لطفاً دقایقی دیگر تلاش کنید.");
            _logger.LogWarning("حساب کاربری {UserName} به دلیل تعدد تلاش‌های ناموفق قفل شد.", model.UserName);
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور وارد شده اشتباه است.");
        _logger.LogWarning("کلمه عبور نادرست برای حساب: {UserName}", model.UserName);
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userName = User.Identity?.Name;
        await _signInManager.SignOutAsync();
        _logger.LogInformation("کاربر {UserName} از سیستم خارج شد.", userName);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }
}
