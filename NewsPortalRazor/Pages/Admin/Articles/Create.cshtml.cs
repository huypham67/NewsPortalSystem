using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;
using System.Security.Claims;
using Services;

namespace NewsPortalRazor.Pages.Admin.Articles
{
    public class CreateModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public CreateModel(NewsPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Article Article { get; set; } = new() { Status = "Draft" };

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Article.ArticleId = await GenerateArticleIdAsync();
            await LoadViewDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Article.ArticleId)) // Nếu chưa có, tạo lại
            {
                Article.ArticleId = await GenerateArticleIdAsync();
            }
            if (!ModelState.IsValid)
            {
                await LoadViewDataAsync();
                return Page();
            }
            // Kiểm tra trạng thái hợp lệ khi tạo bài viết
            if (Article.Status != "Draft" && Article.Status != "Pending Approval")
            {
                ModelState.AddModelError("Article.Status", "Invalid status selection.");
                return Page();
            }

            int? currentUserId = GetUserIdFromClaims();
            Article.CreatedBy = currentUserId.Value; // Tạm thời hardcode, có thể lấy từ session
            Article.CreatedAt = DateTime.UtcNow;

            if (SelectedTagIds.Any())
            {
                Article.Tags = await _context.Tags
                    .Where(t => SelectedTagIds.Contains(t.TagId))
                    .ToListAsync();
            }

            _context.Articles.Add(Article);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task<string> GenerateArticleIdAsync()
        {
            int year = DateTime.UtcNow.Year;
            string prefix = $"ART-{year}-";

            var lastArticle = await _context.Articles
                .Where(a => a.ArticleId.StartsWith(prefix))
                .OrderByDescending(a => a.ArticleId)
                .FirstOrDefaultAsync();

            int nextNumber = lastArticle != null ? int.Parse(lastArticle.ArticleId.Split('-').Last()) + 1 : 1;
            return $"{prefix}{nextNumber:D3}";
        }

        private async Task LoadViewDataAsync()
        {
            ViewData["Categories"] = new SelectList(await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync(), "CategoryId", "Name");

            ViewData["Tags"] = new SelectList(await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync(), "TagId", "Name");
        }

        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
