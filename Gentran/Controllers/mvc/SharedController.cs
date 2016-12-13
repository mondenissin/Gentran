using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Gentran.Controllers.mvc
{
    public class SharedController : Controller
    {
        //
        // GET: /Shared/

        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            System.Web.HttpRuntime.UnloadAppDomain();
            return RedirectToAction("Login", "Home");
            //return View();
        }

    }
}
