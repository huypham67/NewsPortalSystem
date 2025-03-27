using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;

namespace NewsPortalRazor.Pages.Admin.Categories
{
    public class EditModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public EditModel(NewsPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            Category = category;
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories.Where(c => c.CategoryId != id), "CategoryId", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
                return Page();
            }

            var categoryToUpdate = await _context.Categories.FindAsync(Category.CategoryId);
            if (categoryToUpdate == null)
            {
                return NotFound();
            }

            categoryToUpdate.Name = Category.Name;
            categoryToUpdate.Description = Category.Description;
            categoryToUpdate.ParentCategoryId = Category.ParentCategoryId;
            categoryToUpdate.IsActive = Category.IsActive;
            categoryToUpdate.ModifiedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(Category.CategoryId))
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

        public async Task<IActionResult> OnPostToggleStatusAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            category.IsActive = !category.IsActive;
            category.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Category has been {(category.IsActive ? "activated" : "deactivated")}";

            return RedirectToPage("./Edit", new { id = category.CategoryId });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
