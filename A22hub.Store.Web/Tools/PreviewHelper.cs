using A2208hub.Store.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A2208hub.Store.Web.Tools
{
    public class PreviewHelper
    {
        private static string root = System.Web.Configuration.WebConfigurationManager.AppSettings["DocumentRoot"];
        public static ActionResult Preview(string dir)
        {
            if (dir.StartsWith("/"))
                dir = dir.Remove(0, 1);
            dir = Path.Combine(root, dir);
            string ext = Path.GetExtension(dir);
            ext = ext.ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".png":
                case ".ico":
                case ".bmp":
                    return RreviewAsImage(dir);
                case ".doc":
                case ".docx":
                    return PreviewAsWord(dir);
                case ".xls":
                case ".xlsx":
                    return PreviewAsExcel(dir);
                case ".pdf":
                    return PreviewAsPdf(dir);
                default:
                    return PreviewAsText(dir);
            }
        }

        private static ActionResult PreviewAsText(string dir)
        {
            string[] infos = GetFileInfo(dir);
            return Json(new AjaxData(data: new {
                source = "<pre style=\"text-align:left\">" + System.Web.HttpContext.Current.Server.HtmlEncode(System.IO.File.ReadAllText(dir)) + "</pre>",
                modify = infos[0]
            }));
        }

        private static ActionResult RreviewAsImage(string dir)
        {
            string ext = MimeType.GetTypeName(Path.GetExtension(dir));
            string[] infos = GetFileInfo(dir);
            string base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(dir));
            return Json(new AjaxData(data: new {
                source = string.Format("data:{0};base64,{1}", ext, base64),
                modify = infos[0]
            }));
        }

        private static ActionResult PreviewAsWord(string dir)
        {
            string[] infos = GetFileInfo(dir);
            return Json(new AjaxData(data: new {
                source = OfficeHelper.HtmlWithWord(dir),
                modify = infos[0]
            }));
        }

        private static ActionResult PreviewAsExcel(string dir)
        {
            string[] infos = GetFileInfo(dir);
            return Json(new AjaxData(data: new
            {
                source = OfficeHelper.HtmlWithExcel(dir),
                modify = infos[0]
            }));
        }

        private static ActionResult PreviewAsPdf(string dir)
        {
            string[] infos = GetFileInfo(dir);
            return Json(new AjaxData(data: new
            {
                source = OfficeHelper.HtmlWithPdf(dir),
                modify = infos[0]
            }));
        }

        private static JsonResult Json(object data)
        {
            var json = new ConfigurableJsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            json.Data = data;
            return json;
        }

        private static string[] GetFileInfo(string dir) 
        {
            string[] s = new string[] { "" };
            s[0] = File.GetLastWriteTime(dir).ToString("yyyy-MM-dd HH:mm:ss");
            
            return s;
        }
    }
}