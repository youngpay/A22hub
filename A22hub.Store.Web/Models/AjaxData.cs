using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Models
{
    public class AjaxData
    {
        public AjaxData() {
            this.Status = AjaxStatusCode.OK;
        }
        public AjaxData(string message)
        {
            this.Status = AjaxStatusCode.Error;
            this.Message = message;
        }
        public AjaxData(object data)
        {
            this.Status = AjaxStatusCode.OK;
            this.Data = data;
        }
        public AjaxData(AjaxStatusCode status) 
        {
            this.Status = status;
        }

        public AjaxStatusCode Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}