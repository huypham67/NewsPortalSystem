using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using X.PagedList;

namespace NewsPortalRazor.Pages.Viewer
{
    public class IndexModel : PageModel
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private const int PageSize = 6;

        public IPagedList<Article> Articles { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tag> PopularTags { get; set; }

        [BindProperty(SupportsGet = true)]
        public FilterModel Filter { get; set; } = new();

        public IndexModel(IArticleService articleService, ICategoryService categoryService, ITagService tagService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        public async Task<IActionResult> OnGetAsync(int PageNumber = 1)
        {
            // Đảm bảo StartDate <= EndDate
            if (Filter.StartDate.HasValue && Filter.EndDate.HasValue && Filter.StartDate > Filter.EndDate)
            {
                Filter.StartDate = null;
                Filter.EndDate = null;
            }

            // Lấy danh mục và tags phổ biến
            Categories = await _categoryService.GetActiveCategories();
            PopularTags = await _tagService.GetPopularTagsAsync(6);

            // Lọc bài viết theo bộ lọc
            Articles = await _articleService.GetPagedArticlesAsync(
                PageNumber,
                PageSize,
                Filter.SearchQuery,
                Filter.SelectedCategoryId,
                Filter.DateRange,
                Filter.StartDate,
                Filter.EndDate,
                Filter.SelectedTags
            );

            return Page();
        }
    }

    public class FilterModel
    {
        public string? SearchQuery { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string? DateRange { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int>? SelectedTags { get; set; }
    }
}
