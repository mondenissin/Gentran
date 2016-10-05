using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gentran
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "Home/{*catchall}",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(
                name: "Admin",
                url: "Admin/{*catchall}",
                defaults: new { controller = "Admin", action = "Index" });

            routes.MapRoute(
                name: "Retrieve",
                url: "Retrieve/{*catchall}",
                defaults: new { controller = "Retrieve", action = "Index" });

            routes.MapRoute(
                name: "Order",
                url: "Order/{*catchall}",
                defaults: new { controller = "Order", action = "Index" });

            routes.MapRoute(
                name: "Monitoring",
                url: "Monitoring/{*catchall}",
                defaults: new { controller = "Monitoring", action = "Index" });


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}