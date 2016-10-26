using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gentran.Controllers
{
    public class HomeController : Controller
    {
        private string basePath = "";

        public ActionResult Index()
        {
            if (!GetSession())
            {
                basePath = HttpContext.Request.ApplicationPath.Length > 1 ? HttpContext.Request.ApplicationPath + "/" : HttpContext.Request.ApplicationPath;
                HttpContext.Response.Redirect(basePath);
            }
            return View();
        }
        public ActionResult Login()
        {
            if (GetSession())
            {
                basePath = HttpContext.Request.ApplicationPath.Length > 1 ? HttpContext.Request.ApplicationPath + "/" : HttpContext.Request.ApplicationPath;
                HttpContext.Response.Redirect(basePath + "Home/Index");
            }
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }

        public bool GetSession()
        {
            if(Session.Count >0)
                return true;
            
            return false;
        }
    }
}