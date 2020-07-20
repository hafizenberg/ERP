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
    public class EmployeePropInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmployeePropInfoes
        public ActionResult Index()
        {
            var employeePropInfo = db.EmployeePropInfo.Include(e => e.Employees);
            return View(employeePropInfo.ToList());
        }

        // GET: EmployeePropInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePropInfo employeePropInfo = db.EmployeePropInfo.Find(id);
            if (employeePropInfo == null)
            {
                return HttpNotFound();
            }
            return View(employeePropInfo);
        }

        // GET: EmployeePropInfoes/Create
        public ActionResult Create()
        {
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode");
            return View();
        }

        // POST: EmployeePropInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmployeeJoiningDate,SalaryTypes,EmpBasicSalary,EmployeesId")] EmployeePropInfo employeePropInfo)
        {
            if (ModelState.IsValid)
            {
                db.EmployeePropInfo.Add(employeePropInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode", employeePropInfo.EmployeesId);
            return View(employeePropInfo);
        }

        // GET: EmployeePropInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePropInfo employeePropInfo = db.EmployeePropInfo.Find(id);
            if (employeePropInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode", employeePropInfo.EmployeesId);
            return View(employeePropInfo);
        }

        // POST: EmployeePropInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmployeeJoiningDate,SalaryTypes,EmpBasicSalary,EmployeesId")] EmployeePropInfo employeePropInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeePropInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode", employeePropInfo.EmployeesId);
            return View(employeePropInfo);
        }

        // GET: EmployeePropInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePropInfo employeePropInfo = db.EmployeePropInfo.Find(id);
            if (employeePropInfo == null)
            {
                return HttpNotFound();
            }
            return View(employeePropInfo);
        }

        // POST: EmployeePropInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeePropInfo employeePropInfo = db.EmployeePropInfo.Find(id);
            db.EmployeePropInfo.Remove(employeePropInfo);
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
