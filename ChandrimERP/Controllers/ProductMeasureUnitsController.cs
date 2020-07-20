using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;

namespace ChamdrimERP.Controllers
{
    public class ProductMeasureUnitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductMeasureUnits
        public ActionResult Index()
        {
            var productMeasureUnit = db.ProductMeasureUnit.Include(p => p.Company);
            return View(productMeasureUnit.ToList());
        }

        // GET: ProductMeasureUnits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMeasureUnit productMeasureUnit = db.ProductMeasureUnit.Find(id);
            if (productMeasureUnit == null)
            {
                return HttpNotFound();
            }
            return View(productMeasureUnit);
        }

        // GET: ProductMeasureUnits/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");
            return View();
        }

        // POST: ProductMeasureUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Note,CompanyId")] ProductMeasureUnit productMeasureUnit)
        {
            if (ModelState.IsValid)
            {
                db.ProductMeasureUnit.Add(productMeasureUnit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", productMeasureUnit.CompanyId);
            return View(productMeasureUnit);
        }

        // GET: ProductMeasureUnits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMeasureUnit productMeasureUnit = db.ProductMeasureUnit.Find(id);
            if (productMeasureUnit == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", productMeasureUnit.CompanyId);
            return View(productMeasureUnit);
        }

        // POST: ProductMeasureUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Note,CompanyId")] ProductMeasureUnit productMeasureUnit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productMeasureUnit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", productMeasureUnit.CompanyId);
            return View(productMeasureUnit);
        }

        // GET: ProductMeasureUnits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMeasureUnit productMeasureUnit = db.ProductMeasureUnit.Find(id);
            if (productMeasureUnit == null)
            {
                return HttpNotFound();
            }
            return View(productMeasureUnit);
        }

        // POST: ProductMeasureUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductMeasureUnit productMeasureUnit = db.ProductMeasureUnit.Find(id);
            db.ProductMeasureUnit.Remove(productMeasureUnit);
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
