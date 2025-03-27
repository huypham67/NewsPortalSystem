using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Services
{
    public interface IArticleService
    {
        Task<IPagedList<Article>> GetPagedArticlesAsync(int pageNumber, int pageSize);
        Task<IPagedList<Article>> GetPagedArticlesAsync(int page, int pageSize,
                                string? searchQuery = null, int? categoryId = null,
                                string? dateRange = null,
                                DateTime? startDate = null, DateTime? endDate = null, List<int>? selectedTags = null);
        Task<List<Article>> GetLatestArticles(int count = 5);
    }
}
