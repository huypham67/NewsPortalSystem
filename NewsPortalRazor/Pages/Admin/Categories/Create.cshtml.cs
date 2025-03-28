using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NewsPortalRazor.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public CreateModel(NewsPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ParentCategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ParentCategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
                return Page();
            }

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
