using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Interface
{
    public interface IProductRepository
    {
        int SaveProductChanges();
        Product CreateProduct(Product product);
        Product GetProductById(int productId);
        IEnumerable<Product> GetAllProducts();

        Product UpdateProduct(Product product);

        Product DeleteProduct(int productId);
    }
}
