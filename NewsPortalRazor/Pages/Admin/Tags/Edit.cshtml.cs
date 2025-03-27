using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;

namespace NewsPortalRazor.Pages.Admin.Tags
{
    public class EditModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public EditModel(NewsPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

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

            Tag = tag;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tagToUpdate = await _context.Tags.FindAsync(Tag.TagId);
            if (tagToUpdate == null)
            {
                return NotFound();
            }

            tagToUpdate.Name = Tag.Name;
            tagToUpdate.Description = Tag.Description;
            tagToUpdate.ModifiedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                SuccessMessage = "Tag updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(Tag.TagId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }
    }
}