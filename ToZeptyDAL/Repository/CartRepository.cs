using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;

namespace ToZeptyDAL.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ZeptyDbContext _context;

        public CartRepository(ZeptyDbContext context)
        {
            _context = context;
        }

        public bool cartStatus()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public Cart GetCartItemByCartIdAndCustomerId(int cartId, int customerId)
        {
            return _context.Carts.FirstOrDefault(c =>
                c.CartId == cartId && c.CusomerId == customerId
            );
        }

        public List<Cart> GetCartItemsByCustomerId(int? customerId)
        {
            return _context.Carts.Where(c => c.CusomerId == customerId).ToList();
        }

        public Cart GetCartItemByProductIdAndCustomerId(int productId, int customerId)
        {
            return _context.Carts.FirstOrDefault(c =>
                c.ProductId == productId && c.CusomerId == customerId
            );
        }

        // Create
        public Cart CreateCartItem(Cart cartItem)
        {
            var savedItem = _context.Carts.Add(cartItem);
            _context.SaveChanges();
            return savedItem;
        }

        // Read
        public Cart GetCartItemById(int cartItemId)
        {
            return _context.Carts.Find(cartItemId);
        }

        public IEnumerable<Cart> GetCartItemById(int[] cartIds)
        {
            return _context.Carts.Where(c => cartIds.Contains(c.CartId)).ToList();
        }

        public IEnumerable<Cart> GetAllCartItems()
        {
            return _context.Carts.ToList();
        }

        // Update
        public Cart UpdateCartItem(Cart cartItem)
        {
            var existingCartItem = _context.Carts.Find(cartItem.CartId);

            if (existingCartItem != null)
            {
                existingCartItem.CusomerId = cartItem.CusomerId;
                existingCartItem.ProductName = cartItem.ProductName;
                existingCartItem.Quantity = cartItem.Quantity;
                existingCartItem.Price = cartItem.Price;
                existingCartItem.ImageFileName = cartItem.ImageFileName;
                existingCartItem.ProductId = cartItem.ProductId;

                _context.SaveChanges();
            }

            return existingCartItem;
        }

        // Delete
        public Cart DeleteCartItem(int cartItemId)
        {
            var cartItem = _context.Carts.Find(cartItemId);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            return cartItem;
        }

        public int cartSaveChanges()
        {
            return _context.SaveChanges();
        }

        public void DeleteCartItems(List<Cart> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                var existingCartItem = _context.Carts.Find(cartItem.CartId);

                if (existingCartItem != null)
                {
                    _context.Carts.Remove(existingCartItem);
                }
            }

            _context.SaveChanges();
        }

        public Cart RemoveCartItem(Cart cartItem)
        {
            var existingCartItem = _context.Carts.Find(cartItem.CartId);

            if (existingCartItem != null)
            {
                _context.Carts.Remove(existingCartItem);
                _context.SaveChanges();
                return existingCartItem;
            }
            return null;
        }
    }
}
