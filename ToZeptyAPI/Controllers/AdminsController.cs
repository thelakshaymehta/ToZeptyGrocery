using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ToZeptyDAL;
using ToZeptyDAL.Data;
using ToZeptyDAL.Repository;

namespace ToZeptyAPI.Controllers
{
    public class AdminsController : ApiController
    {
        private readonly AdminRepository _adminRepository;

        public AdminsController()
        {
            _adminRepository = new AdminRepository(new ZeptyDbContext());
        }

        // GET: api/Admins
        public IHttpActionResult GetAdmins()
        {
            var admins = _adminRepository.GetAllAdmins();
            return Ok(admins);
        }

        // GET: api/Admins/5
        [ResponseType(typeof(Admin))]
        public IHttpActionResult GetAdmin(int id)
        {
            Admin admin = _adminRepository.GetAdminById(id);
            if (admin == null)
            {
                return NotFound();
            }

            return Ok(admin);
        }

        // POST: api/Admins
        [ResponseType(typeof(Admin))]
        public IHttpActionResult PostAdmin(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAdmin = _adminRepository.CreateAdmin(admin);
            return CreatedAtRoute("DefaultApi", new { id = createdAdmin.Id }, createdAdmin);
        }

        // PUT: api/Admins/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAdmin(int id, Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != admin.Id)
            {
                return BadRequest();
            }

            var updatedAdmin = _adminRepository.UpdateAdmin(admin);

            if (updatedAdmin == null)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Admins/5
        [ResponseType(typeof(Admin))]
        public IHttpActionResult DeleteAdmin(int id)
        {
            Admin admin = _adminRepository.DeleteAdmin(id);
            if (admin == null)
            {
                return NotFound();
            }

            return Ok(admin);
        }


    }
}
