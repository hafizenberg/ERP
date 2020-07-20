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
    public class VouchersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Vouchers
        public ActionResult Index()
        {
            return View(db.Voucher.ToList());
        }
        // GET: Vouchers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Voucher.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // GET: Vouchers/Create
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            return View();
        }

        // POST: Vouchers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Voucher voucher)
        {
            var username = User.Identity.GetUserId();
            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName", voucher.CompanyId);
            if (ModelState.IsValid)
            {
                voucher.Id = Guid.NewGuid();
                db.Voucher.Add(voucher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(voucher);
        }

        // GET: Vouchers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Voucher.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // POST: Vouchers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VoucherType,Name,Prefix,CreatedOn")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voucher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(voucher);
        }

        // GET: Vouchers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Voucher.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // POST: Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Voucher voucher = db.Voucher.Find(id);
            db.Voucher.Remove(voucher);
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
