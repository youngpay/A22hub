using A2208hub.Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace A2208hub.Store.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CheckConfig();
            CheckDatabase();
        }

        private void CheckConfig()
        {
            string documentRoot = System.Web.Configuration.WebConfigurationManager.AppSettings["DocumentRoot"];
            if (string.IsNullOrEmpty(documentRoot))
            {
                throw new Exception("没有设置 DocumentRoot");
            }
        }

        private void CheckDatabase()
        {
            if (false == Roles.RoleExists("admin"))
            {
                Roles.CreateRole("admin");
                Roles.CreateRole("user");
                Roles.CreateRole("guest");

                Membership.CreateUser("admin", "123");
                Membership.CreateUser("public", "public");
                Membership.CreateUser("guest", "guest");

                Roles.AddUserToRole("admin", "admin");
                Roles.AddUserToRole("public", "user");
                Roles.AddUserToRole("guest", "guest");
            }
        }
    }
}