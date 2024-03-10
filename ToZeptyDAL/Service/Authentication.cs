using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ToZeptyDAL.Data;

namespace ToZeptyDAL.Service
{
    public static class Authentication
    {
        private static readonly ZeptyDbContext context = new ZeptyDbContext();

        public static bool VerifyAdminCredentials(string username, string password)
        {
            var admin = context.Admins.FirstOrDefault(a => a.UserName == username);

            if (password == null)
            {
                return false;
            }
            if (username == null)
            {
                return false;
            }

            if (admin != null)
            {
                var passwordHasher = new PasswordHasher<Admin>();
                var result = passwordHasher.VerifyHashedPassword(admin, admin.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool VerifyCustomerCredentials(string username, string password)
        {
            var customer = context.Customers.FirstOrDefault(a => a.UserName == username);

            if (customer != null)
            {
                var passwordHasher = new PasswordHasher<Customer>();
                var result = passwordHasher.VerifyHashedPassword(
                    customer,
                    customer.Password,
                    password
                );

                if (result == PasswordVerificationResult.Success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
