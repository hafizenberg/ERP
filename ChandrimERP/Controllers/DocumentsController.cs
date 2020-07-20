using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;

namespace ChamdrimERP.Controllers
{
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Documents
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var document = db.Document.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            return View(document.ToList());
        }
       
        public ActionResult GetDocumentList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Document>();
                jsonData = db.Document.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
                var subCategoryToReturn = jsonData.Select(S => new {
                    D_image = S.DocumentPath,
                    D_name = S.DocumentName,
                    D_code = S.DocumentCode,
                    D_details = S.DocumentDetails,
                    D_acc_count = S.DocumentAccessCount,
                    D_Exp = S.EXPDate,
                    D_id = S.Id
                });
                return Json(new { data = subCategoryToReturn }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }
        // GET: Documents/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");
            return View(new Document());
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DocumentCode,DocumentName,DocumentAccessCount,DocumentDetails,DocumentPath,EXPDate,UpdateDate,CreatedOn,CompanyId")] Document document)
        {
            if (ModelState.IsValid)
            {
                if (document.FileUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(document.FileUpload.FileName);
                    string extension = Path.GetExtension(document.FileUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    document.DocumentPath = "~/Image/Document/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/Document/"), fileName);
                    document.FileUpload.SaveAs(fileName);
                }
                document.Id = Guid.NewGuid();
                db.Document.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", document.CompanyId);
            return View(document);
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", document.CompanyId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DocumentCode,DocumentName,DocumentAccessCount,DocumentDetails,DocumentPath,EXPDate,UpdateDate,CreatedOn,CompanyId")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", document.CompanyId);
            return View(document);
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Document document = db.Document.Find(id);
            db.Document.Remove(document);
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
