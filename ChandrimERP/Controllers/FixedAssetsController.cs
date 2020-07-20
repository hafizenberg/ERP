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

namespace ChamdrimERP.Controllers
{
    public class FixedAssetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FixedAssets
        [Authorize(Roles = "companyAdmin, fix_a_index")]
        public ActionResult Index()
        {
            var fixedAsset = db.FixedAsset.Include(f => f.Branch).Include(f => f.Company).Include(f => f.FixedAssetCategory).Include(f => f.Warehouse);
            return View(fixedAsset.ToList());
        }
        public ActionResult GetList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<FixedAsset>();
                //jsonData = db.FixedAsset.Include(f => f.Branch).Include(f => f.Company).Include(f => f.FixedAssetCategory).Include(f => f.Warehouse).ToList();
                //Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).
                //jsonData = db.FixedAsset.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
                jsonData = db.FixedAsset.Include(f => f.Branch).Include(f => f.Company).Include(f => f.FixedAssetCategory).Include(f => f.Warehouse).ToList();
                var nodes = jsonData.Select(S => new
                {
                    BranchNames = S.Branch.BranchName,
                    CompanyNames = S.Company.CompanyName,
                    WarehouseNames = S.Warehouse.WarehouseName,
                    Description = S.Description,
                    AssetCode = S.AssetCode,
                    AssetValue = S.AssetValue,
                    AssetLife = S.AssetLife,
                    DepreciationRate = S.DepreciationRate,
                    DepreciationEffectFrom = S.DepreciationEffectFrom,
                    SalvageValue = S.SalvageValue,
                    PurchaseDate = S.PurchaseDate,
                    WarrentyDetails = S.WarrentyDetails

                });

                return Json(new { data = nodes }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }


        }
        // GET: FixedAssets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FixedAsset fixedAsset = db.FixedAsset.Find(id);
            if (fixedAsset == null)
            {
                return HttpNotFound();
            }
            return View(fixedAsset);
        }

        // GET: FixedAssets/Create
        [Authorize(Roles = "companyAdmin, fix_a_create")]
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            var branchlist = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.BranchId = new SelectList(branchlist, "Id", "BranchName");
            var fixedassetcategorylist = db.FixedAssetCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.FixedAssetCategoryId = new SelectList(fixedassetcategorylist, "Id", "Name");
            var warehouselist = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.WarehouseId = new SelectList(warehouselist, "Id", "WarehouseName");
            var supplierlist = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.CompanyName + "-" + s.BussinessPhone, value = s.SupplierId })
                   .ToList();
            ViewBag.SupplierId = new SelectList(supplierlist, "value", "Text");

            return View();
        }

        // POST: FixedAssets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,AssetCode,AssetValue,AssetLife,DepreciationRate,DepreciationEffectFrom,AccumulateDepriciation,WrittenDownValue,SalvageValue,Node,SupplierId,PurchaseDate,WarrentyDetails,Status,CreatedOn,BranchId,CompanyId,WarehouseId,FixedAssetCategoryId")] FixedAsset fixedAsset)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                var isExist = IsDataExist(fixedAsset.Name);
                if (isExist)
                {
                    ModelState.AddModelError("NameExist", "This name is already exist.");
                    var companyLists = db.Company
                   .Where(a => a.ApplicationUser_Company
                   .Any(c => c.ApplicationUser_Id == username))
                   .ToList();
                    ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", fixedAsset.CompanyId);
                    var branchlists = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.BranchId = new SelectList(branchlists, "Id", "BranchName",fixedAsset.BranchId);
                    var fixedassetcategorylists = db.FixedAssetCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                   .ToList();
                    ViewBag.FixedAssetCategoryId = new SelectList(fixedassetcategorylists, "Id", "Name",fixedAsset.FixedAssetCategoryId);
                    var warehouselists = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                   .ToList();
                    ViewBag.WarehouseId = new SelectList(warehouselists, "Id", "WarehouseName",fixedAsset.WarehouseId);
                    var supplierlists = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.CompanyName + "-" + s.BussinessPhone, value = s.SupplierId })
                   .ToList();
                    ViewBag.SupplierId = new SelectList(supplierlists, "value", "Text", fixedAsset.SupplierId);
                    return View(new FixedAsset());
                }
                fixedAsset.Id = Guid.NewGuid();
                db.FixedAsset.Add(fixedAsset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var branchlist = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
            ViewBag.BranchId = new SelectList(branchlist, "Id", "BranchName", fixedAsset.BranchId);
            var fixedassetcategorylist = db.FixedAssetCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.FixedAssetCategoryId = new SelectList(fixedassetcategorylist, "Id", "Name", fixedAsset.FixedAssetCategoryId);
            var warehouselist = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.WarehouseId = new SelectList(warehouselist, "Id", "WarehouseName", fixedAsset.WarehouseId);
            var supplierlist = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.CompanyName + "-" + s.BussinessPhone, value = s.SupplierId })
                   .ToList();
            ViewBag.SupplierId = new SelectList(supplierlist, "value", "Text", fixedAsset.SupplierId);
            return View(fixedAsset);
        }
        [NonAction]
        public bool IsDataExist(string data)
        {
            var username = User.Identity.GetUserId();
            var v = db.FixedAsset
                    .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name == data);
            return v != null;
        }
        // GET: FixedAssets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FixedAsset fixedAsset = db.FixedAsset.Find(id);
            if (fixedAsset == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName", fixedAsset.BranchId);
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", fixedAsset.CompanyId);
            ViewBag.FixedAssetCategoryId = new SelectList(db.FixedAssetCategory, "Id", "Name", fixedAsset.FixedAssetCategoryId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", fixedAsset.WarehouseId);
            return View(fixedAsset);
        }

        // POST: FixedAssets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,AssetCode,AssetValue,AssetLife,DepreciationRate,DepreciationEffectFrom,AccumulateDepriciation,WrittenDownValue,SalvageValue,Node,SupplierId,PurchaseDate,WarrentyDetails,Status,CreatedOn,BranchId,CompanyId,WarehouseId,FixedAssetCategoryId")] FixedAsset fixedAsset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fixedAsset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName", fixedAsset.BranchId);
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", fixedAsset.CompanyId);
            ViewBag.FixedAssetCategoryId = new SelectList(db.FixedAssetCategory, "Id", "Name", fixedAsset.FixedAssetCategoryId);
            ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName", fixedAsset.WarehouseId);
            return View(fixedAsset);
        }

        // GET: FixedAssets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FixedAsset fixedAsset = db.FixedAsset.Find(id);
            if (fixedAsset == null)
            {
                return HttpNotFound();
            }
            return View(fixedAsset);
        }

        // POST: FixedAssets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            FixedAsset fixedAsset = db.FixedAsset.Find(id);
            db.FixedAsset.Remove(fixedAsset);
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
