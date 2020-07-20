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
    public class EmployeePersionalInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmployeePersionalInfoes
        public ActionResult Index()
        {
            var employeePersionalInfo = db.EmployeePersionalInfo.Include(e => e.Employees);
            return View(employeePersionalInfo.ToList());
        }

        // GET: EmployeePersionalInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePersionalInfo employeePersionalInfo = db.EmployeePersionalInfo.Find(id);
            if (employeePersionalInfo == null)
            {
                return HttpNotFound();
            }
            return View(employeePersionalInfo);
        }

        // GET: EmployeePersionalInfoes/Create
        public ActionResult Create()
        {
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode");
            return View();
        }

        // POST: EmployeePersionalInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,DateOfBirth,Genders,BloodGroup,HomePhone,MobilePhone,Country,State,City,AddressLineOne,AddressLineTwo,ZipOrPostalCode,EmployeesId")] EmployeePersionalInfo employeePersionalInfo)
        {
            if (ModelState.IsValid)
            {
                db.EmployeePersionalInfo.Add(employeePersionalInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode", employeePersionalInfo.EmployeesId);
            return View(employeePersionalInfo);
        }

        // GET: EmployeePersionalInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePersionalInfo employeePersionalInfo = db.EmployeePersionalInfo.Find(id);
            if (employeePersionalInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode", employeePersionalInfo.EmployeesId);
            return View(employeePersionalInfo);
        }

        // POST: EmployeePersionalInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,DateOfBirth,Genders,BloodGroup,HomePhone,MobilePhone,Country,State,City,AddressLineOne,AddressLineTwo,ZipOrPostalCode,EmployeesId")] EmployeePersionalInfo employeePersionalInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeePersionalInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeesId = new SelectList(db.Employee, "Id", "EmployeeCode", employeePersionalInfo.EmployeesId);
            return View(employeePersionalInfo);
        }

        // GET: EmployeePersionalInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePersionalInfo employeePersionalInfo = db.EmployeePersionalInfo.Find(id);
            if (employeePersionalInfo == null)
            {
                return HttpNotFound();
            }
            return View(employeePersionalInfo);
        }

        // POST: EmployeePersionalInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeePersionalInfo employeePersionalInfo = db.EmployeePersionalInfo.Find(id);
            db.EmployeePersionalInfo.Remove(employeePersionalInfo);
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
