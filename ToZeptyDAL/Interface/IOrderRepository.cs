using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Interface
{
    public interface IOrderRepository
    {
        void CreateOrder(Order order);
        Order CreateOrder2(Order order);
        Order GetOrderById(int orderId);
        IEnumerable<Order> GetAllOrders();
        void UpdateOrder(Order order);
        void DeleteOrder(Order order);
        IEnumerable<Order> GetOrdersByCustomerId(int userId);
        IEnumerable<Order> GetIncompleteOrders();
    }


}
