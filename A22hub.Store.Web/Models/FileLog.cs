using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Models
{
    public class FileLog
    {
        public long Id { get; set; }
        public string Log { get; set; }
        public string User { get; set; }
        public string IpAddress { get; set; }
        public DateTime Date { get; set; }
    }
}