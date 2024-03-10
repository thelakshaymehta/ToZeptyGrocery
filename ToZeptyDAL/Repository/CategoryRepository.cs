using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly ZeptyDbContext _dbContext;
        public CategoryRepository(ZeptyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CategoryExists(string categoryName)
        {
            Category category = _dbContext.Categories.FirstOrDefault(cat => cat.CategoryName == categoryName);
            if (category != null)
                return true;
            return false;
        }

        public bool CategoryExists(string categoryName, int id)
        {
            Category category = _dbContext.Categories.FirstOrDefault(cat => cat.CategoryName == categoryName);
            if (category != null)
            {
                if (category.CategoryId == id) return false;
            }

            return true;

        }

        public void DeleteCategory(Category categoryToDelete)
        {
            _dbContext.Categories.Remove(categoryToDelete);
            _dbContext.SaveChanges();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _dbContext.Categories;
        }

        public Category GetCategoryById(int id)
        {
            Category getCategory = _dbContext.Categories.Find(id);
            return getCategory;
        }

        public int GetLastCategoryId()
        {
            int lastCategoryId = _dbContext.Categories.Max(c => (int?)c.CategoryId) ?? 0;
            return lastCategoryId;
        }

        public int SaveCategory(Category category)
        {
            _dbContext.Categories.Add(category);
            int res = _dbContext.SaveChanges();
            return res;
        }

        public Category UpdateCategory(Category existingCategory)
        {
            Category updatecategory = _dbContext.Categories.Find(existingCategory.CategoryId);
            if (updatecategory != null)
            {
                updatecategory.CategoryName = existingCategory.CategoryName;
                _dbContext.SaveChanges();
            }
            return updatecategory;
        }

    }
}
