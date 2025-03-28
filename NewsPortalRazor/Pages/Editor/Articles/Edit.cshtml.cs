using BusinessObjects.Entities;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using NuGet.Packaging;
using Microsoft.AspNetCore.Authorization;

namespace NewsPortalRazor.Pages.Editor.Articles
{
    [Authorize(Roles = "Editor")]
    public class EditModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public EditModel(NewsPortalContext context)
        {
            _context = context;
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
            if (article.Status != "Draft" && article.Status != "Pending Approval") return Forbid();

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
            if (existingArticle.Status != "Draft" && existingArticle.Status != "Pending Approval") return Forbid();

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

        public async Task<IActionResult> OnPostPostForApprovalAsync()
        {
            var article = await _context.Articles.FindAsync(Article.ArticleId);
            if (article == null) return NotFound();
            if (article.Status != "Draft") return Forbid();

            article.Status = "Pending Approval";
            int? currentUserId = GetUserIdFromClaims();
            article.ModifiedBy = currentUserId.Value;
            article.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Article submitted for approval!";
            return RedirectToPage("./Edit", new { id = article.ArticleId });
        }

        private void UpdateArticle(Article existingArticle)
        {
            existingArticle.Title = Article.Title;
            existingArticle.Headline = Article.Headline;
            existingArticle.Content = Article.Content;
            existingArticle.CategoryId = Article.CategoryId;
            existingArticle.Source = Article.Source;
            int? currentUserId = GetUserIdFromClaims();
            existingArticle.ModifiedBy = currentUserId.Value;
            existingArticle.ModifiedAt = DateTime.UtcNow;

            existingArticle.Tags.Clear();
            var selectedTags = _context.Tags.Where(t => SelectedTagIds.Contains(t.TagId)).ToList();
            existingArticle.Tags.AddRange(selectedTags);
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