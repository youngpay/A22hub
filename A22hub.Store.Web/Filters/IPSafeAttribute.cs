using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace A2208hub.Store.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class IPSafeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var Request = filterContext.HttpContext.Request;
            string ip = WebConfigurationManager.AppSettings["IPSafeList"];
            string[] ips = ip.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            bool safeIp = false;
            var userIp = Request.UserHostAddress;
            for (int i = 0; i < ips.Length; i++)
            {
                if (userIp.StartsWith(ips[i]))
                {
                    safeIp = true;
                    break;
                }
            }
            if (safeIp)
            {
                base.OnActionExecuted(filterContext);
            }
            else
            {
                ContentResult c = new ContentResult();
                c.Content = "无权访问=>" + Request.UserHostAddress;
                filterContext.Result = c;
            }
        }
    }
}