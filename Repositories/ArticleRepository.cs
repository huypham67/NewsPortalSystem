using BusinessObjects.Entities;
using BusinessObjects;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using System.Linq;

namespace Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly NewsPortalContext _context;

        public ArticleRepository(NewsPortalContext context)
        {
            _context = context;
        }



        public async Task<List<Article>> GetLatestArticles(int count = 5)
        {
            return await _context.Articles
                .Where(a => a.Status == "Published")
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .Include(a => a.Category) // Bao gồm danh mục nếu cần
                .ToListAsync();
        }

        public async Task<IPagedList<Article>> GetPagedArticlesAsync(int pageNumber, int pageSize)
        {
            return await Task.Run(() =>
                _context.Articles
                .Where(a => a.Status == "Published")
                .OrderByDescending(a => a.CreatedAt)
                .Include(a => a.Category)
                .ToPagedList(pageNumber, pageSize) // Chỉ có ToPagedList(), không có Async
            );
        }
        public async Task<IPagedList<Article>> GetPagedArticlesAsync(int page, int pageSize, 
                                        string? searchQuery = null, int? categoryId = null,
                                        string? dateRange = null, 
                                        DateTime? startDate = null, 
                                        DateTime? endDate = null, List<int>? selectedTags = null)
        {
            var query = _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .Where(a => a.Status == "Published");

            // Xử lý lọc trong Repository
            query = ApplyFilters(query, searchQuery, categoryId, dateRange, startDate, endDate, selectedTags);

            // Sắp xếp bài mới nhất trước
            query = query.OrderByDescending(a => a.CreatedAt);

            return await Task.Run(() => query.ToPagedList(page, pageSize));
        }

        // Hàm riêng để xử lý các điều kiện lọc
        private IQueryable<Article> ApplyFilters(
                IQueryable<Article> query, string? searchQuery, int? categoryId,
                string? dateRange, DateTime? startDate, DateTime? endDate, List<int>? selectedTags)
        {
            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(a => a.Title.Contains(searchQuery) || a.Headline.Contains(searchQuery));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                var categoryIds = _context.Categories
                                    .Where(c => c.CategoryId == categoryId || c.ParentCategoryId == categoryId)
                                    .Select(c => c.CategoryId)
                                    .ToList();

                query = query.Where(a => a.CategoryId != null && categoryIds.Contains(a.CategoryId.Value));
            }

            // Lọc theo khoảng thời gian preset
            if (!string.IsNullOrEmpty(dateRange))
            {
                DateTime now = DateTime.UtcNow;
                startDate = dateRange switch
                {
                    "Today" => now.Date,
                    "Last 3 days" => now.Date.AddDays(-3),
                    "Last week" => now.Date.AddDays(-7),
                    "Last month" => now.Date.AddMonths(-1),
                    "Last 3 months" => now.Date.AddMonths(-3),
                    "Last year" => now.Date.AddYears(-1),
                    _ => startDate
                };
            }

            // Kiểm tra StartDate <= EndDate trước khi lọc
            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate > endDate)
                {
                    startDate = null;
                    endDate = null;
                }
            }

            // Áp dụng bộ lọc theo ngày
            if (startDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= startDate);
            }
            if (endDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt <= endDate.Value.AddDays(1).AddTicks(-1)); // Đưa EndDate đến cuối ngày
            }

            // Lọc theo tags
            if (selectedTags != null && selectedTags.Any())
            {
                query = query.Where(a => a.Tags.Any(t => selectedTags.Contains(t.TagId)));
            }

            return query;
        }

    }
}
