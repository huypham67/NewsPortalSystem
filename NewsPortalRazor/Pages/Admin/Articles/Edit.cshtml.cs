using BusinessObjects.Entities;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Security.Claims;
using Services;
using Microsoft.AspNetCore.Authorization;

namespace NewsPortalRazor.Pages.Admin.Articles
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly NewsPortalContext _context;
        private readonly INotificationService _notificationService;

        public EditModel(NewsPortalContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var article = await _context.Articles
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null) return NotFound();
            Article = article;

            SelectedTagIds = Article.Tags.Select(t => t.TagId).ToList();
            await LoadViewDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadViewDataAsync();
                return Page();
            }

            var existingArticle = await _context.Articles
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.ArticleId == Article.ArticleId);

            if (existingArticle == null) return NotFound();

            UpdateArticle(existingArticle);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Article updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.ArticleId)) return NotFound();
                throw;
            }
        }

        private void UpdateArticle(Article existingArticle)
        {
            existingArticle.Title = Article.Title;
            existingArticle.Headline = Article.Headline;
            existingArticle.Content = Article.Content;
            existingArticle.Source = Article.Source;
            existingArticle.CategoryId = Article.CategoryId;
            existingArticle.Status = Article.Status;
            int? currentUserId = GetUserIdFromClaims();
            existingArticle.ModifiedBy = currentUserId.Value;
            existingArticle.ModifiedAt = DateTime.UtcNow;

            existingArticle.Tags.Clear();
            var selectedTags = _context.Tags.Where(t => SelectedTagIds.Contains(t.TagId)).ToList();
            existingArticle.Tags.AddRange(selectedTags);
        }

        public async Task<IActionResult> OnPostApproveAsync(string articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                TempData["ErrorMessage"] = "Article not found.";
                return RedirectToPage("./Index");
            }

            article.Status = "Published";
            int? currentUserId = GetUserIdFromClaims();
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "User not authenticated.";
                return RedirectToPage("./Index");
            }
            article.ModifiedBy = currentUserId.Value;

            article.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Article approved successfully.";


            // Gửi thông báo
            await _notificationService.NotifyUserAsync(
                article.CreatedBy,
                $"Admin has approved your article: {article.Title}",
                articleId
            );

            return RedirectToPage("./Edit", new { id = articleId });
        }

        public async Task<IActionResult> OnPostArchiveAsync(string articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                TempData["ErrorMessage"] = "Article not found.";
                return RedirectToPage("./Index");
            }

            article.Status = "Archived";
            int? currentUserId = GetUserIdFromClaims();
            article.ModifiedBy = currentUserId.Value;
            article.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Article archived successfully.";
            return RedirectToPage("./Edit", new { id = articleId });
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

        private bool ArticleExists(string id) => _context.Articles.Any(e => e.ArticleId == id);

        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
