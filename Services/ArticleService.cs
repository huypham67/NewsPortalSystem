using BusinessObjects.Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<IPagedList<Article>> GetPagedArticlesAsync(int pageNumber, int pageSize)
        {
            return await _articleRepository.GetPagedArticlesAsync(pageNumber, pageSize);
        }

        public async Task<IPagedList<Article>> GetPagedArticlesAsync(int page, int pageSize, 
                                string? searchQuery = null, int? categoryId = null,
                                string? dateRange = null,
                                DateTime? startDate = null, DateTime? endDate = null, List<int>? selectedTags = null)
        {
            // Service chỉ gọi Repo, không xử lý điều kiện lọc
            return await _articleRepository.GetPagedArticlesAsync(page, pageSize, searchQuery, categoryId, dateRange, startDate, endDate, selectedTags);
        }

        public async Task<List<Article>> GetLatestArticles(int count = 5)
        {
            return await _articleRepository.GetLatestArticles(count);
        }
    }
}
