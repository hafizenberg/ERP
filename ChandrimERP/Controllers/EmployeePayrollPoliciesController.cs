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
    public class EmployeePayrollPoliciesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmployeePayrollPolicies
        public ActionResult Index()
        {
            var employeePayrollPolicy = db.EmployeePayrollPolicy.Include(e => e.Employees);
            return View(employeePayrollPolicy.ToList());
        }

        // GET: EmployeePayrollPolicies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePayrollPolicy employeePayrollPolicy = db.EmployeePayrollPolicy.Find(id);
            if (employeePayrollPolicy == null)
            {
                return HttpNotFound();
            }
            return View(employeePayrollPolicy);
        }

        // GET: EmployeePayrollPolicies/Create
        public ActionResult Create()
        {
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmailAddress");
            return View();
        }

        // POST: EmployeePayrollPolicies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeePayrollPolicy employeePayrollPolicy)
        {
            if (ModelState.IsValid)
            {
                db.EmployeePayrollPolicy.Add(employeePayrollPolicy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmailAddress", employeePayrollPolicy.EmployeesId);
            return View(employeePayrollPolicy);
        }

        // GET: EmployeePayrollPolicies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePayrollPolicy employeePayrollPolicy = db.EmployeePayrollPolicy.Find(id);
            if (employeePayrollPolicy == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmailAddress", employeePayrollPolicy.EmployeesId);
            return View(employeePayrollPolicy);
        }

        // POST: EmployeePayrollPolicies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PpolicyName,Ppay,Pstatus,Paccount,EmployeesId")] EmployeePayrollPolicy employeePayrollPolicy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeePayrollPolicy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmailAddress", employeePayrollPolicy.EmployeesId);
            return View(employeePayrollPolicy);
        }

        // GET: EmployeePayrollPolicies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePayrollPolicy employeePayrollPolicy = db.EmployeePayrollPolicy.Find(id);
            if (employeePayrollPolicy == null)
            {
                return HttpNotFound();
            }
            return View(employeePayrollPolicy);
        }

        // POST: EmployeePayrollPolicies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeePayrollPolicy employeePayrollPolicy = db.EmployeePayrollPolicy.Find(id);
            db.EmployeePayrollPolicy.Remove(employeePayrollPolicy);
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
