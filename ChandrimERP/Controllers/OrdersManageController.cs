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
    public class OrdersManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrdersManage
        public ActionResult Index()
        {
            var order = db.Order.Include(o => o.Branch).Include(o => o.Customer).Include(o => o.Ledger).Include(o => o.SalesAgent).Include(o => o.Warehouse);
            return View(order.ToList());
        }

        // GET: OrdersManage/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: OrdersManage/Create
        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerId", "CompanyName");
            ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name");
            ViewBag.SalesAgentId = new SelectList(db.SalesAgent, "Id", "Name");
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName");
            return View();
        }

        // POST: OrdersManage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderDate,InvoiceNo,CustomerID,LedgerId,DueDate,BranchId,WarehouseId,SalesAgentId,Narration,TotalQNT,VatAmount,InvoicedAmount,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid();
                db.Order.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName", order.BranchId);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerId", "CompanyName", order.CustomerID);
            ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name", order.LedgerId);
            ViewBag.SalesAgentId = new SelectList(db.SalesAgent, "Id", "Name", order.SalesAgentId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", order.WarehouseId);
            return View(order);
        }

        // GET: OrdersManage/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName", order.BranchId);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerId", "CompanyName", order.CustomerID);
            ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name", order.LedgerId);
            ViewBag.SalesAgentId = new SelectList(db.SalesAgent, "Id", "Name", order.SalesAgentId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", order.WarehouseId);
            return View(order);
        }

        // POST: OrdersManage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderDate,InvoiceNo,CustomerID,LedgerId,DueDate,BranchId,WarehouseId,SalesAgentId,Narration,TotalQNT,VatAmount,InvoicedAmount,UserId")]Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName", order.BranchId);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerId", "CompanyName", order.CustomerID);
            ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name", order.LedgerId);
            ViewBag.SalesAgentId = new SelectList(db.SalesAgent, "Id", "Name", order.SalesAgentId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", order.WarehouseId);
            return View(order);
        }

        // GET: OrdersManage/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: OrdersManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Order order = db.Order.Find(id);
            db.Order.Remove(order);
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
