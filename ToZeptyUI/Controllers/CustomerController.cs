using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ToZeptyDAL;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using ToZeptyUI.Models;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IAddressRepository addressRepository;
        private readonly ICategory _categoryRepository;

        public CustomerController(
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IAddressRepository addressRepository,
            ICategory category
        )
        {
            this.customerRepository = customerRepository;
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
            this.orderRepository = orderRepository;
            this.addressRepository = addressRepository;
            this._categoryRepository = category;
        }

        // GET: Customer
        public ActionResult Index()
        {
            var products = productRepository.GetAllProducts();
            var productViewModels = products.Select(MapToViewModel).ToList();
            return View(productViewModels);
        }

        public ActionResult DashBoard()
        {
            return View();
        }

        private ProductViewModel MapToViewModel(Product product)
        {
            IEnumerable<Category> categories = _categoryRepository.GetAllCategories();
            string categoryName = categories
                .FirstOrDefault(c => c.CategoryId == product.CategoryId)
                ?.CategoryName;
            return new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                CategoryName = categoryName,
                CategoryId = product.CategoryId,
            };
        }

        public ActionResult AddToCart2(int productId)
        {
            var userId = Session["UserId"];
            if (userId == null)
            {
                ModelState.AddModelError("errorMessage", "User session not found. Please log in.");
                return RedirectToAction("Index", "Customer");
            }

            var cart = productRepository.GetProductById(productId);

            if (cart == null)
            {
                ModelState.AddModelError("errorMessage", "Product not found.");
                return RedirectToAction("Index", "Customer");
            }

            var addTocart = new Cart
            {
                ProductName = cart.Name,
                Quantity = 1,
                ImageFileName = cart.ImageFileName,
                Price = cart.Price,
                CusomerId = Convert.ToInt32(userId)
            };
            if (ModelState.IsValid)
            {
                cartRepository.CreateCartItem(addTocart);

                return RedirectToAction("Index", "Customer");
            }
            return RedirectToAction("Index", "Customer");
        }

        public ActionResult AddToCart(int productId)
        {
            var userId = Session["UserId"] as int?;
            if (userId == null)
            {
                ModelState.AddModelError("errorMessage", "User session not found. Please log in.");
                return RedirectToAction("Index", "Customer");
            }
            int temp = Convert.ToInt32(userId);
            var existingCartItem = customerRepository.GetCartItemByProductIdAndCustomerId(
                productId,
                temp
            );

            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
                cartRepository.cartSaveChanges();
            }
            else
            {
                var cart = productRepository.GetProductById(productId);

                if (cart == null)
                {
                    ModelState.AddModelError("errorMessage", "Product not found.");
                    return RedirectToAction("Index", "Customer");
                }

                var addTocart = new Cart
                {
                    ProductName = cart.Name,
                    Quantity = 1,
                    ImageFileName = cart.ImageFileName,
                    Price = cart.Price,
                    CusomerId = Convert.ToInt32(userId),
                    ProductId = productId
                };

                cartRepository.CreateCartItem(addTocart);
            }

            return RedirectToAction("Index", "Customer");
        }

        public ActionResult ViewCart(int? customerId)
        {
            if (customerId == null)
            {
                return RedirectToAction("CustomerLogin", "Account");
            }
            var loggedInUserId = (int?)Session["UserId"];

            var cartItems = cartRepository.GetCartItemsByCustomerId(customerId);
            var carViewList = cartItems.Select(item => new CartViewModel
            {
                CartId = item.CartId,
                ImageFileName = item.ImageFileName,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price,
                CustomerId = item.CusomerId
            });

            return View(carViewList);
        }

        [HttpPost]
        public ActionResult UpdateCartQuantity(int cartId, int newQuantity)
        {
            var cartItem = cartRepository.GetCartItemById(cartId);
            var product = productRepository.GetProductById(cartItem.ProductId);
            if (product.ProductQuantity < newQuantity)
            {
                return Json(
                    new
                    {
                        success = false,
                        errorMessage = $"Only {product.ProductQuantity} available."
                    }
                );
            }

            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;

                cartRepository.cartSaveChanges();
                var subtotal = cartItem.Quantity * cartItem.Price;
                return Json(new { success = true, subtotal });
            }
            return Json(new { success = true });
        }

        public ActionResult PlaceOrder(int? customerId)
        {
            if (customerId == null)
            {
                var existingOrder = Session["OrderViewModel"] as OrderViewModel;
                return View(existingOrder);
            }
            else
            {
                var cartItems2 = cartRepository.GetCartItemsByCustomerId(customerId);
                int[] cartIds1 = cartItems2.Select(item => item.CartId).ToArray();
                int custId = Convert.ToInt32(customerId);
                var cartItems = cartRepository.GetCartItemById(cartIds1);
                var customer = customerRepository.GetCustomerById(custId);
                var UserAddress = addressRepository.GetAddressesByUserId(custId);
                List<AddressViewModel> addressModel = null;

                if (UserAddress != null && UserAddress.Any())
                {
                    addressModel = UserAddress
                        .Select(address => new AddressViewModel
                        {
                            Id = address.Id,
                            Street = address.Street,
                            City = address.City,
                            PostalCode = address.PostalCode,
                            State = address.State,
                            Country = address.Country,
                        })
                        .ToList();
                }
                var order = new OrderViewModel
                {
                    name = customer.UserName,
                    CustId = customer.Id,
                    UserName = customer.UserName,
                    OrderDate = DateTime.Now,
                    Addresses = addressModel,
                    OrderDetails = cartItems
                        .Select(item => new OrderDetailsViewModel
                        {
                            ProductName = item.ProductName,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Subtotal = item.Quantity * item.Price,
                            ProductId = item.ProductId,
                        })
                        .ToList()
                };
                decimal total = cartItems.Sum(item => item.Quantity * item.Price);
                order.TotalAmount = total;
                Session["OrderViewModel"] = order;

                return View(order);
            }
        }

        [HttpPost]
        public ActionResult OrderConfirm(int customerId)
        {
            var cartItems = cartRepository.GetCartItemsByCustomerId(customerId);
            var customer = customerRepository.GetCustomerById(customerId);

            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = customer.Id,
                TotalAmount = cartItems.Sum(item => item.Quantity * item.Price),
                OrderDetails = cartItems
                    .Select(item => new OrderDetail
                    {
                        ProductName = item.ProductName,
                        price = item.Price,
                        Quantity = item.Quantity
                    })
                    .ToList()
            };

            orderRepository.CreateOrder(order);

            foreach (var cartItem in cartItems)
            {
                cartRepository.DeleteCartItem(cartItem.CartId);
                var ProductItem = productRepository.GetProductById(cartItem.ProductId);
                if (cartItem.Quantity == ProductItem.ProductQuantity)
                    productRepository.DeleteProduct(cartItem.ProductId);
                else
                {
                    int newQuantity =
                        Convert.ToInt32(ProductItem.ProductQuantity)
                        - Convert.ToInt32(cartItem.Quantity);
                    ProductItem.ProductQuantity = newQuantity;
                    productRepository.SaveProductChanges();
                }
            }

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }

        public ActionResult OrderConfirmation(int orderId)
        {
            var order = orderRepository.GetOrderById(orderId);
            var message = new OrderViewModel
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
            };

            return View(message);
        }

        public ActionResult OrderPlacedSuccessfully()
        {
            return View();
        }

        public ActionResult ViewOrderedProducts()
        {
            int userId = (int)Session["UserId"];
            var orders = orderRepository.GetOrdersByCustomerId(userId);
            var productViewModels = orders
                .SelectMany(od =>
                    od.OrderDetails.Select(detail => new OrderDetailsViewModel
                    {
                        OrderId = detail.OrderId,
                        OrderDate = detail.Order.OrderDate,
                        ProductId = detail.OrderId,
                        ProductName = detail.ProductName,
                        Quantity = detail.Quantity,
                        Price = detail.price
                    })
                )
                .ToList();
            return View(productViewModels);
        }

        public ActionResult ViewProfile(int customerId = 1)
        {
            CustomerModel viewProfile = GetCustomerProfile(customerId);
            return View(viewProfile);
        }

        public ActionResult EditCustomer(int customerId)
        {
            Customer customer = customerRepository.GetCustomerById(customerId);

            var customerView = new EditUserView
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                UserName = customer.UserName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
            };
            return View(customerView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(EditUserView customer)
        {
            if (customerRepository.CustomerExistsEmail(customer.Email, customer.Id))
            {
                ModelState.AddModelError("Email", "Email already registered with us.");
            }
            if (!ModelState.IsValid)
            {
                return View("EditCustomer", customer);
            }
            else
            {
                Customer ToUpdateProfile = customerRepository.GetCustomerById(customer.Id);
                if (ToUpdateProfile != null)
                {
                    ToUpdateProfile.FirstName = customer.FirstName;
                    ToUpdateProfile.LastName = customer.LastName;
                    ToUpdateProfile.Email = customer.Email;
                    ToUpdateProfile.PhoneNumber = customer.PhoneNumber;

                    customerRepository.customerSAveChanges();
                }
                return RedirectToAction(
                    "ViewProfile",
                    "Customer",
                    new { customerId = ToUpdateProfile.Id }
                );
            }
        }

        public CustomerModel GetCustomerProfile(int customerId = 1)
        {
            var customerProfile = customerRepository.GetCustomerById(customerId);
            var viewProfile = new CustomerModel
            {
                Id = customerProfile.Id,
                FirstName = customerProfile.FirstName,
                LastName = customerProfile.LastName,
                UserName = customerProfile.UserName,
                Email = customerProfile.Email,
                PhoneNumber = customerProfile.PhoneNumber,
            };
            return viewProfile;
        }

        [HttpPost]
        public ActionResult DeleteOrder(int orderId)
        {
            var order = orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("ViewOrderedProducts");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            if (order.CustomerId != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this order.";
                return RedirectToAction("ViewOrderedProducts");
            }

            orderRepository.DeleteOrder(order);

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("ViewOrderedProducts");
        }

        [HttpPost]
        public ActionResult RemoveCartItem(int cartId)
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "User not logged in.";
                return RedirectToAction("ViewCart");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            var cartItem = cartRepository.GetCartItemByCartIdAndCustomerId(cartId, userId);

            if (cartItem == null)
            {
                TempData["ErrorMessage"] = "Cart item not found.";
                return RedirectToAction("ViewCart", "Customer");
            }

            cartRepository.DeleteCartItem(cartItem.CartId);

            return RedirectToAction("ViewCart", "Customer", new { customerId = userId });
        }
    }
}
