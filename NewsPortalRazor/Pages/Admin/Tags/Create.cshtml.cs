using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;
using BusinessObjects.Entities;

namespace NewsPortalRazor.Pages.Admin.Tags
{
    public class CreateModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public CreateModel(NewsPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tag Tag { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set CreatedAt
            Tag.CreatedAt = DateTime.UtcNow;

            _context.Tags.Add(Tag);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
