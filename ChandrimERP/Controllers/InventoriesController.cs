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
    public class InventoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Inventories
        public ActionResult Index()
        {
            var inventory = db.Inventory.Include(i => i.Company).Include(i => i.Warehouse);
            return View(inventory.ToList());
        }

        // GET: Inventories/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventory.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // GET: Inventories/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName");
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemId,ItemCode,BalanceQuantity,AleartLavel,ItemMRP,ItemUnitCost,ItemEditionalCost,ItemTotalCost,ItemAvrageCost,UpdateDate,CreatedOn,Status,CompanyId,WarehouseId")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.Id = Guid.NewGuid();
                db.Inventory.Add(inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", inventory.CompanyId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", inventory.WarehouseId);
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventory.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", inventory.CompanyId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", inventory.WarehouseId);
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", inventory.CompanyId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", inventory.WarehouseId);
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventory.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Inventory inventory = db.Inventory.Find(id);
            db.Inventory.Remove(inventory);
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
