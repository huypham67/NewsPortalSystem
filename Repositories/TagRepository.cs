using BusinessObjects.Entities;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly NewsPortalContext _context;

        public TagRepository(NewsPortalContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetPopularTagsAsync(int count)
        {
            return await _context.Tags
                .OrderByDescending(t => t.Articles.Count) // Sắp xếp theo số lượng bài viết
                .Take(count) // Lấy số lượng yêu cầu
                .ToListAsync();
        }
    }
}
