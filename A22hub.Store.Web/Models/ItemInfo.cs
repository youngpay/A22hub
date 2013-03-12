using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Models
{
    public class ItemInfo
    {
        private static string imgPath = "/Content/images/store/";
        public string Dir { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public long Length { get; set; }
        public string FileExtension { 
            get {
                if (string.Equals("dir", Type, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                if (string.IsNullOrEmpty(Title))
                {
                    return null;
                }
                int index = Title.LastIndexOf('.');
                if (index == -1)
                    return null;
                else
                    return Title.Substring(index);
            } 
        }
        public string Icon { 
            get {
                string ext = this.FileExtension;
                if (ext != null)
                {
                    ext = ext.ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                            ext = "jpg";
                            break;
                        case ".png":
                            ext = "png";
                            break;
                        case ".gif":
                            ext = "gif";
                            break;
                        case ".bmp":
                            ext = "bmp";
                            break;
                        case ".zip":
                        case ".iso":
                        case ".rar":
                            ext = "compressed";
                            break;
                        case ".ini":
                        case ".inc":
                        case ".config":
                            ext = "config";
                            break;
                        case ".exe":
                        case ".dll":
                        case ".pyc":
                            ext = "exe";
                            break;
                        case ".txt":
                        case ".js":
                        case ".css":
                        case ".cs":
                        case ".java":
                        case ".cpp":
                        case ".h":
                        case ".py":
                        case ".markdown":
                        case ".md":
                        case ".log":
                            ext = "text";
                            break;
                        case ".htm":
                        case ".html":
                            ext = "html";
                            break;
                        case ".doc":
                        case ".docx":
                            ext = "word";
                            break;
                        case ".xls":
                        case ".xlsx":
                            ext = "excel";
                            break;
                        case ".pdf":
                            ext = "pdf";
                            break;
                        default:
                            ext = "file";
                            break;
                    }
                    return string.Format("{0}{1}.ico", imgPath, ext);
                }
                else
                    return string.Format("{0}{1}.ico", imgPath, Type);
            } 
        }
        public string Link {
            get {
                if (string.IsNullOrWhiteSpace(Dir))
                {
                    return Title;
                }
                else
                {
                    return string.Format("{0}/{1}", Dir, Title);
                }
            }
        }
    }
}