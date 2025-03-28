using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace NewsPortalRazor.Pages.Admin.Articles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public IndexModel(NewsPortalContext context)
        {
            _context = context;
        }

        public IPagedList<Article> Articles { get; set; } = default!;
        public List<Category> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            var query = _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(a => a.Title.Contains(SearchTerm) || a.Headline.Contains(SearchTerm));
            }

            if (CategoryFilter.HasValue)
            {
                query = query.Where(a => a.CategoryId == CategoryFilter);
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                query = query.Where(a => a.Status == StatusFilter);
            }

            Articles = await Task.Run(() => query.OrderByDescending(a => a.CreatedAt).ToPagedList(PageNumber, 10));
        }
    }
}
