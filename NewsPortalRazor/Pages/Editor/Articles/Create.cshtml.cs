using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace NewsPortalRazor.Pages.Editor.Articles
{
    [Authorize(Roles = "Editor")]
    public class CreateModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public CreateModel(NewsPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Article Article { get; set; } = new();

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Article.ArticleId = await GenerateArticleIdAsync();
            await LoadViewDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveDraftAsync()
        {
            return await SaveArticleAsync("Draft");
        }

        public async Task<IActionResult> OnPostPostForApprovalAsync()
        {
            return await SaveArticleAsync("Pending Approval");
        }

        private async Task<IActionResult> SaveArticleAsync(string status)
        {
            if (string.IsNullOrEmpty(Article.ArticleId))
            {
                Article.ArticleId = await GenerateArticleIdAsync();
            }
            if (!ModelState.IsValid)
            {
                await LoadViewDataAsync();
                return Page();
            }

            int? currentUserId = GetUserIdFromClaims();
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            Article.CreatedBy = currentUserId.Value;
            Article.CreatedAt = DateTime.UtcNow;
            Article.Status = status;

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
