using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToZeptyUI.Models;
using ToZeptyDAL;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using ToZeptyDAL.Service;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ICustomerRepository customerRepository;
        private readonly OrderRepository orderRepository;
        private readonly IAdminRepository adminRepository;

        // GET: Admin
        public AdminController(
            ICustomerRepository customerRepository,
            OrderRepository orderRepository,
            IAdminRepository adminRepository
        )
        {
            this.customerRepository = customerRepository;
            this.orderRepository = orderRepository;
            this.adminRepository = adminRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCustomers()
        {
            var customers = customerRepository.GetAllCustomers();
            var customerViewModels = customers.Select(MapToViewModel).ToList();
            return View(customerViewModels);
        }

        public ActionResult ViewOrderedProducts(int customerId)
        {
            int userId = (int)Session["UserId"];
            var orders = orderRepository.GetOrdersByCustomerId(customerId);
            var productViewModels = orders
                .SelectMany(od =>
                    od.OrderDetails.Select(detail => new OrderDetailsViewModel
                    {
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

        public ActionResult ViewMostOrderedProducts()
        {
            int userId = (int)Session["UserId"];
            var allOrders = orderRepository.GetAllOrders();

            var productViewModels = allOrders
                .SelectMany(order =>
                    order.OrderDetails.Select(orderDetail => new OrderDetailsViewModel
                    {
                        OrderDate = orderDetail.Order.OrderDate,
                        ProductId = orderDetail.OrderId,
                        ProductName = orderDetail.ProductName,
                        Quantity = orderDetail.Quantity,
                        Price = orderDetail.price
                    })
                )
                .GroupBy(product => new { product.ProductId, product.ProductName })
                .Select(productGroup => new OrderDetailsViewModel
                {
                    ProductId = productGroup.Key.ProductId,
                    ProductName = productGroup.Key.ProductName,
                    Quantity = productGroup.Sum(product => product.Quantity),
                    Price = productGroup.FirstOrDefault().Price
                })
                .OrderByDescending(product => product.Quantity)
                .Take(5)
                .ToList();
            return View(productViewModels);
        }

        public ActionResult ViewCustomerDetails(int customerId)
        {
            var customer = customerRepository.GetCustomerById(customerId);
            var customerModel = MapToViewModel(customer);

            return View(customerModel);
        }

        public ActionResult ViewProfile(int AdminId)
        {
            ActionResult result = GetAdminProfile(AdminId);
            return result;
        }

        public ActionResult GetAdminProfile(int AdminId)
        {
            int userId = (int)Session["UserId"];

            if (AdminId != userId)
            {
                return RedirectToAction("UnautzorizedAccess", "Errorr");
            }
            else
            {
                var adminProfile = adminRepository.GetAdminById(AdminId);

                var viewProfile = new UserView
                {
                    Id = adminProfile.Id,
                    FirstName = adminProfile.FirstName,
                    LastName = adminProfile.LastName,
                    UserName = adminProfile.UserName,
                    Email = adminProfile.Email,
                    PhoneNumber = adminProfile.PhoneNumber,
                };

                return View(viewProfile);
            }
        }

        public ActionResult EditAdmin(int AdminId)
        {
            Admin receivedAdminInfo = adminRepository.GetAdminById(AdminId);
            var sendAdminInfo = new EditUserView
            {
                Id = receivedAdminInfo.Id,
                FirstName = receivedAdminInfo.FirstName,
                LastName = receivedAdminInfo.LastName,
                UserName = receivedAdminInfo.UserName,
                Email = receivedAdminInfo.Email,
                PhoneNumber = receivedAdminInfo.PhoneNumber,
            };
            return View(sendAdminInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdmin(EditUserView User)
        {
            if (adminRepository.AdminExistsEmail(User.Email, User.Id))
            {

                ModelState.AddModelError("Email", "Email already registered with us.");
            }
            if (!ModelState.IsValid)
            {

                return View("EditAdmin", User);
            }

            Admin ToUpdateProfile = adminRepository.GetAdminById(User.Id);
            if (ToUpdateProfile != null)
            {
                ToUpdateProfile.FirstName = User.FirstName;
                ToUpdateProfile.LastName = User.LastName;
                ToUpdateProfile.Email = User.Email;
                ToUpdateProfile.PhoneNumber = User.PhoneNumber;
                adminRepository.SaveAdminchages();
            }

            return RedirectToAction("ViewProfile", "Admin", new { AdminId = ToUpdateProfile.Id });
        }

        private CustomerModel MapToViewModel(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                UserName = customer.UserName,
                Password = customer.Password,
                ConfirmPassword = customer.Password
            };
        }
    }
}
