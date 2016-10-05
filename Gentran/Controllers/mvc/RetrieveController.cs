using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gentran.Controllers.mvc
{
    public class RetrieveController : Controller
    {
        private string basePath = "";
        byte[] sessionName = new Byte[20];
        // GET: /<controller>/
        public ActionResult Index()
        {
            if (!GetSession())
            {
                basePath = HttpContext.Request.ApplicationPath.Length > 1 ? HttpContext.Request.ApplicationPath + "/" : HttpContext.Request.ApplicationPath;
                HttpContext.Response.Redirect(basePath);
            }

            ViewData["Message"] = "Sub modules";

            return View();
        }

        public bool GetSession()
        {
            if (Session.Count > 0)
                return true;

            return false;
        }
    }
}
