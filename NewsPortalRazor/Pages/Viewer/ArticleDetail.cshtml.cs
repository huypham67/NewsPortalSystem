using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace NewsPortalRazor.Pages.Viewer
{
    public class ArticleDetailModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public ArticleDetailModel(NewsPortalContext context)
        {
            _context = context;
        }

        public Article? Article { get; set; }
        public List<Article> RelatedArticles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Lấy bài viết theo ID và load danh mục, tags
            Article = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (Article == null)
            {
                return NotFound();
            }

            // Lấy danh sách bài viết liên quan (cùng danh mục)
            RelatedArticles = await _context.Articles
                .Where(a => a.CategoryId == Article.CategoryId && a.ArticleId != id)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new Article
                {
                    ArticleId = a.ArticleId,
                    Title = a.Title,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Page();
        }
    }
}
