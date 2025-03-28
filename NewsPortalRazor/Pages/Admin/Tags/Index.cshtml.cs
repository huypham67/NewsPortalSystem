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

namespace NewsPortalRazor.Pages.Admin.Tags
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public IndexModel(NewsPortalContext context)
        {
            _context = context;
        }

        public IPagedList<Tag> Tags { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public async Task OnGetAsync()
        {
            IQueryable<Tag> query = _context.Tags;

            // Search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(t => t.Name.Contains(SearchTerm));
            }

            // Pagination
            Tags = await Task.Run(() => query.OrderBy(t => t.Name)
                              .ToPagedList(PageNumber, 10));
        }
    }
}
