using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "companyAdmin, ord_salesorder")]
        public ActionResult SalesOrder()
        {
            List<OrderReportVM> allOrder = new List<OrderReportVM>();

            // here MyDatabaseEntities is our data context
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var o = db.Order.Where(a => a.Branch.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).OrderByDescending(a => a.Id)
                .Include(a => a.Branch).Include(a => a.Customer).Include(a => a.Ledger)
                .Include(a => a.SalesAgent).Include(a => a.Warehouse).Include(a => a.OrderDetail);

                foreach (var i in o)
                {
                    var orderdetail = db.OrderDetail.Where(a => a.OrderID.Equals(i.Id)).ToList();
                    allOrder.Add(new OrderReportVM { order = i, orderdetail = orderdetail });
                }
            }
            return View(allOrder.AsEnumerable());
        }



        // GET: Order
        //    public ActionResult Index()
        //    {
        //        return View();
        //    }
        //    public ActionResult SalesOrder()
        //    {
        //        ViewBag.UserName = User.Identity.GetUserName();
        //        ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName");
        //        ViewBag.CustomerID = new SelectList(db.Customer, "CustomerId", "CompanyName");
        //        ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name");
        //        ViewBag.SalesAgentId = new SelectList(db.SalesAgent, "Id", "Name");
        //        ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName");
        //        ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");

        //        var product = db.Products.Select(s => new { Text = s.ProductCode + "-" + s.ProductName, value = s.ProductId});
        //        ViewBag.ProductList = new SelectList(product, "value", "Text");
        //        return View();
        //    }

        //    public JsonResult getProduct(Guid  id)
        //    {
        //        //Product product = new Product();
        //       // var ToReturn = db.Supplier.First(x => x.SupplierId == id);

        //        List<Product> product = new List<Product>();
        //        product = db.Products.Where(x=>x.ProductId == id).ToList();

        //        var ToReturn = product.Select(S => new
        //        {
        //            ProductId = S.ProductId,
        //            ProductCode = S.ProductCode,
        //            ProductName = S.ProductName,
        //            Description = S.ProductDescription,
        //            ProductMeasure = S.ProductMeasureUnit.Name,
        //            ProductRate = S.ProductPrice,
        //            ProductVat = S.Vat,

        //            ModelName = S.ModelName
        //        });
        //        return Json(ToReturn, JsonRequestBehavior.AllowGet);
        //    }

        //    public JsonResult getProductList() //It will be fired from Jquery ajax call  
        //    {
        //        ApplicationDbContext cshparpEntity = new ApplicationDbContext();

        //        db.Configuration.ProxyCreationEnabled = false;

        //        List<Product> product = new List<Product>();

        //        var jsonData = cshparpEntity.Products.ToList();

        //        var ToReturn = jsonData.Select(S => new
        //        {
        //            ProductId = S.ProductId,
        //            ProductCode = S.ProductCode,
        //            ProductName = S.ProductName,
        //            Description = S.ProductDescription,
        //            ProductMeasure = S.ProductMeasureUnit.Name,
        //            ProductRate = S.ProductPrice,
        //            ProductVat = S.Vat,
        //            ModelName = S.ModelName
        //        });
        //        return Json(ToReturn, JsonRequestBehavior.AllowGet);
        //    }
        //    public ActionResult addcustomer()
        //    {
        //        ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");
        //        return View();
        //    }
        //    [HttpPost]
        //    public ActionResult addcustomer(Customer customer)
        //    {
        //        db.Customer.Add(customer);
        //        db.SaveChanges();
        //        string message = "SUCCESS";

        //        ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", customer.CompanyId);
        //        return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        //    }
        //   // [WebMethod]
        //    [HttpPost]
        //    public ActionResult SalesOrder(Order orders, OrderDetail[] orderdata, Guid? Product, int? quantity)
        //    {
        //        string result = "Error! Order Is Not Complete!";
        //            orders.UserId = User.Identity.GetUserId();
        //            orders.Id = Guid.NewGuid();

        //            var rowcount = db.Order.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId));

        //            if (rowcount > 0)
        //            {
        //                var invoiceNo = db.Order.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId)).Max(x => x.InvoiceNo);
        //                orders.InvoiceNo = invoiceNo + 1;
        //            }
        //            else
        //            {
        //                orders.InvoiceNo = 1000;
        //            }

        //            db.Order.Add(orders);
        //        var invoiceno = orders.InvoiceNo;
        //        var orderid = orders.Id;
        //        var comid = db.Warehouse.SingleOrDefault(s => s.Id == orders.WarehouseId);
        //        foreach (var item in orderdata)
        //        {
        //            var orderdetalsId = Guid.NewGuid();
        //            OrderDetail O = new OrderDetail();
        //            O.OrderID = orderid;
        //            O.Id = orderdetalsId;
        //            O.ProductName = item.ProductName;
        //            O.Quantity = item.Quantity;
        //            O.Rate = item.Rate;
        //            O.BonusQuantity = item.BonusQuantity;
        //            O.BatchOrSerial = item.BatchOrSerial;
        //            O.ProductId = item.ProductId;
        //            O.NetTotal = item.NetTotal;
        //            O.Discount = item.Discount;
        //            O.TotalAmount = item.TotalAmount;
        //            O.VAT = item.VAT;
        //            O.ProductCode = item.ProductCode;
        //            O.ProductDescription =item. ProductDescription;
        //            O.MeasureUnit = item.MeasureUnit;
        //            db.OrderDetail.Add(O);

        //            //item.ProductId = Product.Value;
        //            //int ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
        //            //int ttquantity = Convert.ToInt32(quantity);
        //            //Inventory inventory = db.Inventory.Find(item.ProductId);
        //            //inventory.BalanceQuantity = ttStock - ttquantity;
        //            //db.Entry(inventory).State = EntityState.Modified;


        //            //Inventory invo = new Inventory();

        //            //Inventory inventory = db.Inventory.Find(item.ProductId);


        //            ////var ttStock = db.Inventory.SingleOrDefault(s => s.ItemId == item.ProductId);
        //            ////var carentStock = ttStock.BalanceQuantity;
        //            //var ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
        //            //var ttquantity = item.Quantity;
        //            //inventory.BalanceQuantity = ttStock - ttquantity;

        //            //db.Entry(inventory).State = EntityState.Modified;


        //            InventoryOutGoing Invout = new InventoryOutGoing();
        //            Invout.Id = Guid.NewGuid();
        //            Invout.ItemId = item.ProductId;
        //            Invout.ItemCode = item.ProductCode;
        //            Invout.InvoiceNo = invoiceno;
        //            Invout.InvoiceType = "Sales Invoice";
        //            Invout.ItemQuantity = item.Quantity;
        //            Invout.ItemUnitCost = item.Rate;
        //            Invout.BatchOrSerial = item.BatchOrSerial;
        //            Invout.Status = true;
        //            Invout.WarehouseId = orders.WarehouseId;
        //            Invout.CompanyId = comid.CompanyId;
        //            db.InventoryOutGoing.Add(Invout);
        //        }
        //            db.SaveChanges();


        //        result = "Success! Order Is Complete!";
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }

        //    public ActionResult ServiceOrder()
        //    {
        //        ViewBag.UserName = User.Identity.GetUserName();
        //        ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName");
        //        ViewBag.CustomerID = new SelectList(db.Customer, "CustomerId", "CompanyName");
        //        ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name");
        //        ViewBag.SalesAgentId = new SelectList(db.SalesAgent, "Id", "Name");
        //        ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");

        //        ViewBag.serviceList = new SelectList(db.PService, "Id", "Name");
        //        return View();
        //    }
        //    public JsonResult getService(Guid id)
        //    {
        //        //Product product = new Product();
        //        // var ToReturn = db.Supplier.First(x => x.SupplierId == id);

        //        List<PService> service = new List<PService>();
        //        service = db.PService.Where(x => x.Id == id).ToList();

        //        var ToReturn = service.Select(S => new
        //        {
        //            serviceId = S.Id,
        //            serviceCode = S.PServiceCode,
        //            serviceName = S.Name,
        //            Description = S.Description,
        //            serviceRate = S.ServiceCost,
        //            serviceVat = S.VAT,
        //        });
        //        return Json(ToReturn, JsonRequestBehavior.AllowGet);
        //    }
        //    public ActionResult tailorOrder()
        //    {
        //        ViewBag.UserName = User.Identity.GetUserName();
        //        ViewBag.BranchId = new SelectList(db.Branch, "Id", "BranchName");
        //        var customer = db.Customer.Select(s => new { Text = s.ContactFirstName + " " + s.ContactLastName + "-"+s.Phone, value = s.CustomerId });
        //        ViewBag.CustomerID = new SelectList(customer, "value", "Text");


        //        ViewBag.LedgerId = new SelectList(db.Ledger, "Id", "Name");
        //        ViewBag.SalesAgentId = new SelectList(db.SalesAgent, "Id", "Name");
        //        ViewBag.WarehouseId = new SelectList(db.Warehouse, "Id", "WarehouseName");
        //        ViewBag.TailorList = new SelectList(db.Tailor, "Id", "Name");
        //        ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");
        //        ViewBag.dressModel = new SelectList(db.DressModel, "Id", "Name");

        //        var product = db.Products.Select(s => new { Text = s.ProductCode + "-" + s.ProductName, value = s.ProductId });
        //        ViewBag.ProductList = new SelectList(product, "value", "Text");

        //        return View();
        //    }
        //    protected override void Dispose(bool disposing)
        //    {
        //        if (disposing)
        //        {
        //            db.Dispose();
        //        }
        //        base.Dispose(disposing);
        //    }
    }
}
        
           