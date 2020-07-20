using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using System.IO;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class ServiceImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceImages
        public ActionResult Index()
        {
            var pserviceImage = db.PserviceImage.Include(p => p.PService);
            return View(pserviceImage.ToList());
        }

        // GET: ServiceImages/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PserviceImage pserviceImage = db.PserviceImage.Find(id);
            if (pserviceImage == null)
            {
                return HttpNotFound();
            }
            return View(pserviceImage);
        }

        // GET: ServiceImages/Create
        public ActionResult Create()
        {
            ViewBag.PServiceId = new SelectList(db.PService, "Id", "Name");
            return View(new PserviceImage());
        }

        // POST: ServiceImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PServiceId,ImageId")] PserviceImage pserviceImage)
        {
            if (ModelState.IsValid)
            {
                if (pserviceImage.imageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(pserviceImage.imageUpload.FileName);
                    string extension = Path.GetExtension(pserviceImage.imageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    pserviceImage.ImageId = "~/Image/Product/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/Product/"), fileName);
                    pserviceImage.imageUpload.SaveAs(fileName);
                }

                pserviceImage.Id = Guid.NewGuid();
                db.PserviceImage.Add(pserviceImage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PServiceId = new SelectList(db.PService, "Id", "Name", pserviceImage.PServiceId);
            return View(pserviceImage);
        }

        // GET: ServiceImages/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PserviceImage pserviceImage = db.PserviceImage.Find(id);
            if (pserviceImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.PServiceId = new SelectList(db.PService, "Id", "Name", pserviceImage.PServiceId);
            return View(pserviceImage);
        }

        // POST: ServiceImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PServiceId,ImageId")] PserviceImage pserviceImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pserviceImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PServiceId = new SelectList(db.PService, "Id", "Name", pserviceImage.PServiceId);
            return View(pserviceImage);
        }

        // GET: ServiceImages/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PserviceImage pserviceImage = db.PserviceImage.Find(id);
            if (pserviceImage == null)
            {
                return HttpNotFound();
            }
            return View(pserviceImage);
        }

        // POST: ServiceImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PserviceImage pserviceImage = db.PserviceImage.Find(id);
            db.PserviceImage.Remove(pserviceImage);
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
