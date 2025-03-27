using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace NewsPortalRazor.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        // Xử lý đăng nhập bằng tài khoản hệ thống
        public async Task<IActionResult> OnPost()
        {
            var user = await _userService.GetUserByEmailAsync(Email);

            if (user == null || user.Password != Password)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role), // Role = "Admin" hoặc "Student"
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) // Lưu thêm UserId để tiện lấy dữ liệu
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToPage("/Index");
        }

        // Xử lý đăng nhập bằng Google
        public IActionResult OnGetGoogleLogin()
        {
            var redirectUrl = Url.Page("/Login", "GoogleResponse", null, Request.Scheme);
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnGetGoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                return RedirectToPage("/Login");
            }

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Google authentication failed.");
                return Page();
            }

            // Kiểm tra hoặc tạo user mới nếu chưa tồn tại
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FullName = name ?? "Google User",
                    Email = email,
                    Role = "Viewer",
                    IsActive = true,
                    IsExternalUser = true
                };
                await _userService.CreateUserAsync(user);
            }

            // Tạo Claims để lưu thông tin đăng nhập
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Index");
        }
    }
}
