using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToZeptyDAL;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using ToZeptyUI.Models;

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
            CategoryModel newCategory = new CategoryModel { CategoryId = nextCategoryId };
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
                    Category categoryToSave = new Category { CategoryName = category.CategoryName };

                    _categoryRepository.SaveCategory(categoryToSave);
                }
                else
                {
                    ModelState.AddModelError(
                        "CategoryName",
                        "Category with this name already exists."
                    );
                    return View(category);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            Category category = _categoryRepository.GetCategoryById(id);

            if (category == null)
            {
                return HttpNotFound();
            }

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
                Category existingCategory = _categoryRepository.GetCategoryById(
                    categoryModel.CategoryId
                );

                if (existingCategory == null)
                {
                    return HttpNotFound();
                }

                if (
                    _categoryRepository.CategoryExists(
                        categoryModel.CategoryName,
                        categoryModel.CategoryId
                    )
                )
                {
                    existingCategory.CategoryName = categoryModel.CategoryName;

                    _categoryRepository.UpdateCategory(existingCategory);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(
                        "CategoryName",
                        "Category with this name already exists."
                    );
                }
            }

            return View("EditCategory", categoryModel);
        }

        public ActionResult DeleteCategory(int id)
        {
            Category categoryToDelete = _categoryRepository.GetCategoryById(id);
            if (categoryToDelete != null)
            {
                _categoryRepository.DeleteCategory(categoryToDelete);
            }

            return RedirectToAction("Index");
        }
    }
}
