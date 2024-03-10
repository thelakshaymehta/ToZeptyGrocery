using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Interface
{
    public interface IOrderDetailRepository
    {
        void CreateOrderDetail(OrderDetail orderDetail);
        OrderDetail GetOrderDetailById(int orderDetailId);
        IEnumerable<OrderDetail> GetAllOrderDetails();
        void UpdateOrderDetail(OrderDetail orderDetail);
        void DeleteOrderDetail(OrderDetail orderDetail);
        int GetOrderDetailByOrderId(int orderId);
        List<OrderDetail> GetOrderDetailsByOrderId(int orderId);
        bool UpdateOrderStatus(int orderId, int status);
    }

}
