using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly NewsPortalContext _context;

        public CategoryRepository(NewsPortalContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetActiveCategories()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.IsActive)
                .ToListAsync();
        }
    }
}
