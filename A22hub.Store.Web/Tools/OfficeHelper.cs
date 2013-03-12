using W = Aspose.Words;
using WS = Aspose.Words.Saving;
using C = Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace A2208hub.Store.Web.Tools
{
    public class OfficeHelper
    {
        public static string HtmlWithWord(string dir)
        {
            if (!dir.EndsWith(".doc") && !dir.EndsWith(".docx"))
                throw new ArgumentException("dir");
            W.Document doc = new W.Document(dir);
            string html = null;

            WS.HtmlSaveOptions options = new WS.HtmlSaveOptions(W.SaveFormat.Html);
            options.ExportTextInputFormFieldAsText = true;
            options.ExportImagesAsBase64 = true;

            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms, options);
                ms.Position = 0;
                html = new StreamReader(ms).ReadToEnd();
            }
            //return html.Replace(@"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />", string.Empty);
            return html;
        }

        public static string HtmlWithExcel(string dir)
        {
            if (!dir.EndsWith(".xls") && !dir.EndsWith(".xlsx"))
                throw new ArgumentException("dir");
            string html = null;
            C.Workbook workbook = new C.Workbook(dir);
            C.HtmlSaveOptions options = new C.HtmlSaveOptions(C.SaveFormat.Html);
            options.ExportImagesAsBase64 = true;

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Save(ms, options);
                ms.Position = 0;
                html = new StreamReader(ms).ReadToEnd();
            }
            return html;
        }

        public static string HtmlWithPdf(string dir) 
        {
            var doc = org.apache.pdfbox.pdmodel.PDDocument.load(dir);
            var conv = new org.apache.pdfbox.util.PDFText2HTML("utf-8");
            string html = conv.getText(doc);
            doc.close();
            return html;
        }

        public static string HtmlWithPdfUsePdf2txt(string dir)
        {
            if (!dir.EndsWith(".pdf"))
                throw new ArgumentException("dir");

            System.Diagnostics.Process cmd = new System.Diagnostics.Process();

            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = @"/c pdf2txt.py -t html -c gb2312 " + dir;
            cmd.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("gb2312");

            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;

            cmd.Start();

            string html = cmd.StandardOutput.ReadToEnd();
            cmd.Close();
            return html;
        }
    }
}