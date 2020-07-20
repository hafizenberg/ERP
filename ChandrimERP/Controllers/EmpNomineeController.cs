using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class EmpNomineeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmpNominee
        public ActionResult Index()
        {
            var employeeNomineeInfo = db.EmployeeNomineeInfo.Include(e => e.Employee);
            return View(employeeNomineeInfo.ToList());
        }

        // GET: EmpNominee/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeNomineeInfo employeeNomineeInfo = db.EmployeeNomineeInfo.Find(id);
            if (employeeNomineeInfo == null)
            {
                return HttpNotFound();
            }
            return View(employeeNomineeInfo);
        }

        public ActionResult Create()
        {
            ViewBag.EmpId = new SelectList(db.Employee, "Id", "EmailAddress");
            return View(new EmployeeNomineeInfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeNomineeInfo model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();


                if (model.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageUpload.FileName);
                    string extension = Path.GetExtension(model.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.ImageUrl = "~/Image/EmpNominee/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/EmpNominee/Image/"), fileName);
                    model.ImageUpload.SaveAs(fileName);
                }
                if (model.SignUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(model.SignUpload.FileName);
                    string extension = Path.GetExtension(model.SignUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.Signature = "~/Image/EmpNominee/Signature/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/EmpNominee/Signature/"), fileName);
                    model.SignUpload.SaveAs(fileName);
                }
                db.EmployeeNomineeInfo.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", "EmpNominee");
            }

            ViewBag.EmpId = new SelectList(db.Employee, "Id", "EmailAddress", model.EmpId);
            return View(model);
        }

        // GET: EmpNominee/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeNomineeInfo employeeNomineeInfo = db.EmployeeNomineeInfo.Find(id);
            if (employeeNomineeInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpId = new SelectList(db.Employee, "Id", "EmailAddress", employeeNomineeInfo.EmpId);
            return View(employeeNomineeInfo);
        }

        // POST: EmpNominee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NomineeName,NomineeDetails,Signature,DateOfBirth,Country,State,City,AddressLineOne,AddressLineTwo,CreatedOn,EmpId")] EmployeeNomineeInfo employeeNomineeInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeNomineeInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpId = new SelectList(db.Employee, "Id", "EmailAddress", employeeNomineeInfo.EmpId);
            return View(employeeNomineeInfo);
        }

        // GET: EmpNominee/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeNomineeInfo employeeNomineeInfo = db.EmployeeNomineeInfo.Find(id);
            if (employeeNomineeInfo == null)
            {
                return HttpNotFound();
            }
            return View(employeeNomineeInfo);
        }

        // POST: EmpNominee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            EmployeeNomineeInfo employeeNomineeInfo = db.EmployeeNomineeInfo.Find(id);
            db.EmployeeNomineeInfo.Remove(employeeNomineeInfo);
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
