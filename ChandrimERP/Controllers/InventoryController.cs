using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChamdrimERP.Controllers
{
    public class InventoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Inventory
        [Authorize(Roles = "companyAdmin, inv_index")]
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Inventorydatatwo()
        {
            var username = User.Identity.GetUserId();
            var inventory = db.Inventory.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            var data = inventory.OrderBy(x => x.ItemCode).GroupBy(x => x.ItemId).Select(s => new
            {
                ItemCode = s.FirstOrDefault().ItemCode,
                ItemName = s.FirstOrDefault().Product.ProductName,
                SupplierName = s.FirstOrDefault().Product.Supplier.CompanyName,
                BalanceQuantity = s.FirstOrDefault().BalanceQuantity,
                AleartLavel = s.FirstOrDefault().AleartLavel,
                ItemMRP = s.FirstOrDefault().Product.ProductPrice,
                ItemUnitCost = s.FirstOrDefault().ItemUnitCost,
                ItemEditionalCost = s.FirstOrDefault().ItemEditionalCost,
                ItemTotalCost = s.FirstOrDefault().ItemTotalCost,
                ItemBarCode = s.FirstOrDefault().Product.Barcode,
                ItemAverageCost = s.FirstOrDefault().ItemAvrageCost,
                WarehouseName = s.FirstOrDefault().Warehouse.WarehouseName

            }).ToList();

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, inv_Inventory")]
        public ActionResult Inventory()
        {
            return View();
        }

        public ActionResult InventoryDetails(Guid? id)
        {
            TempData["ID"] = id;
            return View();
        }
        public ActionResult InventoryData()
        {
            var username = User.Identity.GetUserId();
            var all = db.InventoryMovement.OrderBy(a => a.CreatedOn).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            var data = all.OrderBy(x => x.ItemCode).GroupBy(x => x.ItemId).Select(s => new
            {
                ItemCode = s.FirstOrDefault().ItemCode,
                ItemBarCode = s.FirstOrDefault().Product.Barcode,
                ItemName = s.FirstOrDefault().ItemName,
                ItemMRP = s.FirstOrDefault().Product.ProductPrice,
                AleartLavel = s.FirstOrDefault().AleartLavel,
                WarehouseName = s.FirstOrDefault().Warehouse.WarehouseName,
                CompanyName = s.FirstOrDefault().Company.CompanyName,
                InInventory = s.Sum(x => x.InQuantity),
                OutInventory = s.Sum(x => x.OutQuantity),
                BalanceInventory = (s.Sum(x => x.InQuantity) - s.Sum(x => x.OutQuantity)),
                ItemID = s.FirstOrDefault().ItemId
            }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InventoryDetailsData()
        {
            var lID = TempData["ID"].ToString();
            var username = User.Identity.GetUserId();
            var all = db.InventoryMovement.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
            var data2 = all.OrderBy(x => x.ItemCode).Where(x=>x.ItemId.ToString() == lID ).Select(x=> new
                        {
                            Date = x.CreatedOn,
                            Name = x.ItemName,
                            Vouchertype = x.InvoiceType,
                            VoucherNo =  x.InvoiceNo,
                            InQuantity = x.InQuantity,
                            OutQuantity = x.OutQuantity,
                            Narration = x.Note,
                            WarehouseName = x.Warehouse.WarehouseName,
            }).ToList();
            return Json(new { data = data2 }, JsonRequestBehavior.AllowGet);
        }
    }
}