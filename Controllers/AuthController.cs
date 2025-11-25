using Microsoft.AspNetCore.Mvc;
using PawHelp.Models;

namespace PawHelp.Controllers;

public class AuthController : Controller
{
    // Tài khoản mặc định (trong thực tế nên dùng database và hash password)
    private static readonly Dictionary<string, string> _accounts = new()
    {
        { "admin", "admin123" },
        { "staff", "staff123" }
    };

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password, string? returnUrl = null)
    {
        if (_accounts.TryGetValue(username, out var pass) && pass == password)
        {
            // Đăng nhập thành công - trong thực tế nên dùng Cookie Authentication
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("IsAuthenticated", "true");
            
            TempData["Success"] = $"Đăng nhập thành công! Xin chào {username}";
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            
            return RedirectToAction("Index", "Admin");
        }
        
        TempData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "Đăng xuất thành công!";
        return RedirectToAction("Login");
    }
}

