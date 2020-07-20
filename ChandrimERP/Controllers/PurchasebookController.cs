using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ChamdrimERP.Controllers
{
    public class PurchasebookController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Purchasebook
        [Authorize(Roles = "companyAdmin, pur_b_purchaseinvoice")]
        [Route("Purchase")]
        public ActionResult PurchaseInvoice()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var supplier = db.Supplier
           .Where(a => a.Company.ApplicationUser_Company
           .Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.SupplierID = new SelectList(supplier, "SupplierId", "CompanyName");
            var ledger = db.Ledger
                 .Where(a => a.Company.ApplicationUser_Company
                 .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Finished Goods")
                 .ToList();
            ViewBag.LedgerId = new SelectList(ledger, "Id", "Name");
            var salesagent = db.SalesAgent
                            .Where(a => a.Company.ApplicationUser_Company
                            .Any(c => c.ApplicationUser_Id == username))
                            .ToList();
            ViewBag.SalesAgentId = new SelectList(salesagent, "Id", "Name");
            var warehouse = db.Warehouse
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.WarehouseId = new SelectList(warehouse, "Id", "WarehouseName");
            var company = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");

            var product = db.Products.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.Barcode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseInvoice(PurchaseOrder  orders, PurchaseOrderDetails[] orderdata)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Purchase Invoice";
            var rowcount = db.PurchaseOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Purchase Invoice");

            if (rowcount > 0)
            {
                var invoiceNo = db.PurchaseOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Purchase Invoice").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.PurchaseOrder.Add(orders);

            var user = orders.UserId;
            var note = orders.Narration;

            var orderamount = orders.InvoicedAmount;
            var orderbranch = orders.BranchId;
            var orderdate = orders.OrderDate;
            var ordernarration = orders.Narration;

            var invoiceno = orders.InvoiceNo;
            var orderid = orders.Id;
            var comid = db.Warehouse.SingleOrDefault(s => s.Id == orders.WarehouseId);
            foreach (var item in orderdata)
            {
                var orderdetalsId = Guid.NewGuid();
                PurchaseOrderDetails O = new PurchaseOrderDetails();
                O.OrderID = orderid;
                O.Id = orderdetalsId;
                O.ProductName = item.ProductName;
                O.Quantity = item.Quantity;
                O.Rate = item.Rate;
                O.BonusQuantity = item.BonusQuantity;
                O.BatchOrSerial = item.BatchOrSerial;
                O.ProductId = item.ProductId;
                O.NetTotal = item.NetTotal;
                O.Discount = item.Discount;
                O.TotalAmount = item.TotalAmount;
                O.VAT = item.VAT;
                O.ProductCode = item.ProductCode;
                O.ProductDescription = item.ProductDescription;
                O.MeasureUnit = item.MeasureUnit;
                db.PurchaseOrderDetails.Add(O);

                var ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
                int ttquantity = item.Quantity;
                var invId = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.Id).SingleOrDefault();
                Inventory inventory = db.Inventory.Find(invId);
                inventory.BalanceQuantity = ttStock + ttquantity;
                //  int ttFStock = ttStock;
                inventory.ItemAvrageCost = (((inventory.ItemTotalCost * inventory.BalanceQuantity) +
                    ((item.Quantity) * item.Rate)) / (inventory.BalanceQuantity + item.Quantity));

                db.Entry(inventory).State = EntityState.Modified;
               

                InventoryIncomming Invin = new InventoryIncomming();
                Invin.Id = Guid.NewGuid();
                Invin.ItemId = item.ProductId;
                Invin.ItemCode = item.ProductCode;
                Invin.InvoiceNo = invoiceno;
                Invin.InvoiceType = "Purchase Invoice";
                Invin.ItemQuantity = item.Quantity;
                Invin.ItemUnitCost = item.Rate;
                Invin.BatchOrSerial = item.BatchOrSerial;
                Invin.Status = true;
                Invin.WarehouseId = orders.WarehouseId;
                Invin.CompanyId = comid.CompanyId;
                db.InventoryIncomming.Add(Invin);

                var alaretLavel = db.Products.Where(x => x.ProductId == item.ProductId).Select(s => s.ProductVolume).SingleOrDefault();

                InventoryMovement IM = new InventoryMovement();
                IM.Id = Guid.NewGuid();
                IM.InvoiceNo = Convert.ToInt32(invoiceno);
                IM.ItemId = item.ProductId;
                IM.ItemCode = item.ProductCode;
                IM.ItemName = item.ProductName;
                IM.AleartLavel = alaretLavel;
                IM.InvoiceType = "Purchase Invoice";
                IM.InQuantity = item.Quantity;
                IM.OutQuantity = 0;
                IM.BatchOrSerial = item.BatchOrSerial;
                IM.Note = note;
                IM.UserName = user;
                IM.ProId = item.ProductId;
                IM.CompanyId = comid.CompanyId;
                IM.WarehouseId = orders.WarehouseId;
                db.InventoryMovement.Add(IM);
            }

            Transaction transaction = new Transaction();
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Purchase Invoice");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Purchase Invoice").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Purchase Invoice";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);

            
            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var supplierid = orders.SupplierID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var supplierledger = db.Ledger.Where(x => x.RefNo == supplierid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var supplierledgerCode = db.Ledger.Where(x => x.RefNo == supplierid).Select(s => s.LedgerCode).SingleOrDefault();
            var supplierledgerId = db.Ledger.Where(x => x.RefNo == supplierid).Select(s => s.Id).SingleOrDefault();

            var transactionid = transaction.Id;
            var vouchernos = transaction.VoucherNo;


            var username = User.Identity.GetUserId();
            var companyid = db.Company.Where(x => x.ApplicationUser_Company.Any(a => a.ApplicationUser_Id == username)).Select(x => x.Id).SingleOrDefault();

            var discountAmount = orders.TotalDiscount;
            var discountLedgerName = "Discount";
            var discountLedgerId = db.Ledger.Where(x => x.Name == discountLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var discountLedgerCode = db.Ledger.Where(x => x.Id == discountLedgerId).Select(s => s.LedgerCode).SingleOrDefault();


            var texesAmount = orders.VatAmount;
            var texesLedgerName = "Texes Payable";
            var texesLedgerId = db.Ledger.Where(x => x.Name == texesLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var texesLedgerCode = db.Ledger.Where(x => x.Id == texesLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var addAmount = orders.Addamount;
            var addLedgerName = "Cost Of Goods Purchase";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Trade Discount";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },


                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },


                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};
            }

            foreach (var ts in transactiondata)
            {
                ts.Id = Guid.NewGuid();
                ts.VoucherNo = vouchernos;
                ts.VoucherType = vouchertypes;
                ts.Narration = ordernarration;
                ts.TransactionID = transactionid;
                ts.TransactionDate = orderdate;
                ts.BranchID = orderbranch;
                db.TransactionDetails.Add(ts);
            }

            db.SaveChanges();


            result = "Success! Order Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getProduct(Guid id)
        {
            //Product product = new Product();
            // var ToReturn = db.Supplier.First(x => x.SupplierId == id);

            List<Product> product = new List<Product>();
            product = db.Products.Where(x => x.ProductId == id).ToList();

            var ToReturn = product.Select(S => new
            {
                ProductId = S.ProductId,
                ProductCode = S.ProductCode,
                ProductName = S.ProductName,
                Description = S.ProductDescription,
                ProductMeasure = S.ProductMeasureUnit.Name,
                ProductRate = S.ProductPrice,
                ProductVat = S.Vat,

                ModelName = S.ModelName
            });
            return Json(ToReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProductList() //It will be fired from Jquery ajax call  
        {
            ApplicationDbContext cshparpEntity = new ApplicationDbContext();

            db.Configuration.ProxyCreationEnabled = false;

            List<Product> product = new List<Product>();

            var jsonData = cshparpEntity.Products.ToList();

            var ToReturn = jsonData.Select(S => new
            {
                ProductId = S.ProductId,
                ProductCode = S.ProductCode,
                ProductName = S.ProductName,
                Description = S.ProductDescription,
                ProductMeasure = S.ProductMeasureUnit.Name,
                ProductRate = S.ProductPrice,
                ProductVat = S.Vat,
                ModelName = S.ModelName
            });
            return Json(ToReturn, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, pur_b_purchasereturn")]
        public ActionResult PurchaseReturn()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var supplier = db.Supplier
           .Where(a => a.Company.ApplicationUser_Company
           .Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.SupplierID = new SelectList(supplier, "SupplierId", "CompanyName");
            var ledger = db.Ledger
                 .Where(a => a.Company.ApplicationUser_Company
                 .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Finished Goods")
                 .ToList();
            ViewBag.LedgerId = new SelectList(ledger, "Id", "Name");
            var salesagent = db.SalesAgent
                            .Where(a => a.Company.ApplicationUser_Company
                            .Any(c => c.ApplicationUser_Id == username))
                            .ToList();
            ViewBag.SalesAgentId = new SelectList(salesagent, "Id", "Name");
            var warehouse = db.Warehouse
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.WarehouseId = new SelectList(warehouse, "Id", "WarehouseName");
            var company = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");

            var product = db.Products.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.Barcode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseReturn(PurchaseOrder orders, PurchaseOrderDetails[] orderdata)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Purchase Return";
            var rowcount = db.PurchaseOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Purchase Return");

            if (rowcount > 0)
            {
                var invoiceNo = db.PurchaseOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Purchase Return").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.PurchaseOrder.Add(orders);

            var user = orders.UserId;
            var note = orders.Narration;

            var orderamount = orders.InvoicedAmount;
            var orderbranch = orders.BranchId;
            var orderdate = orders.OrderDate;
            var ordernarration = orders.Narration;

            var invoiceno = orders.InvoiceNo;
            var orderid = orders.Id;
            var comid = db.Warehouse.SingleOrDefault(s => s.Id == orders.WarehouseId);
            foreach (var item in orderdata)
            {
                var orderdetalsId = Guid.NewGuid();
                PurchaseOrderDetails O = new PurchaseOrderDetails();
                O.OrderID = orderid;
                O.Id = orderdetalsId;
                O.ProductName = item.ProductName;
                O.Quantity = item.Quantity;
                O.Rate = item.Rate;
                O.BonusQuantity = item.BonusQuantity;
                O.BatchOrSerial = item.BatchOrSerial;
                O.ProductId = item.ProductId;
                O.NetTotal = item.NetTotal;
                O.Discount = item.Discount;
                O.TotalAmount = item.TotalAmount;
                O.VAT = item.VAT;
                O.ProductCode = item.ProductCode;
                O.ProductDescription = item.ProductDescription;
                O.MeasureUnit = item.MeasureUnit;
                db.PurchaseOrderDetails.Add(O);

                var ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
                int ttquantity = item.Quantity;
                var invId = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.Id).SingleOrDefault();
                Inventory inventory = db.Inventory.Find(invId);
                inventory.BalanceQuantity = ttStock - ttquantity;
                //  int ttFStock = ttStock;
                inventory.ItemAvrageCost = (((inventory.ItemTotalCost * inventory.BalanceQuantity) +
                    ((item.Quantity) * item.Rate)) / (inventory.BalanceQuantity + item.Quantity));

                db.Entry(inventory).State = EntityState.Modified;


                InventoryOutGoing Invin = new InventoryOutGoing();
                Invin.Id = Guid.NewGuid();
                Invin.ItemId = item.ProductId;
                Invin.ItemCode = item.ProductCode;
                Invin.InvoiceNo = invoiceno;
                Invin.InvoiceType = "Purchase Return";
                Invin.ItemQuantity = item.Quantity;
                Invin.ItemUnitCost = item.Rate;
                Invin.BatchOrSerial = item.BatchOrSerial;
                Invin.Status = true;
                Invin.WarehouseId = orders.WarehouseId;
                Invin.CompanyId = comid.CompanyId;
                db.InventoryOutGoing.Add(Invin);

                var alaretLavel = db.Products.Where(x => x.ProductId == item.ProductId).Select(s => s.ProductVolume).SingleOrDefault();

                InventoryMovement IM = new InventoryMovement();
                IM.Id = Guid.NewGuid();
                IM.InvoiceNo = Convert.ToInt32(invoiceno);
                IM.ItemId = item.ProductId;
                IM.ItemCode = item.ProductCode;
                IM.ItemName = item.ProductName;
                IM.AleartLavel = alaretLavel;
                IM.InvoiceType = "Purchase Return";
                IM.InQuantity = item.Quantity;
                IM.OutQuantity = 0;
                IM.BatchOrSerial = item.BatchOrSerial;
                IM.Note = note;
                IM.UserName = user;
                IM.ProId = item.ProductId;
                IM.CompanyId = comid.CompanyId;
                IM.WarehouseId = orders.WarehouseId;
                db.InventoryMovement.Add(IM);
            }

            Transaction transaction = new Transaction();
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Purchase Return");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Purchase Return").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Purchase Return";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);


            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var supplierid = orders.SupplierID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var supplierledger = db.Ledger.Where(x => x.RefNo == supplierid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var supplierledgerCode = db.Ledger.Where(x => x.RefNo == supplierid).Select(s => s.LedgerCode).SingleOrDefault();
            var supplierledgerId = db.Ledger.Where(x => x.RefNo == supplierid).Select(s => s.Id).SingleOrDefault();

            var transactionid = transaction.Id;
            var vouchernos = transaction.VoucherNo;


            var username = User.Identity.GetUserId();
            var companyid = db.Company.Where(x => x.ApplicationUser_Company.Any(a => a.ApplicationUser_Id == username)).Select(x => x.Id).SingleOrDefault();

            var discountAmount = orders.TotalDiscount;
            var discountLedgerName = "Discount";
            var discountLedgerId = db.Ledger.Where(x => x.Name == discountLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var discountLedgerCode = db.Ledger.Where(x => x.Id == discountLedgerId).Select(s => s.LedgerCode).SingleOrDefault();


            var texesAmount = orders.VatAmount;
            var texesLedgerName = "Texes Payable";
            var texesLedgerId = db.Ledger.Where(x => x.Name == texesLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var texesLedgerCode = db.Ledger.Where(x => x.Id == texesLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var addAmount = orders.Addamount;
            var addLedgerName = "Cost Of Goods Purchase";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Trade Discount";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = supplierledger,
                             LedgerNo = supplierledgerCode.ToString(), LedgerID = supplierledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};
            }



            foreach (var ts in transactiondata)
            {
                ts.Id = Guid.NewGuid();
                ts.VoucherNo = vouchernos;
                ts.VoucherType = vouchertypes;
                ts.Narration = ordernarration;
                ts.TransactionID = transactionid;
                ts.TransactionDate = orderdate;
                ts.BranchID = orderbranch;

                db.TransactionDetails.Add(ts);

            }

            db.SaveChanges();


            result = "Success! Order Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, pur_b_purchaseorder")]
        public ActionResult PurchaseOrder()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var supplier = db.Supplier
           .Where(a => a.Company.ApplicationUser_Company
           .Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.SupplierID = new SelectList(supplier, "SupplierId", "CompanyName");
            var ledger = db.Ledger
                 .Where(a => a.Company.ApplicationUser_Company
                 .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Finished Goods")
                 .ToList();
            ViewBag.LedgerId = new SelectList(ledger, "Id", "Name");
            var salesagent = db.SalesAgent
                            .Where(a => a.Company.ApplicationUser_Company
                            .Any(c => c.ApplicationUser_Id == username))
                            .ToList();
            ViewBag.SalesAgentId = new SelectList(salesagent, "Id", "Name");
            var warehouse = db.Warehouse
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.WarehouseId = new SelectList(warehouse, "Id", "WarehouseName");
            var company = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");

            var product = db.Products.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.Barcode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseOrder(Order orders, OrderDetail[] orderdata)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Sales Order";
            orders.InvoiceStatus = "Pending";

            var rowcount = db.Order.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Order");

            if (rowcount > 0)
            {
                var invoiceNo = db.Order.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Order").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }
            db.Order.Add(orders);

            var invoiceno = orders.InvoiceNo;
            var orderid = orders.Id;
            var comid = db.Warehouse.SingleOrDefault(s => s.Id == orders.WarehouseId);
            foreach (var item in orderdata)
            {
                var orderdetalsId = Guid.NewGuid();
                OrderDetail O = new OrderDetail();
                O.OrderID = orderid;
                O.Id = orderdetalsId;
                O.ProductName = item.ProductName;
                O.Quantity = item.Quantity;
                O.Rate = item.Rate;
                O.BonusQuantity = item.BonusQuantity;
                O.BatchOrSerial = item.BatchOrSerial;
                O.ProductId = item.ProductId;
                O.NetTotal = item.NetTotal;
                O.Discount = item.Discount;
                O.TotalAmount = item.TotalAmount;
                O.VAT = item.VAT;
                O.ProductCode = item.ProductCode;
                O.ProductDescription = item.ProductDescription;
                O.MeasureUnit = item.MeasureUnit;
                db.OrderDetail.Add(O);

                InventoryUpgoing Invgo = new InventoryUpgoing();
                Invgo.Id = Guid.NewGuid();
                Invgo.ItemId = item.ProductId;
                Invgo.ItemCode = item.ProductCode;
                Invgo.InvoiceNo = invoiceno;
                Invgo.InvoiceType = "Sales Order";
                Invgo.ItemQuantity = item.Quantity;
                Invgo.ItemUnitCost = item.Rate;
                Invgo.BatchOrSerial = item.BatchOrSerial;
                Invgo.Status = true;
                Invgo.WarehouseId = orders.WarehouseId;
                Invgo.CompanyId = comid.CompanyId;
                db.InventoryUpgoing.Add(Invgo);
            }
            db.SaveChanges();


            result = "Success! Order Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, pur_b_purchaseinvoicereport")]
        public ActionResult PurchaseInvoiceReport()
        {
            List<PurchaseInvoiceReportVM> allOrder = new List<PurchaseInvoiceReportVM>();

            // here MyDatabaseEntities is our data context
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var o = db.PurchaseOrder.Where(a => a.Branch.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).OrderByDescending(a => a.Id)
                .Include(a => a.Branch).Include(a => a.Supplier).Include(a => a.Ledger)
                .Include(a => a.SalesAgent).Include(a => a.Warehouse).Include(a => a.PurchaseOrderDetails);

                foreach (var i in o)
                {
                    var orderdetail = db.PurchaseOrderDetails.Where(a => a.OrderID.Equals(i.Id)).ToList();
                    allOrder.Add(new PurchaseInvoiceReportVM { PurchaseOrder = i, PurchaseOrderDetails = orderdetail });
                }
            }
            return View(allOrder.AsEnumerable());
        }
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrder order = db.PurchaseOrder.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
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