using A2208hub.Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A2208hub.Store.Web.Filters
{
    public class RequiresAuthorizeAttribute : AuthorizeAttribute
    {
        public RequiresAuthorizeAttribute() 
        {
            ResultType = UnauthorizedResultType.Default;
        }

        public UnauthorizedResultType ResultType { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["user"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            switch (ResultType)
            {
                case UnauthorizedResultType.Default:
                    filterContext.Result = new RedirectResult("/");
                    break;
                case UnauthorizedResultType.Ajax:
                    var result = new JsonResult();
                    result.Data = Newtonsoft.Json.JsonConvert.SerializeObject(new AjaxData(AjaxStatusCode.Unauthorized));
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    filterContext.Result = result;
                    break;
            }
        }
    }

    public enum UnauthorizedResultType
    {
        Default,
        Ajax
    }
}