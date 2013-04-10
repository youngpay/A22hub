using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Models
{
    public class FileLink
    {
        public long Id { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string Guid { get; set; }
    }
}