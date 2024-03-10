using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToZeptyDAL.Data;
using ToZeptyDAL.Interface;

namespace ToZeptyDAL.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ZeptyDbContext _dbContext;

        public AddressRepository(ZeptyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Address> GetAddressesByUserId(int userId)
        {
            return _dbContext.Addresses.Where(a => a.CustomerId == userId).ToList();
        }

        public void SaveAddress(Address address)
        {
            _dbContext.Addresses.Add(address);
            _dbContext.SaveChanges();
        }
    }
}
