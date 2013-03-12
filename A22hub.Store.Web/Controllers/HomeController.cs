using A2208hub.Store.Web.Models;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

namespace A2208hub.Store.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            StoreDBContext db = new StoreDBContext();
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            var username = form["username"];
            var password = form["password"];
            var remember = form["remember"];
            var returnurl = form["ReturnUrl"];
            string error = null;
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(remember))
            {
                error = "错误的请求";
                goto response;
            }
            var finduser = Membership.FindUsersByName(username);
            if (finduser.Count == 0)
            {
                error = "用户名不存在";
                goto response;
            }
            if (Membership.ValidateUser(username, password) == false)
            {
                error = "密码错误";
                goto response;
            }
            else
            {
                bool persistentCookie = remember == "1";
                FormsAuthentication.SetAuthCookie(username, persistentCookie);
            }
            response:
            if (error == null)
                return Json(new AjaxData(data: returnurl ?? FormsAuthentication.DefaultUrl));
            else
                return Json(new AjaxData(error));
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
