using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string LoginIp { get; set; }
        public int State { get; set; }
    }
}