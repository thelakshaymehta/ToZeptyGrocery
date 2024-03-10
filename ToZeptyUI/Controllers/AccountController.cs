using CaptchaMvc.HtmlHelpers;
using ToZeptyUI.Models;
using ToZeptyDAL;
using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Service;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class AccountController : Controller
    {

        private readonly IAdminRepository adminRepository;
        private readonly ICustomerRepository customerRepository;

        public AccountController(IAdminRepository adminRepository, ICustomerRepository customerRepository)
        {
            this.adminRepository = adminRepository;
            this.customerRepository = customerRepository;
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }


        public ActionResult AdminResetPassword()
        {
            return View();
        }
        // GET: Account
        public ActionResult CustomerLogin()
        {

            return View();
        }

        public ActionResult AdminLogin()
        {


            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(LoginViewModel loginView)
        {
            var isAdmin = Authentication.VerifyAdminCredentials(loginView.UserName, loginView.Password);

            if (!this.IsCaptchaValid("Validate your captcha"))
            {
                ViewBag.ErrMessage = "Incorrect captcha entered";
                return View("AdminLogin", loginView);
            }
            if (isAdmin)
            {
                var user = adminRepository.GetAdminByUserName(loginView.UserName);
                Session["UserId"] = user.Id;
                Session["UserName"] = user.UserName;
                FormsAuthentication.SetAuthCookie(loginView.UserName, false);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // If authentication fails, you may want to show an error message.
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(loginView);
            }
        }
        [HttpPost]
        public ActionResult CustomerLogin(LoginViewModel loginView)
        {
            var isAdmin = Authentication.VerifyCustomerCredentials(loginView.UserName, loginView.Password);


            if (!this.IsCaptchaValid("Validate your captcha"))
            {
                ViewBag.ErrMessage = "Incorrect captcha entered";
                return View("CustomerLogin", loginView);
            }
            if (isAdmin)
            {
                var user = customerRepository.GetCustomerByUserName(loginView.UserName);
                Session["UserId"] = user.Id;
                Session["UserName"] = user.UserName;
                FormsAuthentication.SetAuthCookie(loginView.UserName, false);
                return RedirectToAction("DashBoard", "Customer");
            }
            else
            {
                // If authentication fails, you may want to show an error message.
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(loginView);
            }
        }

        [HttpPost]
        public ActionResult Registration(UserView user)
        {

            if (!this.IsCaptchaValid("Validate your captcha"))
            {
                ViewBag.ErrMessage = "Incorrect captcha entered";
                return View("Registration", user);
            }
            if (user.UserType == 1)
            {
                if (adminRepository.AdminExistsEmail(user.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered with us.");
                }
                if (adminRepository.AdminExists(user.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already registered with us.");
                }
            }
            else
            {
                if (customerRepository.CustomerExistsEmail(user.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered with us.");
                }
                if (customerRepository.CustomerExists(user.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already registered with us.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Return the view with validation errors
                return View("Registration", user);
            }

            if (user.UserType == 2)
            {
                Customer customer = new Customer
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.UserType
                };
                var passwordHash = new PasswordHasher<Customer>();
                customer.Password = passwordHash.HashPassword(customer, user.Password);
                customerRepository.CreateCustomer(customer);
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                Admin newadmin = new Admin
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.UserType
                };
                var passwordHash = new PasswordHasher<Admin>();
                newadmin.Password = passwordHash.HashPassword(newadmin, user.Password);
                adminRepository.CreateAdmin(newadmin);

                return RedirectToAction("Index", "Product");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = customerRepository.GetCustomerByUserNamePhone(model.UserName, model.PhoneNumber);


                if (!this.IsCaptchaValid("Validate your captcha"))
                {
                    ViewBag.ErrMessage = "Incorrect captcha entered";
                    return View("ResetPassword", model);
                }
                if (user == null)
                {
                    ModelState.AddModelError(nameof(model.UserName), "Invalid username.");
                    ModelState.AddModelError(nameof(model.PhoneNumber), "Invalid Phone Number.");
                    return View(model);
                }
                else
                {
                    var passwordHash = new PasswordHasher<Customer>();
                    user.Password = passwordHash.HashPassword(user, model.Password);
                    customerRepository.customerSAveChanges();
                }
                //TempData["SuccessMessage"] = "Password reset successfully. Please log in with your new password.";
                return RedirectToAction("CustomerLogin", "Account");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = adminRepository.GetAdminByUserNamePhone(model.UserName, model.PhoneNumber);
                if (!this.IsCaptchaValid("Validate your captcha"))
                {
                    ViewBag.ErrMessage = "Incorrect captcha entered";
                    return View("AdminResetPassword", model);
                }

                if (user == null)
                {
                    ModelState.AddModelError(nameof(model.UserName), "Invalid username. Please enter a valid username.");
                    return View(model);
                }
                else
                {
                    var passwordHash = new PasswordHasher<Admin>();
                    user.Password = passwordHash.HashPassword(user, model.Password);
                    adminRepository.SaveAdminchages();
                }
                TempData["SuccessMessage"] = "Password reset successfully. Please log in with your new password.";
                return RedirectToAction("CustomerLogin", "Account");
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();

            FormsAuthentication.SignOut();
            ViewBag.IsLoggedOut = "true";
            return RedirectToAction("CustomerLogin", "Account");


            //return View("Logout");

        }

    }
}