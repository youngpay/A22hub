using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Models
{
    public class UploadFileViewModel
    {
        public UploadFileViewModel() { }
        public UploadFileViewModel(string uploadUrl) {
            this.UploadUrl = uploadUrl;
        }
        public string UploadUrl { get; set; }
    }
}