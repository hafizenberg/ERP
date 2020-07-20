using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Services
        public ActionResult Index()
        {
            return View(db.PService.ToList());
        }
        public ActionResult GetList()
        {
            try
            {
                List<PService> serviceList = new List<PService>();
                serviceList = db.PService.ToList<PService>();
                return Json(new { data = serviceList }, JsonRequestBehavior.AllowGet);
            }

            catch (EntityException ex)
            {

                return Content(" Connection to Database Failed."+ex);
            }
        }
        // GET: Services/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PService pService = db.PService.Find(id);
            if (pService == null)
            {
                return HttpNotFound();
            }
            return View(pService);
        }

        // GET: Services/Create
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();

            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PServiceCode,Name,Description,updateDate,Note,ServiceCost,EstimatedTime,Status,CreatedOn")] PService pService)
        {
            if (ModelState.IsValid)
            {
                var rowcount = db.PService.Count();

                if (rowcount > 0)
                {
                    var code = db.PService.Max(x => x.PServiceCode);
                    pService.PServiceCode = code + 1;
                }
                else
                {
                    pService.PServiceCode = 1000;
                }
                pService.Id = Guid.NewGuid();
                db.PService.Add(pService);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pService);
        }

        // GET: Services/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PService pService = db.PService.Find(id);
            if (pService == null)
            {
                return HttpNotFound();
            }
            return View(pService);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PServiceCode,Name,Description,updateDate,Note,ServiceCost,EstimatedTime,Status,CreatedOn")] PService pService)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pService);
        }

        // GET: Services/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PService pService = db.PService.Find(id);
            if (pService == null)
            {
                return HttpNotFound();
            }
            return View(pService);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PService pService = db.PService.Find(id);
            db.PService.Remove(pService);
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
