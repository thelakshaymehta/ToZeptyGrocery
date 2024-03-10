using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Interface
{
    public interface ICartRepository
    {
        // Create
        Cart CreateCartItem(Cart cartItem);
        Cart GetCartItemByCartIdAndCustomerId(int cartId, int customerId);
        bool cartStatus();

        // Read
        Cart GetCartItemById(int cartItemId);
        IEnumerable<Cart> GetAllCartItems();

        // Update
        Cart UpdateCartItem(Cart cartItem);

        // Delete
        Cart DeleteCartItem(int cartItemId);
        IEnumerable<Cart> GetCartItemById(int[] cartIds);
        int cartSaveChanges();
        List<Cart> GetCartItemsByCustomerId(int? customerId);

        void DeleteCartItems(List<Cart> cartItems);
        Cart RemoveCartItem(Cart cartItem);
    }
}
