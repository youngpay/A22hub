using A2208hub.Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A2208hub.Store.Web.Filters
{
    public class HandleExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            Exception e = filterContext.Exception;
            if (filterContext.HttpContext.Request.AcceptTypes.Contains("application/json"))
            {
                AjaxData data = new AjaxData {
                    Status = AjaxStatusCode.Error,
                    Message = e.Message
                };
                var result = new JsonResult();
                result.Data = data;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = result;
            }
            else
            {
                var result = new ViewResult();
                result.ViewBag.Error = e.Message;
                result.ViewBag.Detail = e.StackTrace;
                result.ViewName = "Error";
                filterContext.Result = result;
            }
        }
    }
}