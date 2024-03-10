using System.Data.Entity;

namespace ToZeptyDAL.Data
{
    public class ZeptyDbContext : DbContext
    {
        public ZeptyDbContext()
            : base("ZeptyDB")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ZeptyDbContext>());

        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure the relationship between Customer and Order
            //modelBuilder.Entity<Category>().Property(c => c.CategoryName).HasMaxLength(255);

            base.OnModelCreating(modelBuilder);
        }
    }
}
