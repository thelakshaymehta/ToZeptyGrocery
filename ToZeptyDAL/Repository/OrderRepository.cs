using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ZeptyDbContext _context;

        public OrderRepository(ZeptyDbContext context)
        {
            _context = context;
        }

        public void CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public Order CreateOrder2(Order order)
        {
            var test = _context.Orders.Add(order);
            _context.SaveChanges();
            return test;
        }


        public Order GetOrderById(int orderId)
        {
            return _context.Orders.Find(orderId);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        public void UpdateOrder(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }

        public IEnumerable<Order> GetOrdersByCustomerId(int userId)
        {
            IEnumerable<Order> orders = _context.Orders
           .Include(o => o.OrderDetails)
           .Where(o => o.CustomerId == userId)
           .ToList();

            return orders;
        }

        public IEnumerable<Order> GetIncompleteOrders()
        {
            IEnumerable<Order> orders = _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderDetails.Any(od => od.OrderStatus < 4))
                .ToList();
            // Selecting only one OrderDetail per OrderId
            foreach (var order in orders)
            {
                // Filter and assign only one OrderDetail per OrderId
                order.OrderDetails = order.OrderDetails
                    .Where(od => od.OrderStatus < 4)
                    .GroupBy(od => od.OrderId)
                    .Select(g => g.First())
                    .ToList();
            }
            return orders;
        }



    }
}
