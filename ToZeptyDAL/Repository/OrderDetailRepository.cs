using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ZeptyDbContext _context;

        public OrderDetailRepository(ZeptyDbContext context)
        {
            _context = context;
        }

        public void CreateOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        public OrderDetail GetOrderDetailById(int orderDetailId)
        {
            return _context.OrderDetails.Find(orderDetailId);
        }

        public List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            return _context.OrderDetails.Where(od => od.OrderId == orderId).ToList();
        }

        public int GetOrderDetailByOrderId(int orderId)
        {
            var orderDetail = _context.OrderDetails
                  .FirstOrDefault(od => od.OrderId == orderId);
            return orderDetail.OrderStatus;
        }

        public IEnumerable<OrderDetail> GetAllOrderDetails()
        {
            return _context.OrderDetails.ToList();
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _context.Entry(orderDetail).State = EntityState.Modified;
            _context.SaveChanges();
        }


        public void DeleteOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
            _context.SaveChanges();
        }

        public bool UpdateOrderStatus(int orderId, int status)
        {
            try
            {
                // Get all order details with the specified orderId
                List<OrderDetail> orderDetails = _context.OrderDetails.Where(od => od.OrderId == orderId).ToList();

                if (orderDetails.Any())
                {
                    // Update the OrderStatus for each order detail
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.OrderStatus = status;
                    }

                    // Save changes to the database
                    _context.SaveChanges();

                    // Return true indicating the update was successful
                    return true;
                }
                else
                {
                    // No order details found with the specified orderId
                    Console.WriteLine("No order details found for the specified orderId.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error updating order status: {ex.Message}");
                return false;
            }
        }
    }
}

