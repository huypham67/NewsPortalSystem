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

namespace NewsPortalRazor.Pages.Admin.Tags
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly BusinessObjects.NewsPortalContext _context;

        public DetailsModel(BusinessObjects.NewsPortalContext context)
        {
            _context = context;
        }

        public Tag Tag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FirstOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }
            else
            {
                Tag = tag;
            }
            return Page();
        }
    }
}
