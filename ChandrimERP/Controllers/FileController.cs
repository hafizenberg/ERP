using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;

namespace ChamdrimERP.Controllers
{
    public class FileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "companyAdmin, fil_index")]
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var document = db.Document.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            return View(document.ToList());
        }
        public ActionResult GetDocumentList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = db.Document.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new
                {
                    DocumentCode = s.DocumentCode,
                    DocumentName = s.DocumentName,
                    Type = s.RefType,
                    ExpDate = s.EXPDate,
                    DocumentDetails = s.DocumentDetails,
                    //Name = (db.Customers.Where(x => x.CustomerId == s.RefId).Select(a => (a.CompanyName + " " + a.ContactFirstName + " " + a.ContactLastName)).SingleOrDefault() + db.Supplier.Where(x => x.SupplierId == s.RefId).Select(a => (a.CompanyName + " " + a.ContactFirstName + " " + a.ContactLastName)).SingleOrDefault() + db.Employee.Where(x => x.Id == s.RefId).Select(a => (a.EmployeePersionalInfo.Select(b => b.FirstName + " " + b.LastName))).SingleOrDefault() + db.Company.Where(x => x.Id == s.RefId).Select(a => (a.CompanyName + " " + a.ContactFirstName + " " + a.ContactLastName)).SingleOrDefault() + db.Branch.Where(x => x.Id == s.RefId).Select(a => a.BranchName).SingleOrDefault()),
                    Name = s.RefId,
                    Image = s.Images.Where(a => a.DocumentId == s.Id).Select(a => new
                    {
                        ImageUrl = a.DocumentPath,
                        Extension = a.DocType
                    })
                }).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }

        [Authorize(Roles = "companyAdmin, fil_uploadesfile")]
        public ActionResult UploadFiles()
        {
            var username = User.Identity.GetUserId();
            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().Select(s => new
            {
                Text = s.CompanyName + " " + s.ContactFirstName + " " + s.ContactLastName + " " + "(" + s.Phone + ")",
                Value = s.CustomerId
            });
            ViewBag.CustomerId = new SelectList(customer, "Value", "Text");

            IEnumerable<Company> company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");

            ViewBag.GetCompanyId = new SelectList(company, "Id", "CompanyName");

            var employeename = db.EmployeePersionalInfo.Where(x => x.Employees.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().Select(s => new
            {
                Text = s.FirstName + " " + s.LastName + " " + "(" + s.Employees.BussinessPhone + ")",
                Value = s.Employees.Id
            });
            ViewBag.EmployeeId = new SelectList(employeename, "Value", "Text");


            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");

            var supplier = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().Select(s => new
            {
                Text = s.CompanyName + " " + s.ContactFirstName + " " + s.ContactLastName + " " + "(" + s.BussinessPhone + ")",
                Value = s.SupplierId
            });
            ViewBag.SupplierId = new SelectList(supplier, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] files, Document document)
        {

            if (ModelState.IsValid)
            {   //iterating through multiple file collection  

                Document doc = new Document();
                doc.Id = Guid.NewGuid();
                doc.DocumentCode = document.DocumentCode;
                doc.DocumentName = document.DocumentName;
                doc.DocumentDetails = document.DocumentDetails;
                doc.RefId = document.RefId;
                doc.RefType = document.RefType;
                doc.EXPDate = document.EXPDate;
                doc.CompanyId = document.CompanyId;
                db.Document.Add(doc);

                var docid = doc.Id;

                foreach (HttpPostedFileBase file in files)
                {
                    try
                    {
                        var username = User.Identity.GetUserId();
                        var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c=>c.ApplicationUser_Id == username)).Select(s => s.CompanyName).FirstOrDefault();
                        companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");
                        string year = DateTime.Now.ToString("yyyy"); 
                        string month = DateTime.Now.ToString("MMMM");

                        string directoryPath= "~/UploadedFiles/" + companyname + "/"+year+"/"+month+"/";
                        bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                        if (!folderExists)
                        Directory.CreateDirectory(Server.MapPath(directoryPath));
                        var supportedTypes = new[] {"txt", "doc", "docx", "pdf", "xls", "xlsx","ppt","pptx","png","jpg","jpeg","gif","ttf","svg"};
                        string fileNameExt = Path.GetExtension(file.FileName).Substring(1);

                        if (!supportedTypes.Contains(fileNameExt))
                        {
                            ViewBag.UploadStatus = "File Extension Is InValid - Only Upload WORD/PDF/EXCEL/TXT File";
                        }
                        
                        else if (supportedTypes.Contains(fileNameExt))
                        {
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            fileName = fileName.Replace(" ", "-");
                            string extension = Path.GetExtension(file.FileName);
                            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                            var ServerSavePath = Path.Combine(Server.MapPath(directoryPath) + fileName);
                            //Save file to server folder  
                            file.SaveAs(ServerSavePath);
                            //assigning file uploaded status to ViewBag for showing message to user.  
                            ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                            Image docsimage = new Image();
                            docsimage.ID = Guid.NewGuid();
                            docsimage.DocumentPath = directoryPath + fileName;
                            docsimage.DocType = extension;
                            docsimage.Title = doc.DocumentName;
                            docsimage.DocumentId = docid;
                            docsimage.RefId = document.RefId.ToString();
                            db.Image.Add(docsimage);
                        }
                        else
                        {
                            ViewBag.UploadStatus = "Unknown file format";
                            return View();
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.UploadStatus = "No file selected"+ex;
                        return View();
                    }
                }
            }
            
            db.SaveChanges();
            ModelState.Clear();
            return View();
        }
    }
}