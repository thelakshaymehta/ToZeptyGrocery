namespace ToZeptyDAL.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNetCore.Identity;

    internal sealed class Configuration
        : DbMigrationsConfiguration<ToZeptyDAL.Data.ZeptyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ToZeptyDAL.Data.ZeptyDbContext context)
        {
            // Add roles if they don't exist
            if (!context.Roles.Any())
            {
                context.Roles.AddOrUpdate(
                    r => r.Name,
                    new Role { Name = "Admin" },
                    new Role { Name = "Customer" }
                );
                context.SaveChanges();
            }

            // Retrieve role IDs
            var adminRole = context.Roles.SingleOrDefault(r => r.Name == "Admin");
            var customerRole = context.Roles.SingleOrDefault(r => r.Name == "Customer");

            // Seeding Data in Admins Table
            if (!context.Admins.Any())
            {
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Admins', RESEED,1 )");

                // Delete existing admins
                var existingAdmins = context.Admins.ToList();
                context.Admins.RemoveRange(existingAdmins);
                context.SaveChanges();

                Admin newadmin = new Admin
                {
                    Email = "thelakshaymehta@gmail.com",
                    UserName = "lakshaymehta",
                    FirstName = "Lakshay",
                    LastName = "Mehta",
                    PhoneNumber = "9253032306",
                    RoleId = adminRole?.RoleId ?? 0 // Use the retrieved RoleId or provide a default value
                };

                var passwordHash = new PasswordHasher<Admin>();
                newadmin.Password = passwordHash.HashPassword(newadmin, "Test@123");
                context.Admins.AddOrUpdate(newadmin);
                context.SaveChanges();
            }

            //---------------------------------------------------------------------------

            // Check if there are no existing customers
            if (!context.Customers.Any())
            {
                // Reset identity column seed to 0
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Customers', RESEED, 1)");

                // Delete existing customers
                var existingCustomers = context.Customers.ToList();
                context.Customers.RemoveRange(existingCustomers);

                Console.WriteLine("Seeding Customers Table");

                // Add 15 dummy customers with the same password
                var dummyPasswordHash = new PasswordHasher<Customer>().HashPassword(null, "Test@123");

                for (int i = 1; i <= 15; i++)
                {
                    Customer dummyCustomer = new Customer
                    {
                        FirstName = $"Dummy{i}",
                        LastName = $"Customer{i}",
                        Email = $"dummy{i}@example.com",
                        PhoneNumber = $"9812{GetRandomDigits(6)}",
                        UserName = $"dummy{i:00}",
                        RoleId = customerRole?.RoleId ?? 0, // Use the retrieved RoleId or provide a default value
                        Password = dummyPasswordHash
                    };

                    Console.WriteLine("Seeding dummy customers");
                    context.Customers.AddOrUpdate(dummyCustomer);
                }

                // Ensure changes are saved to the database
                context.SaveChanges();
            }












            //---------------------------------------------------------------------------
            // Seed 10 random grocery categories
            SeedRandomGroceryCategories(context, 10);

            // Your product seeding logic here...
            var sampleProducts = Enumerable
                .Range(1, 15)
                .Select(i => new Product
                {
                    Name = $"Burger V{i}",
                    Description = $"Delicious burger with a juicy patty - Version {i}",
                    Price = 8.99m + i, // Assuming a price increase for each version
                    ImageFileName = $"burger_v{i}.jpg",
                    ProductQuantity = 30,
                    CategoryId = 1
                })
                .ToList();

            context.Products.AddRange(sampleProducts);
            context.SaveChanges();

            base.Seed(context);

            Console.WriteLine("Data Reset completed.");
        }

        // Function to generate random digits
        private string GetRandomDigits(int count)
        {
            Random random = new Random();
            return new string(
                Enumerable
                    .Repeat("0123456789", count)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray()
            );
        }

        // Seed random grocery categories
        private void SeedRandomGroceryCategories(
            ToZeptyDAL.Data.ZeptyDbContext context,
            int numberOfCategories
        )
        {
            if (!context.Categories.Any())
            {
                var randomCategories = GenerateRandomCategories(numberOfCategories);
                context.Categories.AddRange(randomCategories);
                context.SaveChanges();
            }
        }

        // Function to generate random grocery categories
        private List<Category> GenerateRandomCategories(int numberOfCategories)
        {
            var random = new Random();
            var categoryNames = new List<string>
            {
                "Fresh Produce",
                "Dairy and Eggs",
                "Meat and Seafood",
                "Bakery",
                "Canned Goods",
                "Beverages",
                "Snacks",
                "Frozen Foods",
                "Household Essentials",
                "Personal Care"
            };

            var randomCategories = categoryNames
                .OrderBy(c => random.Next())
                .Take(numberOfCategories)
                .Select(
                    (categoryName, index) =>
                        new Category
                        {
                            CategoryName = categoryName,
                            CategoryId = index + 1 // Assuming CategoryId is an identity column
                        }
                )
                .ToList();

            return randomCategories;
        }
    }
}
