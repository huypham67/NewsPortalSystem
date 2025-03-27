using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;
using System.Security.Cryptography;
using System.Text;

namespace NewsPortalRazor.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly BusinessObjects.NewsPortalContext _context;

        public EditModel(BusinessObjects.NewsPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("User.Password");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userToUpdate = await _context.Users.FindAsync(User.UserId);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            // Chỉ cập nhật thông tin cần thiết
            userToUpdate.FullName = User.FullName;
            userToUpdate.Role = User.Role;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("./Index");
            }

            // Tạo mật khẩu mới
            string newPassword = GenerateRandomPassword();

            // Mã hóa mật khẩu trước khi lưu (Nếu DB lưu hashed password)
            user.Password = HashPassword(newPassword);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Password has been reset! New Password: {newPassword}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToPage("./Edit", new { id = user.UserId });
        }


        public async Task<IActionResult> OnPostToggleStatusAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Đảo ngược trạng thái hiện tại
            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            string status = user.IsActive ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"User has been {status}.";

            return RedirectToPage("./Edit", new { id = user.UserId });
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private string GenerateRandomPassword()
        {
            const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";
            Random rnd = new Random();
            return new string(Enumerable.Repeat(validChars, 10)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
