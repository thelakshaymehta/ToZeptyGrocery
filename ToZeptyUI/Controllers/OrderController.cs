using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToZeptyDAL;
using ToZeptyDAL.Interface;
using ToZeptyUI.Models;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderDetailRepository orderDetailRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository
        )
        {
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
        }

        public ActionResult InCompleteOrders()
        {
            var orders = orderRepository.GetIncompleteOrders();
            var productViewModels = orders
                .SelectMany(od =>
                    od.OrderDetails.Select(detail => new InCompleteOrderDetails
                    {
                        CustomerId = detail.Order.CustomerId,
                        OrderId = detail.Order.OrderId,
                        OrderDetails = detail.OrderDetailId,
                        OrderStatus = detail.OrderStatus,
                    })
                )
                .ToList();

            return View(productViewModels);
        }

        public ActionResult UpdateOrder(int Id)
        {
            var ShowOrderToUpdateStatus = orderDetailRepository.GetOrderDetailsByOrderId(Id);
            List<UpdateOrderStatusModel> orderToUpdate = ShowOrderToUpdateStatus
                .Select(data => new UpdateOrderStatusModel
                {
                    ProductName = data.ProductName,
                    Quantity = data.Quantity,
                    OrderId = Id,
                })
                .ToList();
            return View(orderToUpdate);
        }

        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, int status)
        {
            bool result = orderDetailRepository.UpdateOrderStatus(orderId, status);
            if (result)
            {
                return RedirectToAction("InCompleteOrders", "Order");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update order status.");
                return View();
            }
        }
    }
}
