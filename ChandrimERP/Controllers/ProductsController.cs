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
using System.Drawing;
using System.Drawing.Imaging;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        [Authorize(Roles = "companyAdmin, pro_index")]
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var products = db.Products.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            return View(products.ToList());
        }
        public ActionResult GetProductList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Product>();
                jsonData = db.Products.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x=>x.Status==true).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }

        }
        // GET: Products/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "companyAdmin, pro_create")]
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            var pbrandList = db.ProductBrand.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.ProductBrandId = new SelectList(pbrandList, "Id", "Name");
            var warehouseList = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.warehouseId = new SelectList(warehouseList, "Id", "WarehouseName");
            var pcatList = db.ProductCategory.Where(a=>a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.ProductCategoryId = new SelectList(pcatList, "Id", "Name");
            var pmesureList = db.ProductMeasureUnit.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.ProductMeasureUnitId = new SelectList(pmesureList, "Id", "Name");
            var prackList = db.ProductRack.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.ProductRackId = new SelectList(prackList, "Id", "Name");
            var supplierList = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.SupplierId = new SelectList(supplierList, "SupplierId", "CompanyName");
            return View(new Product());
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase ImageUpload)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                if (product.ProductPrice == 0 && product.ProductUnitCost == 0)
                {
                    if (product.ProductPrice == 0)
                    {
                        ModelState.AddModelError("ProductPrice", "Product Price 0 is not allowed.");
                    }
                    if (product.ProductUnitCost == 0)
                    {
                        ModelState.AddModelError("ProductUnitCost", "Unit cost 0 is not allowed.");
                    }
                    var companyLists = db.Company
                       .Where(a => a.ApplicationUser_Company
                       .Any(c => c.ApplicationUser_Id == username))
                       .ToList();
                    ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", product.CompanyId);
                    var pbrandListt = db.ProductBrand.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductBrandId = new SelectList(pbrandListt, "Id", "Name", product.ProductBrandId);
                    var warehouseListt = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.warehouseId = new SelectList(warehouseListt, "Id", "WarehouseName", product.WarehouseId);
                    var pcatListt = db.ProductCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductCategoryId = new SelectList(pcatListt, "Id", "Name", product.ProductCategoryId);
                    var pmesureListt = db.ProductMeasureUnit.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductMeasureUnitId = new SelectList(pmesureListt, "Id", "Name", product.ProductMeasureUnitId);
                    var prackListt = db.ProductRack.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductRackId = new SelectList(prackListt, "Id", "Name", product.ProductRackId);
                    var supplierListt = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.SupplierId = new SelectList(supplierListt, "SupplierId", "CompanyName", product.SupplierId);
                    return View(product);
                }
                //Use it in your post method
                var isExist = IsDataExist(product.Barcode);
                if (isExist)
                {

                    ModelState.AddModelError("BarcodeExist", "Barcode is already exist.");
                    var companyLists = db.Company
                       .Where(a => a.ApplicationUser_Company
                       .Any(c => c.ApplicationUser_Id == username))
                       .ToList();
                    ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", product.CompanyId);
                    var pbrandListt = db.ProductBrand.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductBrandId = new SelectList(pbrandListt, "Id", "Name", product.ProductBrandId);
                    var warehouseListt = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.warehouseId = new SelectList(warehouseListt, "Id", "WarehouseName",product.WarehouseId);
                    var pcatListt = db.ProductCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductCategoryId = new SelectList(pcatListt, "Id", "Name",product.ProductCategoryId);
                    var pmesureListt = db.ProductMeasureUnit.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductMeasureUnitId = new SelectList(pmesureListt, "Id", "Name",product.ProductMeasureUnitId);
                    var prackListt = db.ProductRack.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.ProductRackId = new SelectList(prackListt, "Id", "Name",product.ProductRackId);
                    var supplierListt = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.SupplierId = new SelectList(supplierListt, "SupplierId", "CompanyName",product.SupplierId);
                    return View(product);
                }
                

                if (ImageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => s.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");
                    string directoryPath = "~/UploadedFiles/" + companyname + "/Product_image/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                        Directory.CreateDirectory(Server.MapPath(directoryPath));

                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    product.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    ImageUpload.SaveAs(fileName);
                }
                var rowcount = db.Products.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
                if (rowcount > 0)
                {
                    var productcode = db.Products.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.ProductCode);
                    product.ProductCode = productcode + 1;
                }
                else
                {
                    product.ProductCode = 1000;
                }
                product.ProductId = Guid.NewGuid();
                db.Products.Add(product);
                db.SaveChanges();
                var pId = product.ProductId;

                if (product.ProductQuantity != null)
                {
                    //Add data to product transaction table
                    InventoryIncomming IIC = new InventoryIncomming();
                    IIC.Id= Guid.NewGuid();
                    IIC.ItemId = pId;
                    IIC.ItemCode = product.ProductCode;
                    IIC.InvoiceType = "Opening Quantity";
                    IIC.ItemUnitCost = product.ProductUnitCost;
                    IIC.BatchOrSerial = product.BatchOrSerial;
                    IIC.WarehouseId = product.WarehouseId;
                    IIC.ItemQuantity = product.ProductQuantity;
                    IIC.CompanyId = product.CompanyId;
                    db.InventoryIncomming.Add(IIC);

                    InventoryMovement IM = new InventoryMovement();
                    IM.Id = Guid.NewGuid();
                    IM.InvoiceNo = 1000;
                    IM.ItemId = product.ProductId;
                    IM.ItemCode = product.ProductCode;
                    IM.ItemName = product.ProductName;
                    IM.AleartLavel = product.ProductVolume;
                    IM.InvoiceType = "Opening Quantity";
                    IM.InQuantity = product.ProductQuantity;
                    IM.OutQuantity = 0;
                    IM.BatchOrSerial = product.BatchOrSerial;
                    IM.UserName = User.Identity.GetUserId(); 
                    IM.ProId = product.ProductId;
                    IM.CompanyId = product.CompanyId;
                    IM.WarehouseId = product.WarehouseId;
                    db.InventoryMovement.Add(IM);
                }
                //Add data to product Inventory table
                int pqtn = Convert.ToInt32(product.ProductQuantity);
                Inventory Inv = new Inventory();
                Inv.Id = Guid.NewGuid(); 
                Inv.ItemId = pId;
                Inv.ItemCode = product.ProductCode;
                Inv.BalanceQuantity = pqtn;
                Inv.WarehouseId = product.WarehouseId;
                if (Inv.ItemEditionalCost == null)
                {
                    Inv.ItemEditionalCost = 0;
                }
                Inv.ItemUnitCost = product.ProductUnitCost;
                Inv.ItemTotalCost = Inv.ItemUnitCost + Inv.ItemEditionalCost;
                Inv.ItemAvrageCost = (((Inv.ItemTotalCost * Inv.BalanceQuantity)+
                    (product.ProductQuantity * product.ProductUnitCost)) / (Inv.BalanceQuantity+product.ProductQuantity));
                Inv.AleartLavel = product.ProductVolume;
                Inv.ItemMRP = product.ProductPrice;
                Inv.ProId =  pId ;
                Inv.CompanyId = product.CompanyId;
                db.Inventory.Add(Inv);
                
                db.SaveChanges();
                ModelState.Clear();

                var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
                ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", product.CompanyId);
                var pbrandList = db.ProductBrand.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
               .ToList();
                var warehouseList = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                .ToList();
                ViewBag.warehouseId = new SelectList(warehouseList, "Id", "WarehouseName", product.WarehouseId);
                ViewBag.ProductBrandId = new SelectList(pbrandList, "Id", "Name", product.ProductBrandId);
                var pcatList = db.ProductCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                .ToList();
                ViewBag.ProductCategoryId = new SelectList(pcatList, "Id", "Name", product.ProductCategoryId);
                var pmesureList = db.ProductMeasureUnit.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                .ToList();
                ViewBag.ProductMeasureUnitId = new SelectList(pmesureList, "Id", "Name", product.ProductMeasureUnitId);
                var prackList = db.ProductRack.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                .ToList();
                ViewBag.ProductRackId = new SelectList(prackList, "Id", "Name", product.ProductRackId);
                var supplierList = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                .ToList();
                ViewBag.SupplierId = new SelectList(supplierList, "SupplierId", "CompanyName", product.SupplierId);
                ViewBag.ConMess = "Data Saved Successfully";
                
                return View(product);

            }

            var companyList3 = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList3, "Id", "CompanyName", product.CompanyId);
            var pbrandList3 = db.ProductBrand.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            var warehouseList3 = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.warehouseId = new SelectList(warehouseList3, "Id", "WarehouseName", product.WarehouseId);
            ViewBag.ProductBrandId = new SelectList(pbrandList3, "Id", "Name", product.ProductBrandId);
            var pcatList3 = db.ProductCategory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.ProductCategoryId = new SelectList(pcatList3, "Id", "Name", product.ProductCategoryId);
            var pmesureList3 = db.ProductMeasureUnit.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.ProductMeasureUnitId = new SelectList(pmesureList3, "Id", "Name", product.ProductMeasureUnitId);
            var prackList3 = db.ProductRack.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.ProductRackId = new SelectList(prackList3, "Id", "Name", product.ProductRackId);
            var supplierList3 = db.Supplier.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.SupplierId = new SelectList(supplierList3, "SupplierId", "CompanyName", product.SupplierId);
            
            return View(product);
        }
        [NonAction]
        public bool IsDataExist(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.Products.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                        .ToList().Where(a => a.Barcode == data).FirstOrDefault();
                return v != null;
            }
        }
        // GET: Products/Edit/5
        public ActionResult UpdateProducts(Guid? id)
        {
            var username = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var warehouseList = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
.ToList();
            ViewBag.warehouseId = new SelectList(warehouseList, "Id", "WarehouseName", product.WarehouseId);
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", product.CompanyId);
            ViewBag.ProductBrandId = new SelectList(db.ProductBrand, "Id", "Name", product.ProductBrandId);
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategory, "Id", "Name", product.ProductCategoryId);
            ViewBag.ProductSubCategoryId = new SelectList(db.ProductSubCategory, "Id", "Name", product.ProductSubCategoryId);
            ViewBag.ProductMeasureUnitId = new SelectList(db.ProductMeasureUnit, "Id", "Name", product.ProductMeasureUnitId);
            ViewBag.ProductRackId = new SelectList(db.ProductRack, "Id", "Name", product.ProductRackId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProducts(Product product)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                Product s = new Product();
                s = db.Products.FirstOrDefault(f => f.ProductId == product.ProductId);

                var oldfilepath = db.Company.Where(x => x.Id == product.ProductId).Select(sd => sd.ImageUrl).FirstOrDefault();

                if (product.ImageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(d => d.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");

                    string directoryPath = "~/UploadedFiles/" + companyname + "/Product_image/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                    {
                        Directory.CreateDirectory(Server.MapPath(directoryPath));
                    }
                    string fileName = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                    string extension = Path.GetExtension(product.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    product.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    product.ImageUpload.SaveAs(fileName);
                    string fullpath = Request.MapPath(oldfilepath);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                }
                s.ImageUrl = product.ImageUrl;
                s.ProductDescription = product.ProductDescription;
                s.ProductExpireDate = product.ProductExpireDate;
                s.ProductManufactureDate = product.ProductManufactureDate;
                s.ProductPrice = product.ProductPrice;
                s.ProductUnitCost = product.ProductUnitCost;
                s.Vat = product.Vat;
                s.Remarks = product.Remarks;
                s.Status = product.Status;
                s.ProductVolume = product.ProductVolume;
                s.ProductType = product.ProductType;

                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", product.CompanyId);
            ViewBag.ProductBrandId = new SelectList(db.ProductBrand, "Id", "Name", product.ProductBrandId);
            ViewBag.ProductCategoryId = new SelectList(db.ProductCategory, "Id", "Name", product.ProductCategoryId);
            ViewBag.ProductMeasureUnitId = new SelectList(db.ProductMeasureUnit, "Id", "Name", product.ProductMeasureUnitId);
            ViewBag.ProductRackId = new SelectList(db.ProductRack, "Id", "Name", product.ProductRackId);
            ViewBag.SupplierId = new SelectList(db.Supplier, "SupplierId", "CompanyName", product.SupplierId);
            var warehouseList = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.warehouseId = new SelectList(warehouseList, "Id", "WarehouseName", product.WarehouseId);

            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetProductSubCategory(int PCId)
        {
            var PC = db.ProductSubCategory.Where(x => x.ProductCategoryId == PCId).ToList();
            ViewBag.ProductSubCategoryId = new SelectList(PC, "Id", "Name");
            return PartialView("ProductSubCategory");

        }
        [HttpPost]
        public ActionResult addCategory(ProductCategory category)
        {
            string message = "";
            //Use it in your post method
            var isExist = IsDataExisttwo(category.Name);
            if (isExist)
            {
                //ModelState.AddModelError("CategoryNameExist", "Category Name is already exist.");
                message = "Category Name is already exist.";
            }
            else
            {
                db.ProductCategory.Add(category);
                db.SaveChanges();
                message = "Categroy added successfully";
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }
        [NonAction]
        public bool IsDataExisttwo(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.ProductCategory
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name.ToUpper() == data.ToUpper());
                return v != null;
            }
        }
        [HttpPost]
        public ActionResult addSubCategory(ProductSubCategory subcategory)
        {
            string message = "";
            //Use it in your post method
            var isExist = IsDataExistthree(subcategory.Name);
            if (isExist)
            {
                //ModelState.AddModelError("CategoryNameExist", "Category Name is already exist.");
                message = "Sub Category Name is already exist.";
            }
            else
            {
                db.ProductSubCategory.Add(subcategory);
                db.SaveChanges();
                message = "Sub Categroy added successfully";
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }
        [NonAction]
        public bool IsDataExistthree(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.ProductSubCategory
                        .Where(a => a.ProductCategory.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name.ToUpper() == data.ToUpper());
                return v != null;
            }
        }
        [HttpPost]
        public ActionResult addMeasureUnit(ProductMeasureUnit unit)
        {
            string message = "";
            //Use it in your post method
            var isExist = IsDataExistfour(unit.Name);
            if (isExist)
            {
                //ModelState.AddModelError("CategoryNameExist", "Category Name is already exist.");
                message = "Measurement unit is already exist.";
            }
            else
            {
                db.ProductMeasureUnit.Add(unit);
                db.SaveChanges();
                message = "Measureunit added successfully";
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });

        }
        [NonAction]
        public bool IsDataExistfour(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.ProductMeasureUnit
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name.ToUpper() == data.ToUpper());
                return v != null;
            }
        }
        [HttpPost]
        public ActionResult addBrand(ProductBrand brand)
        {
            string message = "";
            //Use it in your post method
            var isExist = IsDataExistfive(brand.Name);
            if (isExist)
            {
                //ModelState.AddModelError("CategoryNameExist", "Category Name is already exist.");
                message = "Brand Name is already exist.";
            }
            else
            {
                db.ProductBrand.Add(brand);
                db.SaveChanges();
                message = "Brand added successfully";
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }
        [NonAction]
        public bool IsDataExistfive(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.ProductBrand
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name.ToUpper() == data.ToUpper());
                return v != null;
            }
        }
        [HttpPost]
        public ActionResult addRack(ProductRack rack)
        {
            string message = "";
            //Use it in your post method
            var isExist = IsDataExistSix(rack.Name);
            if (isExist)
            {
                //ModelState.AddModelError("CategoryNameExist", "Category Name is already exist.");
                message = "Rack Name is already exist.";
            }
            else
            {
                db.ProductRack.Add(rack);
                db.SaveChanges();
                message = "Rack added successfully";
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });

        }
        [NonAction]
        public bool IsDataExistSix(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.ProductRack
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name.ToUpper() == data.ToUpper());
                return v != null;
            }
        }

        public ActionResult BarcodeIndex()
        {
            var username = User.Identity.GetUserId();
            var list =
                db.Products.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
            return View(list);
        }
        public ActionResult BarcodeProccess(Guid? id)
        {
            ViewBag.Data = id;
            ViewBag.ProductName = db.Products.Where(x => x.ProductId == id).Select(x => x.ProductName).FirstOrDefault();
            ViewBag.ProductModel = db.Products.Where(x => x.ProductId == id).Select(x => x.ModelName).FirstOrDefault();
            ViewBag.ProductPrice = db.Products.Where(x => x.ProductId == id).Select(x => x.ProductPrice).FirstOrDefault();
            return View();
        }
        public ActionResult BarcodeGen(Guid? id,int? qnt)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Quantity = qnt;
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);

        }

        public ActionResult UpdateProductDrop(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategory.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProductDrop(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productCategory);
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
