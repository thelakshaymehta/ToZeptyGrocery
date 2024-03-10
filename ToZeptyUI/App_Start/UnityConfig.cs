using ToZeptyDAL.Interface;
using ToZeptyDAL.Repository;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace ToZeptyUI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<IAddressRepository, AddressRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            container.RegisterType<ICartRepository, CartRepository>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDetailRepository, OrderDetailRepository>();
            container.RegisterType<ICategory, CategoryRepository>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}