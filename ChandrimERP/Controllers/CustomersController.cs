using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers
        [Authorize(Roles = "companyAdmin, cus_index")]
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)); 
            return View(customer.ToList());
        }
        public ActionResult GetCustomerListData()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Customer>();
                jsonData = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();

                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }

            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }
        public ActionResult GetCustomerList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Customer>();
                jsonData = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.Status == true).ToList();

                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }

            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }
        // GET: Customers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        [Authorize(Roles = "companyAdmin, cus_create")]
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            return View(new Customer());
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer model, HttpPostedFileBase imageUpload)
        {
            var username = User.Identity.GetUserId();
            
            if (ModelState.IsValid)
            {

                //Use it in your post method
                var isExist = IsDataExist(model.Phone);
                if (isExist)
                {
                    ModelState.AddModelError("NameExist", "Phone Number is already exist.");
                    var companyLists = db.Company
                       .Where(a => a.ApplicationUser_Company
                       .Any(c => c.ApplicationUser_Id == username))
                       .ToList();
                    ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", model.CompanyId);
                    return View(model);
                }
                model.CustomerId = Guid.NewGuid();
                
                if (imageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => s.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");
                    string directoryPath = "~/UploadedFiles/" + companyname + "/Customer/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                        Directory.CreateDirectory(Server.MapPath(directoryPath));

                    string fileName = Path.GetFileNameWithoutExtension(imageUpload.FileName);
                    string extension = Path.GetExtension(imageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    imageUpload.SaveAs(fileName);
                }
                if (model.CompanyName == null && model.IsCorporate == false)
                {
                    
                    model.CompanyName = model.ContactFirstName + " " + model.ContactLastName;
                }
                else if (model.CompanyName == null && model.IsCorporate == true)
                {
                    ModelState.AddModelError(model.CompanyName, "Company Name is required.");
                    var companyLists = db.Company
                       .Where(a => a.ApplicationUser_Company
                       .Any(c => c.ApplicationUser_Id == username))
                       .ToList();
                    ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", model.CompanyId);
                    return View(model);
                }
                var rowcount = db.Customers.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

                if (rowcount > 0)
                {
                    var cuscode = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.CustomerCode);
                    model.CustomerCode = cuscode + 1;
                }
                else
                {
                    model.CustomerCode = 1000;
                }

                db.Customers.Add(model);
                db.SaveChanges();
                var Id = model.CustomerId;

                var lcat = db.LedgerCategory.Where(a => a.ChartOfAccount.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().Where(x => x.Name == "Accounts Recieveable (Customer)").Select(s => s.Id).SingleOrDefault();

                Ledger lg = new Ledger();

                var ledgeRrowcount = db.Ledger.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

                if (ledgeRrowcount > 0)
                {
                    var LedgerCode = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.LedgerCode);
                    lg.LedgerCode = LedgerCode + 1;
                }
                else
                {
                    lg.LedgerCode = 1000;
                }

                lg.Id = Guid.NewGuid();
                lg.Email = model.Email;
                lg.Country = model.Country;
                lg.Name = model.CompanyName+ ' ' + '(' + model.CustomerCode + ')';
                lg.Address = model.Country + ',' + model.State + ',' + model.City;
                lg.EffectInventory = false;
                lg.EffectPayrool = false;
                lg.Country = model.Country;
                lg.State = model.State;
                lg.City = model.City;
                lg.OpeningBalance = model.OpeningBlance;
                lg.PhoneNo = model.Phone;
                lg.RefType = "Customer";
                lg.RefNo = Id;
                lg.CompanyId = model.CompanyId;
                lg.LedgerCategoryId = lcat;
                lg.isDefault = true;

                db.Ledger.Add(lg);

                var ledgerid = lg.Id;
                var openingbalance = lg.OpeningBalance;

                if (openingbalance != null || openingbalance != 0)
                {
                    Transaction tr = new Transaction();
                    tr.Id = Guid.NewGuid();
                    tr.UserId = username;
                    tr.BranchID = db.Branch.Where(x=>x.CompanyId== model.CompanyId).OrderBy(o => o.CreatedOn).Select(s=>s.Id).ToList().FirstOrDefault();
                    tr.VoucherNo = 1000;
                    tr.VoucherType = "Opening Balance";
                    tr.TrasactionalAmount = Convert.ToDecimal(openingbalance);
                    tr.TransactionDate = DateTime.Now;

                    db.Transaction.Add(tr);


                    TransactionDetails td = new TransactionDetails();

                    var balance = openingbalance.ToString().ToCharArray();
                    if (openingbalance != null) { 
                        if (balance[0] == '-')
                        {
                            var amount = openingbalance.ToString().Replace("-", "");
                            td.CreditAmount = Convert.ToDecimal(amount);
                        }
                        else
                        {

                            td.DebitAmount = openingbalance;
                        }
                    
                    td.Id = Guid.NewGuid();
                    td.VoucherNo = tr.VoucherNo;
                    td.VoucherType = tr.VoucherType;
                    td.BranchID = tr.BranchID;
                    td.TransactionDate = tr.TransactionDate;
                    td.TransactionID = tr.Id;
                    td.LedgerName = lg.Name;
                    td.LedgerNo = lg.LedgerCode.ToString();
                    td.LedgerID = ledgerid;

                    db.TransactionDetails.Add(td);
                    }
                }

                var id = lg.Id;
                var parent = lcat;
                var name = lg.Name;
                var comid = lg.CompanyId;
                ChartTree ctree = new ChartTree();
                ctree.id = id.ToString();
                ctree.parent = parent.ToString();
                ctree.text = name;
                ctree.isLedger = true;
                ctree.type = "ledger";
                ctree.CompanyId = comid;               
                db.ChartTree.Add(ctree);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", model.CompanyId);
            return View(model);
        }

        [NonAction]
        public bool IsDataExist(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                        .ToList().Where(a => a.Phone == data).FirstOrDefault();
                return v != null;
            }
        }

        // GET: Customers/Edit/5
        public ActionResult UpdateCustomer(Guid? id)
        {
            var username = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", customer.CompanyId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustomer(Customer customer)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                Customer s = new Customer();
                s = db.Customers.FirstOrDefault(f => f.CustomerId == customer.CustomerId);

                var oldfilepath = db.Company.Where(x => x.Id == customer.CustomerId).Select(sd => sd.ImageUrl).FirstOrDefault();

                if (customer.imageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(d => d.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");

                    string directoryPath = "~/UploadedFiles/" + companyname + "/Customer/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                    {
                        Directory.CreateDirectory(Server.MapPath(directoryPath));
                    }
                    string fileName = Path.GetFileNameWithoutExtension(customer.imageUpload.FileName);
                    string extension = Path.GetExtension(customer.imageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    customer.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    customer.imageUpload.SaveAs(fileName);
                    string fullpath = Request.MapPath(oldfilepath);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                }
                s.ImageUrl = customer.ImageUrl;
                s.BlanceLimit = customer.BlanceLimit;
                s.Email = customer.Email;
                s.WebPage = customer.WebPage;
                s.Country = customer.Country;
                s.State = customer.State;
                s.City = customer.City;
                s.TinNumber = customer.TinNumber;
                s.NationalId = customer.NationalId;
                s.AddressLineOne = customer.AddressLineOne;
                s.AddressLineTwo = customer.AddressLineTwo;
                s.Notes = customer.Notes;
                s.Status = customer.Status;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var companyList = db.Customers
           .Where(a => a.Company.ApplicationUser_Company
           .Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", customer.CompanyId);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
