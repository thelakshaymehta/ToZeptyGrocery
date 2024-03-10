using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using ToZeptyDAL;
using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using ToZeptyUI.Controllers;
using ToZeptyUI.Models;

namespace test
{
    [TestFixture]
    public class ControllerTests
    {
        [Test]
        public void MapToViewModel_Should_Map_Customer_To_CustomerModel()
        {
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Manjula",
                LastName = "Nayak",
                Email = "Manjula@gmail.com",
                PhoneNumber = "7498472506",
                UserName = "ManjulaNayak",
                Password = "Manjula@123"
            };

            var homeController = new HomeController();

            var result = homeController.MapToViewModel(customer);

            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf<CustomerModel>(result);
            ClassicAssert.AreEqual(customer.Id, result.Id);
            ClassicAssert.AreEqual(customer.FirstName, result.FirstName);
            ClassicAssert.AreEqual(customer.LastName, result.LastName);
            ClassicAssert.AreEqual(customer.Email, result.Email);
            ClassicAssert.AreEqual(customer.PhoneNumber, result.PhoneNumber);
            ClassicAssert.AreEqual(customer.UserName, result.UserName);
            ClassicAssert.AreEqual(customer.Password, result.Password);
        }

        [Test]
        public void EditCustomer_WithValidCustomerModel_RedirectsToViewProfile()
        {
            int customerId = 1;
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var controller = new CustomerController(
                customerRepositoryMock.Object,
                Mock.Of<IProductRepository>(),
                Mock.Of<ICartRepository>(),
                Mock.Of<IOrderRepository>(),
                Mock.Of<IAddressRepository>(),
                Mock.Of<ICategory>()
            );

            var validCustomerModel = new EditUserView
            {
                Id = customerId,
                FirstName = "Updated",
                LastName = "User",
                Email = "updated.user@example.com",
                PhoneNumber = "987-654-3210",
                UserName = "updated.user"
            };

            var existingCustomer = new Customer
            {
                Id = customerId,
                FirstName = "Etukuri",
                LastName = "Dheeraj",
                Email = "Dheeraj@gmail.com",
                PhoneNumber = "6369087645",
                UserName = "EtukuriDheeraj"
            };

            customerRepositoryMock
                .Setup(repo => repo.GetCustomerById(customerId))
                .Returns(existingCustomer);

            var result = controller.EditCustomer(validCustomerModel) as RedirectToRouteResult;

            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("ViewProfile", result.RouteValues["action"]);
            ClassicAssert.AreEqual("Customer", result.RouteValues["controller"]);

            customerRepositoryMock.Verify(repo => repo.GetCustomerById(customerId), Times.Once);
            customerRepositoryMock.Verify(repo => repo.customerSAveChanges(), Times.Once);
        }

        [Test]
        public void GetCustomerProfile_ValidCustomerId_ReturnsCustomerModel()
        {
            int customerId = 1;
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var addressRepositoryMock = new Mock<IAddressRepository>();

            var controller = new CustomerController(
                customerRepositoryMock.Object,
                Mock.Of<IProductRepository>(),
                Mock.Of<ICartRepository>(),
                Mock.Of<IOrderRepository>(),
                addressRepositoryMock.Object,
                Mock.Of<ICategory>()
            );

            var existingCustomer = new Customer
            {
                Id = customerId,
                FirstName = "Aadharsh",
                LastName = "Reddy",
                UserName = "AadharshReddy",
                Email = "Aadharsh@gmail.com",
                PhoneNumber = "123-456-7890"
            };

            customerRepositoryMock
                .Setup(repo => repo.GetCustomerById(customerId))
                .Returns(existingCustomer);

            var result = controller.GetCustomerProfile(customerId);

            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(customerId, result.Id);
            ClassicAssert.AreEqual(existingCustomer.FirstName, result.FirstName);
            ClassicAssert.AreEqual(existingCustomer.LastName, result.LastName);
            ClassicAssert.AreEqual(existingCustomer.UserName, result.UserName);
            ClassicAssert.AreEqual(existingCustomer.Email, result.Email);
            ClassicAssert.AreEqual(existingCustomer.PhoneNumber, result.PhoneNumber);

            customerRepositoryMock.Verify(repo => repo.GetCustomerById(customerId), Times.Once);
            ClassicAssert.AreEqual(0, addressRepositoryMock.Invocations.Count);
        }

        [Test]
        public void Edit_ValidProductId_ReturnsViewWithProductDetails()
        {
            int productId = 1;
            var productRepositoryMock = new Mock<IProductRepository>();
            var cartRepositoryMock = new Mock<ICartRepository>();
            var CategoryMock = new Mock<ICategory>();

            var productViewModel = new ProductViewModel { };
            productRepositoryMock
                .Setup(repo => repo.GetProductById(productId))
                .Returns(new Product { ProductId = productId, });

            var controller = new ProductController(
                productRepositoryMock.Object,
                cartRepositoryMock.Object,
                CategoryMock.Object
            );

            var result = controller.Edit(productId) as ViewResult;
            var model = result?.Model as ProductViewModel;

            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(model);
        }

        [Test]
        public void DeleteConfirmed_ValidProductId_DeletesProductAndRedirectsToIndex()
        {
            int productId = 1;
            var productRepositoryMock = new Mock<IProductRepository>();
            var cartRepositoryMock = new Mock<ICartRepository>();
            var CategoryMock = new Mock<ICategory>();
            var controller = new ProductController(
                productRepositoryMock.Object,
                cartRepositoryMock.Object,
                CategoryMock.Object
            );

            var productViewModel = new ProductViewModel { };
            productRepositoryMock
                .Setup(repo => repo.GetProductById(productId))
                .Returns(new Product { ProductId = productId, });

            var result = controller.DeleteConfirmed(productId) as RedirectToRouteResult;

            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);

            productRepositoryMock.Verify(repo => repo.DeleteProduct(productId), Times.Once);
        }

        [Test]
        public void EditAdmin_WithValidUserView_RedirectsToViewProfile()
        {
            int adminId = 3;
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var controller = new AdminController(null, null, adminRepositoryMock.Object);

            var userView = new EditUserView
            {
                Id = adminId,
                FirstName = "Kudithaji",
                LastName = "Rajinikanth",
                Email = "rajinikanth@gmail.com",
                PhoneNumber = "1234567890",
                UserName = "KudithajiRajiniKanth"
            };

            var existingAdmin = new Admin
            {
                Id = adminId,
                FirstName = "Alim",
                LastName = "Khan",
                UserName = "AlimKhan",
                Password = "Alim@123",
                PhoneNumber = "7835422132",
                Email = "alimkhan@gmail.com",
                RoleId = 2
            };

            adminRepositoryMock.Setup(repo => repo.GetAdminById(adminId)).Returns(existingAdmin);

            var result = controller.EditAdmin(userView) as RedirectToRouteResult;

            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("ViewProfile", result.RouteValues["action"]);
            ClassicAssert.AreEqual("Admin", result.RouteValues["controller"]);
            ClassicAssert.AreEqual(adminId, result.RouteValues["AdminId"]);

            adminRepositoryMock.Verify(repo => repo.SaveAdminchages(), Times.Once);
        }

    }
}
