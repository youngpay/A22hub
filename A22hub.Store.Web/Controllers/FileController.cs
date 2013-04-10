using A2208hub.Store.Web.Filters;
using A2208hub.Store.Web.Models;
using A2208hub.Store.Web.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace A2208hub.Store.Web.Controllers
{
    [IPSafe]
    [Authorize]
    public class FileController : Controller
    {
        private static object obj = new object();
        private StoreDBContext db = new StoreDBContext();
        private static string root = System.Web.Configuration.WebConfigurationManager.AppSettings["DocumentRoot"];

        private List<ItemInfo> DirsByPath(string dirname)
        {
            if (dirname.Contains(".."))
                dirname = "/";
            if (dirname.Length > 0 && dirname.StartsWith("/"))
                dirname = dirname.Remove(0, 1);
            string path = Path.Combine(root, dirname);
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            List<ItemInfo> items = new List<ItemInfo>(dirs.Length + files.Length);
            for (int i = 0; i < dirs.Length; i++)
            {
                items.Add(new ItemInfo
                {
                    Title = dirs[i].Name,
                    Type = "dir",
                    Dir =  dirname
                });
            }
            for (int i = 0; i < files.Length; i++)
            {
                items.Add(new ItemInfo
                {
                    Title = files[i].Name,
                    Type = "file",
                    Dir = dirname,
                    Length = files[i].Length
                });
            }
            if (dirname == "")
            {
                items.Remove(items.Find(f => f.Title == "IISRoot"));
                items.Remove(items.Find(f => f.Title == "temp"));
            }
            return items;
        }

        public ActionResult Dir(string dirname = "")
        {
            var items = DirsByPath(dirname);
            return Json(new AjaxData(items), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string dir = "/")
        {
            return View(model:dir);
        }

        public ActionResult Preview(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return Json(new AjaxData("参数错误"), JsonRequestBehavior.AllowGet);
            try
            {
                return PreviewHelper.Preview(dir);
            }
            catch (Exception ex)
            {
                return Json(new AjaxData(ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Pdf(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException("dir");
            if (!dir.EndsWith(".pdf"))
                throw new ArgumentException("dir");

            if (dir.StartsWith("/"))
                dir = dir.Remove(0, 1);
            return File(Path.Combine(root, dir), MimeType.GetTypeName(dir.Substring(dir.LastIndexOf("."))));
        }

        [AllowAnonymous]
        public ActionResult Download(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException("dir");
            if (dir.StartsWith("/"))
                dir = dir.Remove(0, 1);
            string path = Path.Combine(root, dir);
            var log = LogHelper.CreateLog("文件下载：" + path);
            db.FileLogs.Add(log);
            db.SaveChanges();
            return File(path, MimeType.GetTypeName(dir), dir.Substring(dir.LastIndexOf('/') + 1));
        }
        [Authorize(Roles="admin")]
        public ActionResult Delete(string dir, string type)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException("dir");
            if (dir.StartsWith("/"))
                dir = dir.Remove(0, 1);
            dir = Path.Combine(root, dir);
            if (type == "dir")
            {
                if (!Directory.Exists(dir))
                    return Json(new AjaxData("此文件夹不存在"), JsonRequestBehavior.AllowGet);
                Directory.Delete(dir);
            }
            else if (type == "file")
            {
                if (!System.IO.File.Exists(dir))
                    return Json(new AjaxData("此文件不存在"), JsonRequestBehavior.AllowGet);
                System.IO.File.Delete(dir);

                var log = LogHelper.CreateLog("文件删除：" + dir);
                db.FileLogs.Add(log);
                db.SaveChanges();
            }
            else
                return Json(new AjaxData("不支持的类型：" + type), JsonRequestBehavior.AllowGet);
            return Json(new AjaxData(AjaxStatusCode.OK), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            var name = Request.Headers["X-File-Path"];
            var size = long.Parse(Request.Headers["X-File-Size"]);
            var position = long.Parse(Request.Headers["X-File-Position"]);
            var stream = Request.InputStream;
            if (name == null || stream == null)
                return Json(new AjaxData("参数错误"));

            name = Server.UrlDecode(name);
            name = Path.Combine(root, name);

            lock (obj)
            {
                using (var fs = new FileStream(name, FileMode.OpenOrCreate))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    fs.SetLength(size);
                    fs.Position = position;
                    fs.Write(bytes, 0, (int)bytes.Length);

                    if (fs.Position == size)
                        return Json(new AjaxData(data: "done"));
                }
            }

            return Json(new AjaxData(AjaxStatusCode.OK));
        }

        public ActionResult Mkdir(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return Json(new AjaxData("参数错误"));
            if (dir.Length > 1 && dir.StartsWith("/"))
                dir = dir.Remove(0, 1);
            if (dir.Length > 1 && dir.StartsWith("/"))
                dir = dir.Remove(0, 1);
            string path = Path.Combine(root, dir);
            if (Directory.Exists(path))
                return Json(new AjaxData("文件夹已存在"));
            Directory.CreateDirectory(path);
            var log = LogHelper.CreateLog("新建文件夹：" + path);
            db.FileLogs.Add(log);
            db.SaveChanges();
            return Json(new AjaxData(AjaxStatusCode.OK));
        }

        public ActionResult Exists(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return Json(new AjaxData("参数错误"), JsonRequestBehavior.AllowGet);
            if (System.IO.File.Exists(Path.Combine(root,dir)))
                return Json(new AjaxData(data: "1"), JsonRequestBehavior.AllowGet);
            else
                return Json(new AjaxData(data: "0"), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Link(string id)
        {
            if (false == Request.IsAuthenticated)
                FormsAuthentication.SetAuthCookie("guest", false);
            ViewBag.IsAuthenticated = true;
            ViewBag.Username = "guest";

            var fileLink = FileRedirectWithGuidFromCache(id);
            if (fileLink == null)
                throw new Exception("此链接无效：" + id);
            if (fileLink.Type == "dir")
                return View("Index", model: fileLink.Path);
            else if (fileLink.Type == "file")
                return RedirectToAction("Download", new { dir = fileLink.Path });
            else
                throw new Exception("错误的文件类型：" + fileLink.Type);
        }

        private FileLink FileRedirectWithGuidFromCache(string guid)
        {
            var cacheKey = string.Format("FileController.FileRedirectWithGuidFromCache({0})", guid);

            var cache = HttpRuntime.Cache[cacheKey] as FileLink;
            if (cache == null)
            {
                var query = from f in db.FileLinks
                            where f.Guid.Equals(guid, StringComparison.InvariantCultureIgnoreCase)
                            select f;
                var fileLink = query.SingleOrDefault();
                if (fileLink != null)
                {
                    HttpRuntime.Cache.Insert(cacheKey, fileLink);
                    cache = fileLink;
                }
            }
            return cache;
        }

        public ActionResult FileLink(string dir, string type)
        {
            var f = FileLinkFromCache(dir, type);
            return Json(new AjaxData(f), JsonRequestBehavior.AllowGet);
        }

        private FileLink FileLinkFromCache(string dir, string type) 
        {
            var cacheKey = string.Format("FileController.FileLinkFromCache({0}, {1})", dir, type);

            var cache = HttpRuntime.Cache[cacheKey] as FileLink;
            if (cache == null)
            {
                var query = from f in db.FileLinks
                            where f.Path.Equals(dir) && f.Type.Equals(type)
                            select f;
                var fileLink = query.SingleOrDefault();
                if (fileLink == null)
                {
                    fileLink = new FileLink
                    {
                        Path = dir,
                        Type = type,
                        Guid = Guid.NewGuid().ToString().Replace("-", string.Empty)
                    };
                    db.FileLinks.Add(fileLink);
                    db.SaveChanges();
                }

                HttpRuntime.Cache.Insert(cacheKey, fileLink);
                cache = fileLink;
            }

            return cache;
        }
    }
}
