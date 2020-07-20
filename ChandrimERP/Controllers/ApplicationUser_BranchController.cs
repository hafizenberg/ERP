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
    public class ApplicationUser_BranchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUser_Branch
        public ActionResult Index()
        {
            var applicationUser_Branch = db.ApplicationUser_Branch.Include(a => a.ApplicationUser).Include(a => a.Branch);
            return View(applicationUser_Branch.ToList());
        }

        // GET: ApplicationUser_Branch/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser_Branch applicationUser_Branch = db.ApplicationUser_Branch.Find(id);
            if (applicationUser_Branch == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser_Branch);
        }

        // GET: ApplicationUser_Branch/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUser_Id = new SelectList(db.ApplicationUsers, "Id", "Email");
            return View();
        }

        // POST: ApplicationUser_Branch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApplicationUser_Id,Branch_Id")] ApplicationUser_Branch applicationUser_Branch)
        {
            if (ModelState.IsValid)
            {
                db.ApplicationUser_Branch.Add(applicationUser_Branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUser_Id = new SelectList(db.ApplicationUsers, "Id", "Email", applicationUser_Branch.ApplicationUser_Id);
            ViewBag.Branch_Id = new SelectList(db.Branch, "Id", "BranchName", applicationUser_Branch.Branch_Id);
            return View(applicationUser_Branch);
        }

        // GET: ApplicationUser_Branch/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser_Branch applicationUser_Branch = db.ApplicationUser_Branch.Find(id);
            if (applicationUser_Branch == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUser_Id = new SelectList(db.ApplicationUsers, "Id", "Email", applicationUser_Branch.ApplicationUser_Id);
            ViewBag.Branch_Id = new SelectList(db.Branch, "Id", "BranchName", applicationUser_Branch.Branch_Id);
            return View(applicationUser_Branch);
        }

        // POST: ApplicationUser_Branch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApplicationUser_Id,Branch_Id")] ApplicationUser_Branch applicationUser_Branch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser_Branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUser_Id = new SelectList(db.ApplicationUsers, "Id", "Email", applicationUser_Branch.ApplicationUser_Id);
            ViewBag.Branch_Id = new SelectList(db.Branch, "Id", "BranchName", applicationUser_Branch.Branch_Id);
            return View(applicationUser_Branch);
        }

        // GET: ApplicationUser_Branch/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser_Branch applicationUser_Branch = db.ApplicationUser_Branch.Find(id);
            if (applicationUser_Branch == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser_Branch);
        }

        // POST: ApplicationUser_Branch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser_Branch applicationUser_Branch = db.ApplicationUser_Branch.Find(id);
            db.ApplicationUser_Branch.Remove(applicationUser_Branch);
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
