using A2208hub.Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Tools
{
    public class LogHelper
    {
        public static FileLog CreateLog(string arg)
        {
            FileLog f = new FileLog();
            if (HttpContext.Current.Request.IsAuthenticated)
                f.User = HttpContext.Current.User.Identity.Name;
            else
                f.User = "匿名";
            f.Date = DateTime.Now;
            f.IpAddress = HttpContext.Current.Request.UserHostAddress;
            f.Log = arg;
            return f;
        }
    }
}