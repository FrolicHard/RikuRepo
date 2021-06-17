using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using FileSys.Models;


namespace FileSys.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
                else if (file.ContentLength > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 3; //3 MB
                    string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".txt" };

                    if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ModelState.AddModelError("File", "Please input a file of one of the following types: " + string.Join(", ", AllowedFileExtensions));
                    }

                    else if (file.ContentLength > MaxContentLength)
                    {
                        ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                    }
                    else
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                        file.SaveAs(path);
                        ModelState.Clear();
                    }
                }
            }
            return View();
        }

        public ActionResult Downloads()
        {
            //Hae kaikki kansion dokkarit
            string[] filePaths = Directory.GetFiles(Server.MapPath("~/Content/Upload"));

            //Kopioi tiedostonimet Modeliin
            List<FileModel> files = new List<FileModel>();
            foreach (string filePath in filePaths)
            {
                files.Add(new FileModel { FileName = Path.GetFileName(filePath) });
            }
            //Palauta lista modeliin viedyistä nimistä äkymään
            return View(files);
        }

        public FileResult DownloadFile(string fileName)
        {
            string path = Server.MapPath("~/Content/Upload/") + fileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}