using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL.Interface
{
    public interface IAdminRepository
    {
        // Create
        Admin CreateAdmin(Admin admin);

        // Read
        bool AdminExistsEmail(string UserEmail);
        bool AdminExists(string userName);
        Admin GetAdminById(int adminId);
        IEnumerable<Admin> GetAllAdmins();
        bool AdminExistsEmail(string UserEmail, int id);
        // Update
        Admin UpdateAdmin(Admin admin);

        // Delete
        Admin DeleteAdmin(int adminId);
        Admin GetAdminByUserName(string userName);
        void SaveAdminchages();
        Admin GetAdminByUserNamePhone(string userName, string phoneNumber);
    }

}
