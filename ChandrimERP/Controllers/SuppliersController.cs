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
    public class SuppliersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Suppliers
        [Authorize(Roles = "companyAdmin, sup_index")]
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var supplier = db.Supplier.Include(s => s.Company).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            return View(supplier.ToList());
        }
        public ActionResult GetList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Supplier>();
                jsonData = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x=>x.Status==true).ToList();

                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }

            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }
        // GET: Suppliers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Supplier.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        [Authorize(Roles = "companyAdmin, sup_create")]
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();

            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");

            return View(new Supplier());
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierId,SupplierCode,OpeningBlance,CompanyName,JobTitle,ContactFirstName,ContactLastName,Genders,BussinessPhone,MobilePhone,AddressLineOne,AddressLineTwo,FaxNumber,Country,State,City,ZipOrPostalCode,ImageUrl,Email,Website,Notes,Status,Islocked,LockedDateTime,CreatedOn,CompanyId")] Supplier model, HttpPostedFileBase ImageUpload)
        {
            var username = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //Use it in your post method
                var isExist = IsDataExist(model.BussinessPhone);
                if (isExist)
                {
                    ModelState.AddModelError("PhoneNoExist", "Phone Number is already exist.");
                    var companyLists = db.Company
                       .Where(a => a.ApplicationUser_Company
                       .Any(c => c.ApplicationUser_Id == username))
                       .ToList();
                    ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", model.CompanyId);
                    return View(model);
                }

                if (ImageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => s.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");
                    string directoryPath = "~/UploadedFiles/" + companyname + "/Supplier_Image/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                        Directory.CreateDirectory(Server.MapPath(directoryPath));

                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    ImageUpload.SaveAs(fileName);
                }

                var rowcount = db.Supplier.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

                if (rowcount > 0)
                {
                    var supcode = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.SupplierCode);
                    model.SupplierCode = supcode + 1;
                }
                else
                {
                    model.SupplierCode = 1000;
                }

                model.SupplierId = Guid.NewGuid();
                db.Supplier.Add(model);
                db.SaveChanges();

                var Id = model.SupplierId;
                var lcat = db.LedgerCategory.Where(a => a.ChartOfAccount.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().Where(x => x.Name == "Accounts Payable (Supplier)").Select(s => s.Id).SingleOrDefault();
                Ledger lg = new Ledger();
               
                var ledgerRowcount = db.Ledger.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)); ;

                if (ledgerRowcount > 0)
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
                lg.Name = model.CompanyName + ' '+'('+"SI"+ model.SupplierCode + ')';
                lg.Address = model.Country + ',' + model.State + ',' + model.City;
                lg.EffectInventory = false;
                lg.EffectPayrool = false;
                lg.Country = model.Country;
                lg.State = model.State;
                lg.City = model.City;
                lg.OpeningBalance = model.OpeningBlance;
                lg.PhoneNo = model.BussinessPhone;
                lg.RefType = "Supplier";
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
                    tr.BranchID = db.Branch.Where(x => x.CompanyId == model.CompanyId).OrderBy(o => o.CreatedOn).Select(s => s.Id).ToList().FirstOrDefault();
                    tr.VoucherNo = 1000;
                    tr.VoucherType = "Opening Balance";
                    tr.TrasactionalAmount = Convert.ToDecimal(openingbalance);
                    tr.TransactionDate = DateTime.Now;

                    db.Transaction.Add(tr);


                    TransactionDetails td = new TransactionDetails();

                    var balance = openingbalance.ToString().ToCharArray();
                    if (balance[0]== '-')
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

            var companyList = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", model.CompanyId);
            return View(model);
        }
        [NonAction]
        public bool IsDataExist(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.Supplier
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.BussinessPhone == data);
                return v != null;
            }
        }
        // GET: Suppliers/Edit/5
        public ActionResult UpdateSupplier(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Supplier.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", supplier.CompanyId);
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSupplier(Supplier supplier)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                Supplier s = new Supplier();
                s= db.Supplier.FirstOrDefault(f => f.SupplierId == supplier.SupplierId);
                var oldfilepath = db.Company.Where(x => x.Id == supplier.SupplierId).Select(sd => sd.CompanyLogo).FirstOrDefault();

                if (supplier.ImageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(d =>d.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");

                    string directoryPath = "~/UploadedFiles/" + companyname + "/Supplier_Image/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                    {
                        Directory.CreateDirectory(Server.MapPath(directoryPath));
                    }
                    string fileName = Path.GetFileNameWithoutExtension(supplier.ImageUpload.FileName);
                    string extension = Path.GetExtension(supplier.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    supplier.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    supplier.ImageUpload.SaveAs(fileName);
                    string fullpath = Request.MapPath(oldfilepath);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                }
                s.ImageUrl = supplier.ImageUrl;
                s.ContactFirstName = supplier.ContactFirstName;
                s.ContactLastName = supplier.ContactLastName;
                s.AddressLineOne = supplier.AddressLineOne;
                s.AddressLineTwo = supplier.AddressLineTwo;
                s.Website = supplier.Website;
                s.Status = supplier.Status;
                s.Notes = supplier.Notes;
                s.NationalId = supplier.NationalId;
                s.TinNumber = supplier.TinNumber;
                db.Entry(s).State=EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Supplier.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Supplier supplier = db.Supplier.Find(id);
            db.Supplier.Remove(supplier);
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
