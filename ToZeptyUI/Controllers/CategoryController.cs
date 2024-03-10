using ToZeptyUI.Models;
using ToZeptyDAL;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategory _categoryRepository;

        // GET: Category
        public CategoryController(ICategory category)
        {
            this._categoryRepository = category;
        }
        public ActionResult Index()
        {
            IEnumerable<Category> categories = _categoryRepository.GetAllCategories();
            IEnumerable<CategoryModel> categoriesModel = categories.Select(cat => new CategoryModel
            {
                CategoryId = cat.CategoryId,
                CategoryName = cat.CategoryName,
            });
            return View(categoriesModel);
        }

        public ActionResult AddCategory()
        {
            int nextCategoryId = _categoryRepository.GetLastCategoryId() + 1;
            CategoryModel newCategory = new CategoryModel
            {
                CategoryId = nextCategoryId
            };
            return View(newCategory);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                if (!_categoryRepository.CategoryExists(category.CategoryName))
                {
                    // If it doesn't exist, proceed to add the new category
                    Category categoryToSave = new Category
                    {
                        CategoryName = category.CategoryName
                    };

                    _categoryRepository.SaveCategory(categoryToSave);
                }
                else
                {
                    // Handle the case where the category already exists (e.g., show an error message)
                    ModelState.AddModelError("CategoryName", "Category with this name already exists.");
                    return View(category);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            // Retrieve the category from the repository based on the id
            Category category = _categoryRepository.GetCategoryById(id);

            // Check if the category exists
            if (category == null)
            {
                return HttpNotFound(); // Handle the case where the category is not found
            }

            // Map the Category model to the CategoryModel if needed
            CategoryModel categoryModel = new CategoryModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };

            return View(categoryModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCategory(CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing category from the repository
                Category existingCategory = _categoryRepository.GetCategoryById(categoryModel.CategoryId);

                // Check if the category exists
                if (existingCategory == null)
                {
                    return HttpNotFound(); // Handle the case where the category is not found
                }


                if (_categoryRepository.CategoryExists(categoryModel.CategoryName, categoryModel.CategoryId))
                {
                    // Update the properties of the existing category
                    existingCategory.CategoryName = categoryModel.CategoryName;

                    // Save the changes to the repository
                    _categoryRepository.UpdateCategory(existingCategory);

                    return RedirectToAction("Index"); // Redirect to the category list or another appropriate action
                }
                else
                {
                    // Handle the case where the category already exists (e.g., show an error message)
                    ModelState.AddModelError("CategoryName", "Category with this name already exists.");
                }
            }

            // If the model state is not valid, redisplay the edit view
            return View("EditCategory", categoryModel);
        }

        public ActionResult DeleteCategory(int id)
        {

            Category categoryToDelete = _categoryRepository.GetCategoryById(id);
            if (categoryToDelete != null)
            {
                // Delete the category from the repository
                _categoryRepository.DeleteCategory(categoryToDelete);

                // Save changes to the database
                // Assuming you have a SaveChanges method in your repository
            }
            // Optionally, you can handle the case where the category doesn't exist
            // else
            // {
            //     // Handle the case where the category is not found
            // }
            return RedirectToAction("Index");
        }
    }
}