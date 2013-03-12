using A2208hub.Store.Web.Filters;
using System.Web;
using System.Web.Mvc;

namespace A2208hub.Store.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleExceptionAttribute());
        }
    }
}