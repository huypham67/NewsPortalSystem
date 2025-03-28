using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authorization;

namespace NewsPortalRazor.Pages.Admin.Articles
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public DetailsModel(NewsPortalContext context)
        {
            _context = context;
        }

        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }
            Article = article;

            return Page();
        }
    }
}
