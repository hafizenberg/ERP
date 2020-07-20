using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class ProductSubCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductSubCategories
        public ActionResult Index()
        {
            var productSubCategory = db.ProductSubCategory.Include(p => p.ProductCategory);
            return View(productSubCategory.ToList());
        }

        // GET: ProductSubCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSubCategory productSubCategory = db.ProductSubCategory.Find(id);
            if (productSubCategory == null)
            {
                return HttpNotFound();
            }
            return View(productSubCategory);
        }

        // GET: ProductSubCategories/Create
        public ActionResult Create()
        {
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategory, "Id", "Name");
            return View();
        }

        // POST: ProductSubCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductCategoryId,Name")] ProductSubCategory productSubCategory)
        {
            if (ModelState.IsValid)
            {
                db.ProductSubCategory.Add(productSubCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductCategoryId = new SelectList(db.ProductCategory, "Id", "Name", productSubCategory.ProductCategoryId);
            return View(productSubCategory);
        }

        // GET: ProductSubCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSubCategory productSubCategory = db.ProductSubCategory.Find(id);
            if (productSubCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategory, "Id", "Name", productSubCategory.ProductCategoryId);
            return View(productSubCategory);
        }

        // POST: ProductSubCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductCategoryId,Name")] ProductSubCategory productSubCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productSubCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategory, "Id", "Name", productSubCategory.ProductCategoryId);
            return View(productSubCategory);
        }

        // GET: ProductSubCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSubCategory productSubCategory = db.ProductSubCategory.Find(id);
            if (productSubCategory == null)
            {
                return HttpNotFound();
            }
            return View(productSubCategory);
        }

        // POST: ProductSubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductSubCategory productSubCategory = db.ProductSubCategory.Find(id);
            db.ProductSubCategory.Remove(productSubCategory);
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
