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
    public class ProductRacksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductRacks
        public ActionResult Index()
        {
            return View(db.ProductRack.ToList());
        }

        // GET: ProductRacks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRack productRack = db.ProductRack.Find(id);
            if (productRack == null)
            {
                return HttpNotFound();
            }
            return View(productRack);
        }

        // GET: ProductRacks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductRacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] ProductRack productRack)
        {
            if (ModelState.IsValid)
            {
                db.ProductRack.Add(productRack);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productRack);
        }

        // GET: ProductRacks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRack productRack = db.ProductRack.Find(id);
            if (productRack == null)
            {
                return HttpNotFound();
            }
            return View(productRack);
        }

        // POST: ProductRacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] ProductRack productRack)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productRack).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productRack);
        }

        // GET: ProductRacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRack productRack = db.ProductRack.Find(id);
            if (productRack == null)
            {
                return HttpNotFound();
            }
            return View(productRack);
        }

        // POST: ProductRacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductRack productRack = db.ProductRack.Find(id);
            db.ProductRack.Remove(productRack);
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
