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
    [Authorize]
    public class FileController : Controller
    {
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

        public ActionResult Index()
        {
            return View();
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

        [HttpGet]
        public ActionResult Upload() 
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Upload(FormCollection fc)
        {
            //string base64 = fc["base64"];
            if (Request.Files.Count == 0)
                return Json(new AjaxData("参数错误"));
            string savePath = fc["path"];
            var file = Request.Files[0];


            if (string.IsNullOrEmpty(savePath))
                return Json(new AjaxData("参数错误"));

            string path = Path.Combine(root, savePath);
            if (System.IO.File.Exists(path))
                return Json(new AjaxData("此文件已存在"));

            file.SaveAs(path);
            var log = LogHelper.CreateLog("文件上传：" + path);
            db.FileLogs.Add(log);
            db.SaveChanges();
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
    }
}
