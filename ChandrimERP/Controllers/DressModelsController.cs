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
    public class DressModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DressModels
        [Authorize(Roles = "companyAdmin, dre_m_index")]
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var dressModel = db.DressModel.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            return View(dressModel);
        }
        public ActionResult GetDressModelList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = db.DressModel.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new {
                    D_id = s.Id,
                    D_Name = s.Name,
                    D_Note = s.Note,
                    D_Image = s.ImageUrl
                }).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }
        // GET: DressModels/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DressModel dressModel = db.DressModel.Find(id);
            if (dressModel == null)
            {
                return HttpNotFound();
            }
            return View(dressModel);
        }
        [Authorize(Roles = "companyAdmin, dre_m_create")]
        // GET: DressModels/Create
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var companyList = db.Company
               .Where(a => a.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            return View(new DressModel());
        }

        // POST: DressModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DressModel dressModel, HttpPostedFileBase ImageUpload)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var isExist = IsDataExist(dressModel.Name);
                if (isExist)
                {
                    ModelState.AddModelError("DressModelExist", "Dress Model is already exist.");
                    var companyList = db.Company
                   .Where(a => a.ApplicationUser_Company
                   .Any(c => c.ApplicationUser_Id == username))
                   .ToList();
                    ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName",dressModel.CompanyId);
                    return View(new DressModel());
                }
                dressModel.Id = Guid.NewGuid();
                
                if (ImageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => s.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");
                    string directoryPath = "~/UploadedFiles/" + companyname + "/Dressmodel/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                        Directory.CreateDirectory(Server.MapPath(directoryPath));

                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    dressModel.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    ImageUpload.SaveAs(fileName);
                }
                db.DressModel.Add(dressModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", dressModel.CompanyId);
            return View(dressModel);
        }
        [NonAction]
        public bool IsDataExist(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.DressModel
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name.ToUpper() == data.ToUpper());
                return v != null;
            }
        }
        // GET: DressModels/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DressModel dressModel = db.DressModel.Find(id);
            if (dressModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", dressModel.CompanyId);
            return View(dressModel);
        }

        // POST: DressModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyId,Name,ImageUrl,Note")] DressModel dressModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dressModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", dressModel.CompanyId);
            return View(dressModel);
        }

        // GET: DressModels/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DressModel dressModel = db.DressModel.Find(id);
            if (dressModel == null)
            {
                return HttpNotFound();
            }
            return View(dressModel);
        }

        // POST: DressModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DressModel dressModel = db.DressModel.Find(id);
            db.DressModel.Remove(dressModel);
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
