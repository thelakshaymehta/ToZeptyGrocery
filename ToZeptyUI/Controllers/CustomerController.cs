using ToZeptyUI.Models;
using ToZeptyDAL;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        //  private readonly ZeptyDbContext _context;
        private readonly ICustomerRepository customerRepository;
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IAddressRepository addressRepository;
        private readonly ICategory _categoryRepository;

        public CustomerController(ICustomerRepository customerRepository, IProductRepository productRepository, ICartRepository cartRepository, IOrderRepository orderRepository, IAddressRepository addressRepository
            , ICategory category)
        {
            //   _context = new ZeptyDbContext();
            this.customerRepository = customerRepository;
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
            this.orderRepository = orderRepository;
            this.addressRepository = addressRepository;
            this._categoryRepository = category;
        }


        //public ActionResult Index()
        //{
        //   return View();
        //}

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
            string categoryName = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId)?.CategoryName;
            return new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                CategoryName = categoryName,
                CategoryId = product.CategoryId,
                // Add other properties as needed
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
                // Add the item to the database or perform other business logic
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
            var existingCartItem = customerRepository.GetCartItemByProductIdAndCustomerId(productId, temp);

            if (existingCartItem != null)
            {
                // Product already exists in the cart, so update the quantity
                existingCartItem.Quantity++;
                cartRepository.cartSaveChanges();
            }
            else
            {
                // Product is not in the cart, so add a new item
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
                    ProductId = productId  // Add the ProductId property to your Cart model to store the product ID
                };

                cartRepository.CreateCartItem(addTocart);

            }


            return RedirectToAction("Index", "Customer");
        }

        public ActionResult ViewCart(int? customerId)
        {
            // Check if customerId is provided and is a valid value
            if (customerId == null)
            {
                // You might want to redirect to a login page or show an error message
                return RedirectToAction("CustomerLogin", "Account");
            }
            var loggedInUserId = (int?)Session["UserId"];
            // Retrieve cart items for the specified customer

            var cartItems = cartRepository.GetCartItemsByCustomerId(customerId);
            // Create a list of CartViewModel to pass to the view
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
                return Json(new { success = false, errorMessage = $"Only {product.ProductQuantity} available." });
            }

            if (cartItem != null)
            {
                // Update the quantity
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
                    // User has addresses, create the addressModel
                    addressModel = UserAddress.Select(address => new AddressViewModel
                    {
                        Id = address.Id,
                        Street = address.Street,
                        City = address.City,
                        PostalCode = address.PostalCode,
                        State = address.State,
                        Country = address.Country,
                    }).ToList();
                }
                var order = new OrderViewModel
                {
                    name = customer.UserName,
                    CustId = customer.Id,
                    UserName = customer.UserName,
                    OrderDate = DateTime.Now,
                    Addresses = addressModel,
                    OrderDetails = cartItems.Select(item => new OrderDetailsViewModel
                    {

                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Subtotal = item.Quantity * item.Price,
                        ProductId = item.ProductId,
                    }).ToList()
                };
                decimal total = cartItems.Sum(item => item.Quantity * item.Price);
                order.TotalAmount = total;
                Session["OrderViewModel"] = order;

                // Other logic (e.g., payment processing) can be added here

                return View(order);
            }
        }

        [HttpPost]
        public ActionResult OrderConfirm(int customerId)
        {
            // Retrieve cart items
            var cartItems = cartRepository.GetCartItemsByCustomerId(customerId);
            var customer = customerRepository.GetCustomerById(customerId);

            // Create an order
            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = customer.Id,
                TotalAmount = cartItems.Sum(item => item.Quantity * item.Price),
                OrderDetails = cartItems.Select(item => new OrderDetail
                {
                    ProductName = item.ProductName,
                    price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };

            // Save the order
            orderRepository.CreateOrder(order);

            // Remove cart items
            foreach (var cartItem in cartItems)
            {
                cartRepository.DeleteCartItem(cartItem.CartId);
                var ProductItem = productRepository.GetProductById(cartItem.ProductId);
                if (cartItem.Quantity == ProductItem.ProductQuantity)
                    productRepository.DeleteProduct(cartItem.ProductId);
                else
                {
                    int newQuantity = Convert.ToInt32(ProductItem.ProductQuantity) - Convert.ToInt32(cartItem.Quantity);
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
           .SelectMany(od => od.OrderDetails.Select(detail => new OrderDetailsViewModel
           {
               OrderId = detail.OrderId,
               OrderDate = detail.Order.OrderDate,
               ProductId = detail.OrderId,
               ProductName = detail.ProductName,
               Quantity = detail.Quantity,
               Price = detail.price
           }))
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
                // Email already registered
                ModelState.AddModelError("Email", "Email already registered with us.");
            }
            if (!ModelState.IsValid)
            {
                // Return the same view with validation errors
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
                return RedirectToAction("ViewProfile", "Customer", new { customerId = ToUpdateProfile.Id });
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
            // Check if the order exists
            var order = orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("ViewOrderedProducts");
            }

            // Check if the logged-in user is authorized to delete this order
            int userId = Convert.ToInt32(Session["UserId"]);
            if (order.CustomerId != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this order.";
                return RedirectToAction("ViewOrderedProducts");
            }

            // Delete the order
            orderRepository.DeleteOrder(order);

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("ViewOrderedProducts");
        }




        [HttpPost]
        public ActionResult RemoveCartItem(int cartId)
        {
            // Ensure the user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "User not logged in.";
                return RedirectToAction("ViewCart");
            }

            // Retrieve the user's ID from the session
            int userId = Convert.ToInt32(Session["UserId"]);

            // Find the cart item for the specified cartId and userId
            var cartItem = cartRepository.GetCartItemByCartIdAndCustomerId(cartId, userId);

            if (cartItem == null)
            {
                TempData["ErrorMessage"] = "Cart item not found.";
                return RedirectToAction("ViewCart", "Customer");
            }

            // Remove the cart item
            cartRepository.DeleteCartItem(cartItem.CartId);

            // Save changes and check for errors


            return RedirectToAction("ViewCart", "Customer", new { customerId = userId });
        }
    }



}