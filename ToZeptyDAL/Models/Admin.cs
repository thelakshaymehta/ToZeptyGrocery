using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL
{
    public class Admin
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(255)]
        [Index("IX_UniqueAdminEmail", IsUnique = true)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        [MaxLength(255)]
        [Index("IX_UniqueAdminUserName", IsUnique = true)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(255)]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
