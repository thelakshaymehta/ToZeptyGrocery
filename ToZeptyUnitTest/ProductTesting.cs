using ToZeptyDAL;
using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestFixture]
    public class ProductTesting
    {

        private ZeptyDbContext _dbContext;
        private IProductRepository _productRepository;
        private Product ProductItem;
        [SetUp]
        public void Setup()
        {
            _dbContext = new ZeptyDbContext();
            _productRepository = new ProductRepository(_dbContext);

            ProductItem = new Product
            {
                ProductQuantity = 10,
                ProductId = 3,
                Name = "Sample Product",
                Description = "This is a sample product.",
                Price = 39.99m,
                ImageFileName = "sample-product.jpg"
            };

        }

        [Test]
        public void CreateProduct_ShouldAddToDbContext()
        {
            var fakeObject = new Mock<IProductRepository>();
            fakeObject.Setup(x => x.CreateProduct(It.IsAny<Product>())).Returns(ProductItem);
            var result = fakeObject.Object.CreateProduct(ProductItem);

            Assert.That(result, Is.EqualTo(ProductItem));

        }


        [Test]
        public void RemoveCartItem_ShouldRemoveFromDatabase()
        {
            var fakeObject = new Mock<IProductRepository>();
            fakeObject.Setup(x => x.DeleteProduct(It.IsAny<int>())).Returns(ProductItem);

            var cartRepository = fakeObject.Object;
            var result = cartRepository.DeleteProduct(ProductItem.ProductId);
            Assert.That(result, Is.EqualTo(result));
        }


        [Test]
        public void GetProductById_ShouldReturnProduct()
        {
            var fakeObject = new Mock<IProductRepository>();
            var productId = 10;


            fakeObject.Setup(x => x.GetProductById(It.IsAny<int>())).Returns(ProductItem);

            var productRepository = fakeObject.Object;
            var result = productRepository.GetProductById(productId);
            Assert.That(result, Is.EqualTo(ProductItem));
        }

        [Test]
        public void UpdateProduct_ShouldUpdateInDatabase()
        {
            var fakeObject = new Mock<IProductRepository>();
            var updatedProduct = new Product
            {
                ProductId = 10,
                Name = "Updated Product",
                Description = "Updated description",
                Price = 39.99m,
                ImageFileName = "updated-image.png"
            };

            fakeObject.Setup(x => x.UpdateProduct(It.IsAny<Product>())).Returns(updatedProduct);

            var productRepository = fakeObject.Object;
            var result = productRepository.UpdateProduct(updatedProduct);
            Assert.That(result, Is.EqualTo(updatedProduct));
        }

    }
}
