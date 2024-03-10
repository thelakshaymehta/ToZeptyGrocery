using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToZeptyUI.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorize]
    public class ErrorrController : Controller
    {
        // GET: Errorr
        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult UnautzorizedAccess()
        {
            return View();
        }
    }
}