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
            var f = new FileLog { 
                User = HttpContext.Current.User.Identity.Name,
                Date = DateTime.Now,
                IpAddress = HttpContext.Current.Request.UserHostAddress,
                Log = arg
            };
            return f;
        }
    }
}