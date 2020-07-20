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

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class LedgerCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LedgerCategories
        public ActionResult Index()
        {
            var ledgerCategory = db.LedgerCategory.Include(l => l.ChartOfAccount);
            return View(ledgerCategory.ToList());
        }

        // GET: LedgerCategories/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerCategory ledgerCategory = db.LedgerCategory.Find(id);
            if (ledgerCategory == null)
            {
                return HttpNotFound();
            }
            return View(ledgerCategory);
        }

        // GET: LedgerCategories/Create
        [Authorize(Roles = "companyAdmin, led_cat_create")]
        public ActionResult Create()
        {
            ViewBag.ParentLedgerCatId = new SelectList(db.LedgerCategory, "Id", "Name");
            ViewBag.ChartOfAccountId = new SelectList(db.ChartOfAccount, "Id", "Name");
            return View();
        }

        // POST: LedgerCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ChartOfAccountId,ParentLedgerCatId,CreatedOn")] LedgerCategory ledgerCategory)
        {
            if (ModelState.IsValid)
            {
                var isExist = IsDataExistt(ledgerCategory.Name);
                if (isExist)
                {
                    ledgerCategory.Id = Guid.NewGuid();
                    db.LedgerCategory.Add(ledgerCategory);

                    var ledgerid = ledgerCategory.Id;
                    var catid = ledgerCategory.ChartOfAccountId;
                    var lname = ledgerCategory.Name;
                    var comid = ledgerCategory.ChartOfAccount.Company.Id;
                    ChartTree ctree = new ChartTree();
                    ctree.id = ledgerid.ToString();
                    ctree.parent = catid.ToString();
                    ctree.text = lname;
                    ctree.type = "cat";
                    ctree.CompanyId = comid;
                    db.ChartTree.Add(ctree);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ParentLedgerCatId = new SelectList(db.LedgerCategory, "Id", "Name", ledgerCategory.ParentLedgerCatId);
            ViewBag.ChartOfAccountId = new SelectList(db.ChartOfAccount, "Id", "Name", ledgerCategory.ChartOfAccountId);
            return View(ledgerCategory);
        }
        [NonAction]
        public bool IsDataExistt(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.LedgerCategory
                        .Where(a => a.ChartOfAccount.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name == data);
                return v != null;
            }
        }
        // GET: LedgerCategories/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerCategory ledgerCategory = db.LedgerCategory.Find(id);
            if (ledgerCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChartOfAccountId = new SelectList(db.ChartOfAccount, "Id", "Name", ledgerCategory.ChartOfAccountId);
            return View(ledgerCategory);
        }

        // POST: LedgerCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ChartOfAccountId,ParentLedgerCatId,CreatedOn")] LedgerCategory ledgerCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ledgerCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChartOfAccountId = new SelectList(db.ChartOfAccount, "Id", "Name", ledgerCategory.ChartOfAccountId);
            return View(ledgerCategory);
        }

        // GET: LedgerCategories/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerCategory ledgerCategory = db.LedgerCategory.Find(id);
            if (ledgerCategory == null)
            {
                return HttpNotFound();
            }
            return View(ledgerCategory);
        }

        // POST: LedgerCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            LedgerCategory ledgerCategory = db.LedgerCategory.Find(id);
            db.LedgerCategory.Remove(ledgerCategory);
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
