using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ZeptyDbContext _context;

        public ProductRepository(ZeptyDbContext context)
        {
            _context = context;
        }

        // Create
        public Product CreateProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }


        // Read
        public Product GetProductById(int productId)
        {
            return _context.Products.Find(productId);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();

        }

        // Update
        public Product UpdateProduct(Product product)
        {
            var existingProduct = _context.Products.Find(product.ProductId);

            if (existingProduct != null)
            {
                // Update the properties of the existing product with the values from the input product
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.ImageFileName = product.ImageFileName;

                _context.SaveChanges();
            }

            return existingProduct;
        }

        // Delete
        public Product DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return product;
        }
        public int SaveProductChanges()
        {
            return _context.SaveChanges();
        }
    }
}
