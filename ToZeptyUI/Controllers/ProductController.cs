using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToZeptyDAL;
using ToZeptyDAL.Interface;
using ToZeptyUI.Models;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;
        private readonly ICategory _categoryRepository;

        public ProductController(
            IProductRepository productRepository,
            ICartRepository cartRepository,
            ICategory categoryRepository
        )
        {
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
            this._categoryRepository = categoryRepository;
        }

        public ActionResult Index()
        {
            var products = productRepository.GetAllProducts();
            if (products == null)
            {
                Console.WriteLine("products are nnull");
            }
            Console.WriteLine("testing the products " + products);
            var productViewModels = products.Select(MapToViewModel).ToList();

            return View(productViewModels);
        }

        public ActionResult AddProduct()
        {
            var categories = _categoryRepository.GetAllCategories();

            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            });
            var viewModel = new ProductViewModel
            {
                Categories = categoryList,
                CategoryId = categories.FirstOrDefault()?.CategoryId ?? 0
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string imagePath = Server.MapPath("~/Images/");
                    string uniqueFileName =
                        Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(imagePath, uniqueFileName);
                    imageFile.SaveAs(filePath);

                    model.ImageFileName = uniqueFileName;
                }

                Product newProduct = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    ImageFileName = model.ImageFileName,
                    ProductQuantity = model.ProductQuantity,
                    CategoryId = model.CategoryId,
                };

                productRepository.CreateProduct(newProduct);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        private ProductViewModel MapToViewModel(Product product)
        {
            return new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                ProductQuantity = product.ProductQuantity,
            };
        }

        public ActionResult Edit(int id)
        {
            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return View();
            }
            var categories = _categoryRepository.GetAllCategories();
            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            });
            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                ProductQuantity = product.ProductQuantity,
                Categories = categoryList,
                CategoryId = product.CategoryId
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = productRepository.GetProductById(viewModel.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found.");
                    return View(viewModel);
                }

                product.Name = viewModel.Name;
                product.Description = viewModel.Description;
                product.Price = viewModel.Price;
                product.ProductQuantity = viewModel.ProductQuantity;
                product.CategoryId = viewModel.CategoryId;

                if (viewModel.ImageFile != null && viewModel.ImageFile.ContentLength > 0)
                {
                    string imagePath = Path.Combine(
                        Server.MapPath("~/Images"),
                        product.ImageFileName
                    );

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    string uniqueFileName =
                        Guid.NewGuid().ToString()
                        + "_"
                        + Path.GetFileName(viewModel.ImageFile.FileName);
                    string newImagePath = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                    viewModel.ImageFile.SaveAs(newImagePath);

                    product.ImageFileName = uniqueFileName;
                }

                try
                {
                    productRepository.SaveProductChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "An error occurred while processing your request. Please try again later."
                    );
                }
            }

            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return RedirectToAction("Index");
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                ProductQuantity = product.ProductQuantity,
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return RedirectToAction("Index");
            }

            DeleteImage(product.ImageFileName);
            productRepository.DeleteProduct(product.ProductId);

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }

        private void DeleteImage(string imageFileName)
        {
            if (!string.IsNullOrEmpty(imageFileName))
            {
                string imagePath = Path.Combine(Server.MapPath("~/Images"), imageFileName);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
        }
    }
}
