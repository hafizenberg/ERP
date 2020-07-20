using ChandrimERP.Models;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Data.Entity;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Data.Entity.Core;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult UserInformation()
        {
            return View();
        }

        public ActionResult Index(int page = 1, string search = "", string search2 = "", string search3 = "", string search4 = "")
        {

            var username = User.Identity.GetUserId();
            var compid = db.ApplicationUser_Company.Where(x => x.ApplicationUser_Id == username).Select(s => s.Company_Id).SingleOrDefault();
            ViewBag.TotalUser = db.Users.Count(a => a.ApplicationUser_Company.Any(c => c.Company_Id == compid));
            ViewBag.TotalCustomer = db.Customers.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            ViewBag.TotalSupplier = db.Supplier.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            ViewBag.TotalProduct = db.Products.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

            int pageSize = 3;
            int pageSize2 = 3;
            int pageSize3 = 3;
            int pageSize4 = 3;
            int totalRecord = 0;
            int totalRecord2 = 0;
            int totalRecord3 = 0;
            int totalRecord4 = 0;
            if (page < 1) page = 1;
            int skip = (page * pageSize) - pageSize;
            int skip2 = (page * pageSize) - pageSize;
            int skip3 = (page * pageSize) - pageSize;
            int skip4 = (page * pageSize) - pageSize;

            ViewBag.data = GetSupplier(search, skip, pageSize, out totalRecord);
            ViewBag.data2 = GetCustomer(search2, skip2, pageSize2, out totalRecord2);
            ViewBag.data3 = GetProduct(search3, skip3, pageSize3, out totalRecord3);
            ViewBag.data4 = GetEmployee(search4, skip4, pageSize4, out totalRecord4);

            ViewBag.TotalRows = totalRecord;
            ViewBag.TotalRows2 = totalRecord2;
            ViewBag.TotalRows3 = totalRecord3;
            ViewBag.TotalRows4 = totalRecord4;

            ViewBag.Search = search;
            ViewBag.Search2 = search2;
            ViewBag.Search3 = search3;
            ViewBag.Search4 = search4;

            return View(db.Supplier.OrderBy(e => e.SupplierCode));
        }

        public ActionResult GetCustomer()
        {
            var username = User.Identity.GetUserId();
            var customerList = db.Customers
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            return View(customerList);
        }

        public ActionResult ProfileUpdate(string id)
        {
            var context = new ApplicationDbContext();
            var username = User.Identity.GetUserId();
            var UserMail = User.Identity.GetUserName();
            if (!string.IsNullOrEmpty(username))
            {
                var user = context.UserDetails.SingleOrDefault(u => u.Id == username);
                if (user != null)
                {
                    var fullName = string.Concat(new string[] { user.FirstName, " ", user.LastName });
                    String Id = user.Id;
                    ViewBag.UserFullName = fullName;
                    ViewBag.ProfileUpdateId = Id;
                }
            }
            ViewBag.UserMail = UserMail;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetails userDetails = db.UserDetails.Find(id);
            if (userDetails == null)
            {
                return HttpNotFound();
            }
            return View(userDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfileUpdate(UserDetails userDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userDetails).State = EntityState.Modified;

                if (userDetails.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(userDetails.ImageUpload.FileName);
                    string extension = Path.GetExtension(userDetails.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    userDetails.PhotosUrl = "~/Image/user_logo/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/user_logo/"), fileName);
                    userDetails.ImageUpload.SaveAs(fileName);
                }

                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(userDetails);
        }
        public List<Supplier> GetSupplier(string search, int skip, int pageSize, out int totalRecord)
        {

            var username = User.Identity.GetUserId();
            var supplier = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            var v = (from a in supplier where a.ContactFirstName.Contains(search) || a.ContactLastName.Contains(search) || a.BussinessPhone.Contains(search) select a);
            totalRecord = v.Count();
            v = v.OrderBy(a => a.ContactFirstName);
            if (pageSize > 0)
            {
                v = v.Skip(skip).Take(pageSize);
            }
            return v.ToList();

        }
        public List<Customer> GetCustomer(string search2, int skip, int pageSize, out int totalRecord)
        {
            var username = User.Identity.GetUserId();
            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            var v = (from a in customer where a.ContactFirstName.Contains(search2) || a.ContactLastName.Contains(search2) || a.Phone.Contains(search2) select a);
            totalRecord = v.Count();
            v = v.OrderBy(a => a.CustomerCode);
            if (pageSize > 0)
            {
                v = v.Skip(skip).Take(pageSize);
            }
            return v.ToList();
        }
        public List<Product> GetProduct(string search2, int skip, int pageSize, out int totalRecord)
        {
            var username = User.Identity.GetUserId();
            var customer = db.Products.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            var v = (from a in customer where a.ProductCode.ToString().Contains(search2) || a.ProductName.Contains(search2) || a.ProductCategory.Name.Contains(search2) select a);
            totalRecord = v.Count();
            v = v.OrderBy(a => a.ProductCode);
            if (pageSize > 0)
            {
                v = v.Skip(skip).Take(pageSize);
            }
            return v.ToList();
        }
        public List<EmployeePersionalInfo> GetEmployee(string search2, int skip, int pageSize, out int totalRecord)
        {
            var username = User.Identity.GetUserId();
            var employee = db.EmployeePersionalInfo.Where(a => a.Employees.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            var v = (from a in employee where a.Employees.EmployeeCode.ToString().Contains(search2) || a.FirstName.Contains(search2) || a.LastName.Contains(search2) || a.Employees.ImageUrl.Contains(search2) select a);
            totalRecord = v.Count();
            v = v.OrderBy(a => a.Employees.EmployeeCode);
            if (pageSize > 0)
            {
                v = v.Skip(skip).Take(pageSize);
            }
            return v.ToList();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult AnotherLink()
        {
            return View("Index");
        }
        public ActionResult GetBranchList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                //var jsonData = new List<Branch>();
                var jsonData = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new {
                    CompanyName = s.Company.CompanyName,
                    BranchName = s.BranchName,
                    BranchStatus = s.Status,
                    Address = s.Address,

                }).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }

        public JsonResult NewPieChart() //It will be fired from Jquery ajax call  
        {
            var username = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            var jsonData = db.Order.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            var ToReturn = jsonData.Select(S => new
            {
                InvoiceN = S.InvoiceNo,
                InvoicedA = S.InvoicedAmount
            });

            return Json(ToReturn, JsonRequestBehavior.AllowGet);

        }

        public ActionResult NewChart()
        {
            var username = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            var jsonData = db.SalesOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            var ToReturn = jsonData.Select(S => new
            {
                InvoiceN = S.InvoiceNo,
                InvoicedA = S.InvoicedAmount
            });
            return Json(ToReturn, JsonRequestBehavior.AllowGet);
        }
    }
}
