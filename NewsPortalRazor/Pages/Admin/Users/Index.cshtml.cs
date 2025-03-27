using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Azure;
using System.Drawing.Printing;
using X.PagedList.Extensions;

namespace NewsPortalRazor.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly NewsPortalContext _context;

        public IndexModel(NewsPortalContext context)
        {
            _context = context;
        }

        public IPagedList<User> Users { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        private const int PageSize = 6;

        public async Task OnGetAsync()
        {
            var query = _context.Users.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(u => u.FullName.Contains(SearchTerm) || u.Email.Contains(SearchTerm));
            }

            // Lọc theo Role
            if (!string.IsNullOrEmpty(RoleFilter))
            {
                query = query.Where(u => u.Role == RoleFilter);
            }

            // Lọc theo Trạng thái
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                bool isActive = StatusFilter == "Active";
                query = query.Where(u => u.IsActive == isActive);
            }

            // Sắp xếp theo Tên
            query = SortOrder == "desc" ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName);

            // Phân trang
            Users = await Task.Run(() => query.ToPagedList(PageNumber, PageSize));
        }
    }
}
