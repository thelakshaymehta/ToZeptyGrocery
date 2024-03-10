using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Interface
{
    public interface ICustomerRepository
    {
        // Create
        Customer GetCustomerByUserNamePhone(string UserName, string PhoneNumber);
        int customerSAveChanges();
        Cart GetCartItemByProductIdAndCustomerId(int productId, int customerId);
        Customer GetCustomerByUserName(string userName);
        Customer CreateCustomer(Customer customer);
        bool CustomerExistsEmail(string Email);

        bool CustomerExistsEmail(string Email, int id);
        // Read
        Customer GetCustomerById(int customerId);
        IEnumerable<Customer> GetAllCustomers();
        bool CustomerExists(string userName);
        // Update
        Customer UpdateCustomer(Customer customer);

        // Delete
        Customer DeleteCustomer(int customerId);

    }

}
