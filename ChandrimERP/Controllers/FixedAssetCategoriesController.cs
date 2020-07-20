using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;

namespace ChamdrimERP.Controllers
{
    public class FixedAssetCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FixedAssetCategories
        public ActionResult Index()
        {
            var fixedAssetCategory = db.FixedAssetCategory.Include(f => f.Company).Include(f => f.Ledger);
            return View(fixedAssetCategory.ToList());
        }

        // GET: FixedAssetCategories/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FixedAssetCategory fixedAssetCategory = db.FixedAssetCategory.Find(id);
            if (fixedAssetCategory == null)
            {
                return HttpNotFound();
            }
            return View(fixedAssetCategory);
        }
        [Authorize(Roles = "companyAdmin, fix_a_c_create")]
        // GET: FixedAssetCategories/Create
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            var ledgerlist = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.LedgerId = new SelectList(ledgerlist, "Id", "Name");
            return View();
        }

        // POST: FixedAssetCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,LedgerId,CompanyId")] FixedAssetCategory fixedAssetCategory)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var isExist = IsDataExist(fixedAssetCategory.Name);
                if (isExist)
                {
                    ModelState.AddModelError("NameExist", "This name is already exist.");
                    var companyLists = db.Company
                   .Where(a => a.ApplicationUser_Company
                   .Any(c => c.ApplicationUser_Id == username))
                   .ToList();
                    ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", fixedAssetCategory.CompanyId);
                    var ledgerlists = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.LedgerId = new SelectList(ledgerlists, "Id", "Name",fixedAssetCategory.LedgerId);
                    return View(new FixedAssetCategory());
                }
                fixedAssetCategory.Id = Guid.NewGuid();
                db.FixedAssetCategory.Add(fixedAssetCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var companyList = db.Company
                   .Where(a => a.ApplicationUser_Company
                   .Any(c => c.ApplicationUser_Id == username))
                   .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", fixedAssetCategory.CompanyId);
            var ledgerlist = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.LedgerId = new SelectList(ledgerlist, "Id", "Name", fixedAssetCategory.LedgerId);
            return View(fixedAssetCategory);
        }
        [NonAction]
        public bool IsDataExist(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.FixedAssetCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                        .ToList().Where(a => a.Name == data).FirstOrDefault();
                return v != null;
            }
        }
        // GET: FixedAssetCategories/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FixedAssetCategory fixedAssetCategory = db.FixedAssetCategory.Find(id);
            if (fixedAssetCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", fixedAssetCategory.CompanyId);
            ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name", fixedAssetCategory.LedgerId);
            return View(fixedAssetCategory);
        }

        // POST: FixedAssetCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,LedgerId,CompanyId")] FixedAssetCategory fixedAssetCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fixedAssetCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", fixedAssetCategory.CompanyId);
            ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name", fixedAssetCategory.LedgerId);
            return View(fixedAssetCategory);
        }

        // GET: FixedAssetCategories/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FixedAssetCategory fixedAssetCategory = db.FixedAssetCategory.Find(id);
            if (fixedAssetCategory == null)
            {
                return HttpNotFound();
            }
            return View(fixedAssetCategory);
        }

        // POST: FixedAssetCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            FixedAssetCategory fixedAssetCategory = db.FixedAssetCategory.Find(id);
            db.FixedAssetCategory.Remove(fixedAssetCategory);
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
