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
    public class LedgersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ledgers
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var ledger = db.Ledger.Include(l => l.LedgerCategory).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            return View(ledger.ToList());
        }

        // GET: Ledgers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ledger ledger = db.Ledger.Find(id);
            if (ledger == null)
            {
                return HttpNotFound();
            }
            return View(ledger);
        }
        //GET: Ledgers/Create
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.ParentLedgerId = new SelectList(ledger, "Id", "Name");

            var ledgercat = db.LedgerCategory.Where(a => a.ChartOfAccount.Company
            .ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s =>
           new {
               Text = s.ChartOfAccount.Name + " -> " + "-" + s.Name,
               value = s.Id
           });

            ViewBag.LedgerCategoryId = new SelectList(ledgercat, "value", "Text");
            return View();
        }

        // POST: Ledgers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Ledger ledger)
        {
            var username = User.Identity.GetUserId();
            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName", ledger.CompanyId);
            ViewBag.ParentLedgerId = new SelectList(db.Ledger, "Id", "Name", ledger.ParentLedgerId);
            ViewBag.LedgerCategoryId = new SelectList(db.LedgerCategory, "Id", "Name", ledger.LedgerCategoryId);

            if (ModelState.IsValid)
            {

                var ledgeRrowcount = db.Ledger.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

                if (ledgeRrowcount > 0)
                {
                    var ledgerCode = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.LedgerCode);
                    ledger.LedgerCode = ledgerCode + 1;
                }
                else
                {
                    ledger.LedgerCode = 1000;
                }

                ledger.Id = Guid.NewGuid();
                db.Ledger.Add(ledger);


                var ledgerid = ledger.Id;
                var catid = ledger.LedgerCategoryId;
                var lname = ledger.Name;
                var comid = ledger.CompanyId;
                ChartTree ctree = new ChartTree();
                ctree.id = ledgerid.ToString();
                ctree.parent = catid.ToString();
                ctree.text = lname;
                ctree.isLedger = true;
                ctree.type = "ledger";
                ctree.CompanyId = comid;

                db.ChartTree.Add(ctree);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(ledger);
        }

        // GET: Ledgers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ledger ledger = db.Ledger.Find(id);
            if (ledger == null)
            {
                return HttpNotFound();
            }
            ViewBag.LedgerCategoryId = new SelectList(db.LedgerCategory, "Id", "Name", ledger.LedgerCategoryId);
            return View(ledger);
        }

        // POST: Ledgers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,LedgerCategoryId,ParentLedgerId,EffectInventory,EffectPayrool,OpeningBalance,Address,Country,State,City,PhoneNo,Email,Status,Islocked,LockedDateTime,CreatedOn,RefType,RefNo")] Ledger ledger)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ledger).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LedgerCategoryId = new SelectList(db.LedgerCategory, "Id", "Name", ledger.LedgerCategoryId);
            return View(ledger);
        }

        // GET: Ledgers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ledger ledger = db.Ledger.Find(id);
            if (ledger == null)
            {
                return HttpNotFound();
            }
            return View(ledger);
        }

        // POST: Ledgers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Ledger ledger = db.Ledger.Find(id);
            db.Ledger.Remove(ledger);
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
