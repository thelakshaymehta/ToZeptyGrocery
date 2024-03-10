using System.Collections.Generic;

namespace ToZeptyDAL.Interface
{
    public interface IAddressRepository
    {
        IEnumerable<Address> GetAddressesByUserId(int userId);
        void SaveAddress(Address address);
        // Add other methods as needed
    }
}
