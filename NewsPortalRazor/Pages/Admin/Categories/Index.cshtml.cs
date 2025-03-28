using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace NewsPortalRazor.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public IndexModel(NewsPortalContext context)
        {
            _context = context;
        }

        public IPagedList<Category> Categories { get; set; } = default!;
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync(string? searchTerm, string? statusFilter, int pageNumber = 1)
        {
            SearchTerm = searchTerm;
            StatusFilter = statusFilter;
            PageNumber = pageNumber;

            // Query cơ bản
            var query = _context.Categories.Include(c => c.ParentCategory).AsQueryable();

            // Tìm kiếm theo tên danh mục
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(SearchTerm));
            }

            // Lọc trạng thái
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                bool isActive = StatusFilter == "Active";
                query = query.Where(c => c.IsActive == isActive);
            }

            // Phân trang với X.PagedList
            Categories = await Task.Run(() => query.OrderBy(c => c.Name).ToPagedList(PageNumber, PageSize));
        }
    }
}
