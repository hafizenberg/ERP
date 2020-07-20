using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ChamdrimERP.Controllers
{
    public class SalesbookTailorController : Controller
    {
        // GET: SalesbookTailor
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "companyAdmin, sal_b_t_salesinvoice")]
        public ActionResult SalesInvoice()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ContactFirstName + " " + s.ContactLastName + "-" + s.Phone, value = s.CustomerId });
            ViewBag.CustomerID = new SelectList(customer, "value", "Text");
            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Sales Accounts")
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
            var tailor = db.Tailor
               .Where(a => a.Company.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.TailorList = new SelectList(tailor, "Id", "Name");
            var company = db.Company
               .Where(a => a.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            var dressmodel = db.DressModel
               .Where(a => a.Company.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.dressModel = new SelectList(dressmodel, "Id", "Name");

            var product = db.Products.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.Barcode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult SalesInvoice(TailorOrder orders, TailorOrderDetail[] orderdata)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Sales Invoice";
            var rowcount = db.TailorOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice");

            if (rowcount > 0)
            {
                var invoiceNo = db.TailorOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.TailorOrder.Add(orders);

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
                TailorOrderDetail O = new TailorOrderDetail();
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
                db.TailorOrderDetail.Add(O);


                var ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
                int ttquantity = item.Quantity;
                var invId = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.Id).SingleOrDefault();
                Inventory inventory = db.Inventory.Find(invId);
                inventory.BalanceQuantity = ttStock - ttquantity;
                //  int ttFStock = ttStock;
                if (inventory.BalanceQuantity > -1)
                {
                    db.Entry(inventory).State = EntityState.Modified;
                }
                else
                {
                    ViewBag.Message = "Please Check Again Something Wrong!!!";
                    return View();
                }

                InventoryOutGoing Invout = new InventoryOutGoing();
                Invout.Id = Guid.NewGuid();
                Invout.ItemId = item.ProductId;
                Invout.ItemCode = item.ProductCode;
                Invout.InvoiceNo = invoiceno;
                Invout.InvoiceType = " Tailor Sales Invoice";
                Invout.ItemQuantity = item.Quantity;
                Invout.ItemUnitCost = item.Rate;
                Invout.BatchOrSerial = item.BatchOrSerial;
                Invout.Status = true;
                Invout.WarehouseId = orders.WarehouseId;
                Invout.CompanyId = comid.CompanyId;
                db.InventoryOutGoing.Add(Invout);



                var alaretLavel = db.Products.Where(x => x.ProductId == item.ProductId).Select(s => s.ProductVolume).SingleOrDefault();

                InventoryMovement IM = new InventoryMovement();
                IM.Id = Guid.NewGuid();
                IM.InvoiceNo = invoiceno;
                IM.ItemId = item.ProductId;
                IM.ItemCode = item.ProductCode;
                IM.ItemName = item.ProductName;
                IM.AleartLavel = alaretLavel;
                IM.InvoiceType = "Sales Invoice";
                IM.InQuantity = 0;
                IM.OutQuantity = item.Quantity;
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

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Sales Invoice";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);



            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var coustomerid = orders.CustomerID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var companyledger = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var costomereledgerCode = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.LedgerCode).SingleOrDefault();
            var cusromerledgerId = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Id).SingleOrDefault();

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
            var addLedgerName = "Additional Income";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Cost Of Goods Sold";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


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
        public ActionResult PosInterface()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");

            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ContactFirstName + " " + s.ContactLastName + "-" + s.Phone, value = s.CustomerId });
            ViewBag.CustomerID = new SelectList(customer, "value", "Text");

            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Sales Accounts")
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

            var paymentType = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Cash in Hand" || x.LedgerCategory.Name == " Bank Accounts").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.PaymentTypeList = new SelectList(paymentType, "value", "Text");

            return View();
        }
        [HttpPost]
        public ActionResult PosInterface(TailorOrder orders, TailorOrderDetail[] orderdata, JournalVm payment)
        {
            //,decimal? PayAmount, Guid? PaymentType
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Sales Invoice";
            var rowcount = db.TailorOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice");

            if (rowcount > 0)
            {
                var invoiceNo = db.TailorOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.TailorOrder.Add(orders);

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
                TailorOrderDetail O = new TailorOrderDetail();
                O.Barcode = item.Barcode;
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
                db.TailorOrderDetail.Add(O);


                var ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
                int ttquantity = item.Quantity;
                var invId = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.Id).SingleOrDefault();
                Inventory inventory = db.Inventory.Find(invId);
                inventory.BalanceQuantity = ttStock - ttquantity;
                //  int ttFStock = ttStock;
                if (inventory.BalanceQuantity > -1)
                {
                    db.Entry(inventory).State = EntityState.Modified;
                }
                else
                {
                    ViewBag.Message = "Please Check Again Something Wrong!!!";
                    return View();
                }

                InventoryOutGoing Invout = new InventoryOutGoing();
                Invout.Id = Guid.NewGuid();
                Invout.ItemId = item.ProductId;
                Invout.ItemCode = item.ProductCode;
                Invout.InvoiceNo = invoiceno;
                Invout.InvoiceType = " Tailor Sales Invoice";
                Invout.ItemQuantity = item.Quantity;
                Invout.ItemUnitCost = item.Rate;
                Invout.BatchOrSerial = item.BatchOrSerial;
                Invout.Status = true;
                Invout.WarehouseId = orders.WarehouseId;
                Invout.CompanyId = comid.CompanyId;
                db.InventoryOutGoing.Add(Invout);



                var alaretLavel = db.Products.Where(x => x.ProductId == item.ProductId).Select(s => s.ProductVolume).SingleOrDefault();

                InventoryMovement IM = new InventoryMovement();
                IM.Id = Guid.NewGuid();
                IM.InvoiceNo = invoiceno;
                IM.ItemId = item.ProductId;
                IM.ItemCode = item.ProductCode;
                IM.ItemName = item.ProductName;
                IM.AleartLavel = alaretLavel;
                IM.InvoiceType = "Sales Invoice";
                IM.InQuantity = 0;
                IM.OutQuantity = item.Quantity;
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

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Sales Invoice";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);



            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var coustomerid = orders.CustomerID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var companyledger = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var costomereledgerCode = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.LedgerCode).SingleOrDefault();
            var cusromerledgerId = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Id).SingleOrDefault();

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
            var addLedgerName = "Additional Income";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Cost Of Goods Sold";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


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
            if (payment.Amount != 0)
            {

                Transaction ts = new Transaction();
                ts.Id = Guid.NewGuid();
                ts.UserId = User.Identity.GetUserId();

                 rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == ts.UserId) && a.VoucherType == "Received");

                if (rowcount > 0)
                {
                    var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == ts.UserId) && a.VoucherType == "Received").Max(x => x.VoucherNo);
                    ts.VoucherNo = voucherNo + 1;
                }
                else
                {
                    ts.VoucherNo = 1000;
                }
                ts.VoucherType = "Received";
                ts.TransactionDate = payment.Date;
                ts.Narration = orders.InvoiceType + orders.InvoiceNo.ToString() ;
                ts.TrasactionalAmount = payment.Amount;
                ts.BranchID = payment.BranchName;
                db.Transaction.Add(ts);
                var transaction_id = ts.Id;
                var voucher_no = ts.VoucherNo;
                var CustomerName = db.Ledger.Where(x => x.RefNo == payment.CustomerName).Select(x => x.Name).FirstOrDefault();
                var CustomerLedgerId = db.Ledger.Where(x => x.RefNo == payment.CustomerName).Select(x => x.Id).FirstOrDefault();
                var CusLedgerNameNo = db.Ledger.Where(x => x.RefNo == payment.CustomerName).Select(x => x.LedgerCode).FirstOrDefault();

                var PayLedgerName = db.Ledger.Where(x => x.Id == payment.PaymentType).Select(x => x.Name).FirstOrDefault();
                var PayLedgerNameNo = db.Ledger.Where(x => x.Id == payment.PaymentType).Select(x => x.LedgerCode).FirstOrDefault();

                List<TransactionDetails> td = new List<TransactionDetails>() {
                         new TransactionDetails() { LedgerName = CustomerName,LedgerNo = CusLedgerNameNo.ToString(),  CreditAmount = payment.Amount,LedgerID = CustomerLedgerId },
                         new TransactionDetails() { LedgerName = PayLedgerName,LedgerNo = PayLedgerNameNo.ToString(),DebitAmount  = payment.Amount,LedgerID = payment.PaymentType   }
                    };
                foreach (var T in td)
                {
                    T.Id = Guid.NewGuid();
                    T.VoucherNo = voucher_no;
                    T.VoucherType = ts.VoucherType;
                    T.Narration = ts.Narration;
                    T.TransactionDate = ts.TransactionDate;
                    T.TransactionID = transaction_id;
                    T.BranchID = ts.BranchID;
                    db.TransactionDetails.Add(T);
                }
            }

            db.SaveChanges();

            var ordernewid = orders.Id.ToString();
            result = ordernewid;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getProductsList(string barcode)
        {
            if (barcode != null)
            {
                var username = User.Identity.GetUserId();
                var all = db.InventoryMovement.OrderBy(a => a.CreatedOn).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
                var data2 = all.OrderBy(x => x.ItemCode).GroupBy(x => x.ItemId).Select(s => new
                {
                    InInventory = s.Sum(x => x.InQuantity),
                    OutInventory = s.Sum(x => x.OutQuantity),
                    BalanceInventory = (s.Sum(x => x.InQuantity) - s.Sum(x => x.OutQuantity)),
                    ItemID = s.FirstOrDefault().ItemId
                }).Where(x=>x.ItemID == db.Products.Where(a=>a.Barcode == barcode).Select(s=>s.ProductId).FirstOrDefault()).Select(b=>b.BalanceInventory).FirstOrDefault();
                var data = db.Products.Where(x => x.Barcode == barcode).Select(s => new
                {
                    OpeningQuantity = data2,
                    ProductId = s.ProductId,
                    Barcode = s.Barcode,
                    ProductCode = s.ProductCode,
                    ProductName = s.ProductName,
                    ProductDescription = s.ProductDescription,
                    ProductQuantity = s.ProductQuantity,
                    ProductPrice = s.ProductPrice,
                    ModelName = s.ModelName,
                    ProductVat = s.Vat,
                    ProductMeasure = s.ProductMeasureUnit.Name
                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("An Error Has occoured");
            }
            //return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult getProductList() //It will be fired from Jquery ajax call  
        {
            ApplicationDbContext db = new ApplicationDbContext();

            db.Configuration.ProxyCreationEnabled = false;

            List<Product> product = new List<Product>();

            var jsonData = db.Products.ToList(); 
                var toReturn = jsonData.Select(S => new
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
            return Json(toReturn, JsonRequestBehavior.AllowGet);

        }
        public ActionResult addcustomer()
        {
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            return View();
        }
        [HttpPost]
        public ActionResult addcustomer(Customer customer)
        {
            var username = User.Identity.GetUserId();
            string result = "";
            var isExist = IsDataExist(customer.Phone);
            if (isExist)
            {

                ModelState.AddModelError("PhoneExist", "Phone number is already exist");
                var companyLists = db.Company
                       .Where(a => a.ApplicationUser_Company
                       .Any(c => c.ApplicationUser_Id == username))
                       .ToList();
                ViewBag.CompanyId = new SelectList(companyLists, "Id", "CompanyName", customer.CompanyId);
                result = "Phone number is already exist";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            customer.CustomerId = Guid.NewGuid();
            var rowcount = db.Customers.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

            if (rowcount > 0)
            {
                var cuscode = db.Customers.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.CustomerCode);
                customer.CustomerCode = cuscode + 1;
            }
            else
            {
                customer.CustomerCode = 1000;
            }
            db.Customers.Add(customer);
            db.SaveChanges();

            var Id = customer.CustomerId;

            Ledger lg = new Ledger();


            var ledgeRrowcount = db.Ledger.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

            if (ledgeRrowcount > 0)
            {
                var LedgerCode = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.LedgerCode);
                lg.LedgerCode = LedgerCode + 1;
            }
            else
            {
                lg.LedgerCode = 1000;
            }

            lg.Id = Guid.NewGuid();
            lg.Email = customer.Email;
            lg.Country = customer.Country;
            lg.Name = customer.ContactFirstName + ' '+customer.ContactLastName + ' ' + '(' + customer.CustomerCode + ')';
            lg.Address = customer.Country + ',' + customer.State + ',' + customer.City;
            lg.EffectInventory = false;
            lg.EffectPayrool = false;
            lg.Country = customer.Country;
            lg.State = customer.State;
            lg.City = customer.City;
            lg.OpeningBalance = customer.OpeningBlance;
            lg.PhoneNo = customer.Phone;
            lg.RefType = "Customer";
            lg.RefNo = Id;
            lg.CompanyId = customer.CompanyId;
            var lcat = db.LedgerCategory.Where(a => a.ChartOfAccount.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().Where(x => x.Name == "Accounts Recieveable (Customer)").Select(s => s.Id).SingleOrDefault();
            lg.LedgerCategoryId = lcat;
            lg.isDefault = true;

            db.Ledger.Add(lg);

            var ledgerid = lg.Id;
            var openingbalance = lg.OpeningBalance;

            if (openingbalance != null || openingbalance != 0)
            {
                Transaction tr = new Transaction();
                tr.Id = Guid.NewGuid();
                tr.UserId = username;
                tr.BranchID = db.Branch.Where(x => x.CompanyId == customer.CompanyId).OrderBy(o => o.CreatedOn).Select(s => s.Id).ToList().FirstOrDefault();
                tr.VoucherNo = 1000;
                tr.VoucherType = "Opening Balance";
                tr.TrasactionalAmount = Convert.ToDecimal(openingbalance);
                tr.TransactionDate = DateTime.Now;

                db.Transaction.Add(tr);


                TransactionDetails td = new TransactionDetails();

                var balance = openingbalance.ToString().ToCharArray();
                if (balance[0] == '-')
                {
                    var amount = openingbalance.ToString().Replace("-", "");
                    td.CreditAmount = Convert.ToDecimal(amount);
                }
                else
                {

                    td.DebitAmount = openingbalance;
                }
                td.Id = Guid.NewGuid();
                td.VoucherNo = tr.VoucherNo;
                td.VoucherType = tr.VoucherType;
                td.BranchID = tr.BranchID;
                td.TransactionDate = tr.TransactionDate;
                td.TransactionID = tr.Id;
                td.LedgerName = lg.Name;
                td.LedgerNo = lg.LedgerCode.ToString();
                td.LedgerID = ledgerid;

                db.TransactionDetails.Add(td);
            }


            var id = lg.Id;
            var parent = lcat;
            var name = lg.Name;
            var comid = lg.CompanyId;
            ChartTree ctree = new ChartTree();
            ctree.id = id.ToString();
            ctree.parent = parent.ToString();
            ctree.text = name;
            ctree.isLedger = true;
            ctree.type = "ledger";
            ctree.CompanyId = comid;

            db.ChartTree.Add(ctree);

            db.SaveChanges();
            result = "SUCCESS";

            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", customer.CompanyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // [WebMethod]
        [NonAction]
        public bool IsDataExist(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.Customers
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Phone == data);
                return v != null;
            }
        }

        [HttpGet]
        public ActionResult SalesOrder()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");

            var customer = db.Customers
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CustomerID = new SelectList(customer, "CustomerId", "CompanyName");
            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Sales Accounts")
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
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ProductCode + "-" + s.ProductName, value = s.ProductId });

            ViewBag.ProductList = new SelectList(product, "value", "Text");
            return View();
        }
        [HttpPost]
        public ActionResult SalesOrder(Order orders, OrderDetail[] orderdata)
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

        public ActionResult SeawingInvoice()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ContactFirstName + " " + s.ContactLastName + "-" + s.Phone, value = s.CustomerId });
            ViewBag.CustomerID = new SelectList(customer, "value", "Text");

            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Sales Accounts")
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
            var tailor = db.Tailor
               .Where(a => a.Company.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.TailorList = new SelectList(tailor, "Id", "Name");
            var company = db.Company
               .Where(a => a.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            var dressmodel = db.DressModel
              .Where(a => a.Company.ApplicationUser_Company
              .Any(c => c.ApplicationUser_Id == username))
              .ToList();
            ViewBag.dressModel = new SelectList(dressmodel, "Name", "Name");


            var product = db.Products.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.Barcode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");

            return View();
        }
        [HttpPost]
        public ActionResult SeawingInvoice(TailorOrder orders, TailorOrderDetail[] orderdata, SeawingOrderDetails[] orderdatasw)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Sales Invoice";
            var rowcount = db.TailorOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice");

            if (rowcount > 0)
            {
                var invoiceNo = db.TailorOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.TailorOrder.Add(orders);

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
                TailorOrderDetail O = new TailorOrderDetail();
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
                db.TailorOrderDetail.Add(O);

                var ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
                int ttquantity = item.Quantity;
                var invId = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.Id).SingleOrDefault();
                Inventory inventory = db.Inventory.Find(invId);
                inventory.BalanceQuantity = ttStock - ttquantity;
                //  int ttFStock = ttStock;
                if (inventory.BalanceQuantity > -1)
                {
                    db.Entry(inventory).State = EntityState.Modified;
                }
                else
                {
                    ViewBag.Message = "Please Check Again Something Wrong!!!";
                    return View();
                }


                InventoryOutGoing Invout = new InventoryOutGoing();
                Invout.Id = Guid.NewGuid();
                Invout.ItemId = item.ProductId;
                Invout.ItemCode = item.ProductCode;
                Invout.InvoiceNo = invoiceno;
                Invout.InvoiceType = "Tailor Sales Invoice";
                Invout.ItemQuantity = item.Quantity;
                Invout.ItemUnitCost = item.Rate;
                Invout.BatchOrSerial = item.BatchOrSerial;
                Invout.Status = true;
                Invout.WarehouseId = orders.WarehouseId;
                Invout.CompanyId = comid.CompanyId;
                db.InventoryOutGoing.Add(Invout);

                var alaretLavel = db.Products.Where(x => x.ProductId == item.ProductId).Select(s => s.ProductVolume).SingleOrDefault();

                InventoryMovement IM = new InventoryMovement();
                IM.Id = Guid.NewGuid();
                IM.InvoiceNo = invoiceno;
                IM.ItemId = item.ProductId;
                IM.ItemCode = item.ProductCode;
                IM.ItemName = item.ProductName;
                IM.AleartLavel = alaretLavel;
                IM.InvoiceType = "Sales Invoice";
                IM.InQuantity = 0;
                IM.OutQuantity = item.Quantity;
                IM.BatchOrSerial = item.BatchOrSerial;
                IM.Note = note;
                IM.UserName = user;
                IM.ProId = item.ProductId;
                IM.CompanyId = comid.CompanyId;
                IM.WarehouseId = orders.WarehouseId;
                db.InventoryMovement.Add(IM);
            }
            foreach (var item in orderdatasw)
            {
                var orderdataswId = Guid.NewGuid();
                SeawingOrderDetails O = new SeawingOrderDetails();
                O.TailorOrderId = orderid;
                O.TulLong = item.TulLong;
                O.ArradBody = item.ArradBody;
                O.KocchorWaist = item.KocchorWaist;
                O.Hip = item.Hip;
                O.IbadMohra = item.IbadMohra;
                O.KhetabShoulder = item.KhetabShoulder;
                O.OrdunHand = item.OrdunHand;
                O.Orgoba = item.Orgoba;
                O.Throat = item.Throat;
                O.GherEnclorure = item.GherEnclorure;
                O.Id = orderdataswId;
                O.Qty = item.Qty;
                O.Rate = item.Rate;
                O.TotalAmount = item.TotalAmount;               
                db.SeawingOrderDetails.Add(O);
            }

            Transaction transaction = new Transaction();
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Sales Invoice";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);


            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var coustomerid = orders.CustomerID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var companyledger = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var costomereledgerCode = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.LedgerCode).SingleOrDefault();
            var cusromerledgerId = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Id).SingleOrDefault();

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
            var addLedgerName = "Additional Income";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Cost Of Goods Sold";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
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
        // GET: Salesbook
        [Authorize(Roles = "companyAdmin, sal_b_t_salesinvoicereport")]
        public ActionResult SalesInvoiceReport()
        {
            List<SalesInvoiceReportVMtailor> allOrder = new List<SalesInvoiceReportVMtailor>();

            // here MyDatabaseEntities is our data context
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var o = db.TailorOrder.Where(a => a.Branch.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).OrderByDescending(a => a.Id)
                .Include(a => a.Branch).Include(a => a.Customer).Include(a => a.Ledger)
                .Include(a => a.SalesAgent).Include(a => a.Warehouse).Include(a => a.TailorOrderDetail);

                foreach (var i in o)
                {
                    var orderdetail = db.TailorOrderDetail.Where(a => a.OrderID.Equals(i.Id)).ToList();
                    allOrder.Add(new SalesInvoiceReportVMtailor { TailorOrder = i, TailorOrderDetail = orderdetail });
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
            TailorOrder order = db.TailorOrder.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        public ActionResult PosPrint(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TailorOrder order = db.TailorOrder.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var orderdate = order.CreatedOn.AddSeconds(1);
            ViewBag.Amount = db.Transaction.Where(x=>x.Narration == order.InvoiceType + order.InvoiceNo && x.TransactionDate==order.OrderDate).Select(s=>s.TrasactionalAmount).FirstOrDefault();
            var ClosingBalance = db.TransactionDetails.OrderBy(a => a.CreatedOn).Where(x => x.Ledger.RefNo == order.CustomerID && x.CreatedOn <= orderdate).ToList().OrderBy(x => x.LedgerNo).GroupBy(x => x.LedgerNo).Select(s =>  (s.Sum(x => x.DebitAmount) - s.Sum(x => x.CreditAmount))
            ).FirstOrDefault();
            ViewBag.ClosingBalance = ClosingBalance.ToString();
            return View(order);
        }
        public ActionResult GetClosingBalance(Guid? Id)
        {
            if (Id != null)
            {
                var all = db.TransactionDetails.OrderBy(a => a.CreatedOn).Where(x => x.Ledger.RefNo == Id).ToList();
                var data = all.OrderBy(x => x.LedgerNo).GroupBy(x => x.LedgerNo).Select(s => new
                {
                    ClosinngBalance = (s.Sum(x => x.DebitAmount) - s.Sum(x => x.CreditAmount))
                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return View();

        }
        [HttpGet]
        public ActionResult seawing()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ContactFirstName + " " + s.ContactLastName + "-" + s.Phone, value = s.CustomerId });
            ViewBag.CustomerID = new SelectList(customer, "value", "Text");

            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Sales Accounts")
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
            var tailor = db.Tailor
               .Where(a => a.Company.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.TailorList = new SelectList(tailor, "Id", "Name");
            var company = db.Company
               .Where(a => a.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            var dressmodel = db.DressModel
              .Where(a => a.Company.ApplicationUser_Company
              .Any(c => c.ApplicationUser_Id == username))
              .ToList();
            ViewBag.dressModel = new SelectList(dressmodel, "Name", "Name");


            var product = db.Products.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ProductCode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult seawing(TailorOrder orders, SeawingOrderDetails[] orderdatasw)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Sales Invoice";
            var rowcount = db.TailorOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice");

            if (rowcount > 0)
            {
                var invoiceNo = db.TailorOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.TailorOrder.Add(orders);

            var user = orders.UserId;
            var note = orders.Narration;

            var orderamount = orders.InvoicedAmount;
            var orderbranch = orders.BranchId;
            var orderdate = orders.OrderDate;
            var ordernarration = orders.Narration;

            var invoiceno = orders.InvoiceNo;
            var orderid = orders.Id;
            var comid = db.Warehouse.SingleOrDefault(s => s.Id == orders.WarehouseId);

            foreach (var item in orderdatasw)
            {
                var orderdataswId = Guid.NewGuid();
                SeawingOrderDetails O = new SeawingOrderDetails();
                O.TailorOrderId = orderid;
                O.TulLong = item.TulLong;
                O.ArradBody = item.ArradBody;
                O.KocchorWaist = item.KocchorWaist;
                O.Hip = item.Hip;
                O.IbadMohra = item.IbadMohra;
                O.KhetabShoulder = item.KhetabShoulder;
                O.OrdunHand = item.OrdunHand;
                O.Orgoba = item.Orgoba;
                O.Throat = item.Throat;
                O.GherEnclorure = item.GherEnclorure;
                O.Id = orderdataswId;
                O.Qty = item.Qty;
                O.Rate = item.Rate;
                O.TotalAmount = item.TotalAmount;
                db.SeawingOrderDetails.Add(O);
            }

            Transaction transaction = new Transaction();
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Sales Invoice";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);


            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var coustomerid = orders.CustomerID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var companyledger = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var costomereledgerCode = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.LedgerCode).SingleOrDefault();
            var cusromerledgerId = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Id).SingleOrDefault();

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
            var addLedgerName = "Additional Income";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Cost Of Goods Sold";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
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
        [HttpGet]
        public ActionResult SeawingArea()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ContactFirstName + " " + s.ContactLastName + "-" + s.Phone, value = s.CustomerId });
            ViewBag.CustomerID = new SelectList(customer, "value", "Text");

            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Sales Accounts")
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
            var tailor = db.Tailor
               .Where(a => a.Company.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.TailorList = new SelectList(tailor, "Id", "Name");
            var company = db.Company
               .Where(a => a.ApplicationUser_Company
               .Any(c => c.ApplicationUser_Id == username))
               .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            var dressmodel = db.DressModel
              .Where(a => a.Company.ApplicationUser_Company
              .Any(c => c.ApplicationUser_Id == username))
              .ToList();
            ViewBag.dressModel = new SelectList(dressmodel, "Name", "Name");


            var product = db.Products.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ProductCode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");

            return View();
        }
        [HttpPost]
        public ActionResult SeawingArea(TailorOrder orders, SeawingOrderDetails[] orderdatasw)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Sales Invoice";
            var rowcount = db.TailorOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice");

            if (rowcount > 0)
            {
                var invoiceNo = db.TailorOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Invoice").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.TailorOrder.Add(orders);

            var user = orders.UserId;
            var note = orders.Narration;

            var orderamount = orders.InvoicedAmount;
            var orderbranch = orders.BranchId;
            var orderdate = orders.OrderDate;
            var ordernarration = orders.Narration;

            var invoiceno = orders.InvoiceNo;
            var orderid = orders.Id;
            var comid = db.Warehouse.SingleOrDefault(s => s.Id == orders.WarehouseId);

            foreach (var item in orderdatasw)
            {
                var orderdataswId = Guid.NewGuid();
                SeawingOrderDetails O = new SeawingOrderDetails();
                O.TailorOrderId = orderid;
                O.Shoulder = item.Shoulder;
                O.Neck = item.Neck;
                O.Bust = item.Bust;
                O.Hip = item.Hip;
                O.Waist = item.Waist;
                O.ArmHole = item.ArmHole;
                O.Muscle = item.Muscle;
                O.Elbow = item.Elbow;
                O.Cuff = item.Cuff;
                O.SleeveLength = item.SleeveLength;
                O.Length = item.Length;
                O.LenghtFromShoulderToHip = item.LenghtFromShoulderToHip;
                O.PantLength = item.PantLength;
                O.LegOpening = item.LegOpening;
                O.Note = ordernarration;
                O.Id = orderdataswId;
                O.Qty = item.Qty;
                O.Rate = item.Rate;
                O.TotalAmount = item.TotalAmount;
                db.SeawingOrderDetails.Add(O);
            }

            Transaction transaction = new Transaction();
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Sales Invoice").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Sales Invoice";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);


            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var coustomerid = orders.CustomerID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var companyledger = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var costomereledgerCode = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.LedgerCode).SingleOrDefault();
            var cusromerledgerId = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Id).SingleOrDefault();

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
            var addLedgerName = "Additional Income";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Cost Of Goods Sold";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};

            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount != 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { CreditAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { DebitAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { DebitAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { CreditAmount = orders.TotalAmount + orders.TotalDiscount  + orders.SeawingAmount,LedgerName = transactinledger,
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
        [HttpGet]
        public ActionResult SalesReturn()
        {
            var username = User.Identity.GetUserId();
            ViewBag.UserName = User.Identity.GetUserName();
            var branch = db.Branch
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");

            var customer = db.Customers.Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Select(s => new { Text = s.ContactFirstName + " " + s.ContactLastName + "-" + s.Phone, value = s.CustomerId });
            ViewBag.CustomerID = new SelectList(customer, "value", "Text");
            var ledger = db.Ledger
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Sales Accounts")
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
        public ActionResult SalesReturn(TailorOrder orders, TailorOrderDetail[] orderdata)
        {
            string result = "Error! Order Is Not Complete!";
            orders.UserId = User.Identity.GetUserId();
            orders.Id = Guid.NewGuid();
            orders.InvoiceType = "Sales Return";
            var rowcount = db.TailorOrder.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Return");

            if (rowcount > 0)
            {
                var invoiceNo = db.TailorOrder.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == orders.UserId) && a.InvoiceType == "Sales Return").Max(x => x.InvoiceNo);
                orders.InvoiceNo = invoiceNo + 1;
            }
            else
            {
                orders.InvoiceNo = 1000;
            }

            db.TailorOrder.Add(orders);

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
                TailorOrderDetail O = new TailorOrderDetail();
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
                db.TailorOrderDetail.Add(O);

                var ttStock = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.BalanceQuantity).SingleOrDefault();
                int ttquantity = item.Quantity;
                var invId = db.Inventory.Where(x => x.ItemId == item.ProductId).Select(s => s.Id).SingleOrDefault();
                Inventory inventory = db.Inventory.Find(invId);
                inventory.BalanceQuantity = ttStock + ttquantity;
                //  int ttFStock = ttStock;
                if (inventory.BalanceQuantity > -1)
                {
                    db.Entry(inventory).State = EntityState.Modified;
                }
                else
                {
                    ViewBag.Message = "Please Check Again Something Wrong!!!";
                    return View();
                }

                InventoryIncomming Invout = new InventoryIncomming();
                Invout.Id = Guid.NewGuid();
                Invout.ItemId = item.ProductId;
                Invout.ItemCode = item.ProductCode;
                Invout.InvoiceNo = invoiceno;
                Invout.InvoiceType = "Sales Return";
                Invout.ItemQuantity = item.Quantity;
                Invout.ItemUnitCost = item.Rate;
                Invout.BatchOrSerial = item.BatchOrSerial;
                Invout.Status = true;
                Invout.WarehouseId = orders.WarehouseId;
                Invout.CompanyId = comid.CompanyId;
                db.InventoryIncomming.Add(Invout);

                var alaretLavel = db.Products.Where(x => x.ProductId == item.ProductId).Select(s => s.ProductVolume).SingleOrDefault();

                InventoryMovement IM = new InventoryMovement();
                IM.Id = Guid.NewGuid();
                IM.InvoiceNo = invoiceno;
                IM.ItemId = item.ProductId;
                IM.ItemCode = item.ProductCode;
                IM.ItemName = item.ProductName;
                IM.AleartLavel = alaretLavel;
                IM.InvoiceType = "Sales Return";
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

            rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId)
            && a.VoucherType == "Sales Return");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId)
                && a.VoucherType == "Sales Return").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            transaction.TrasactionalAmount = orderamount;
            transaction.BranchID = orderbranch;
            transaction.TransactionDate = orderdate;
            transaction.VoucherType = "Sales Return";
            transaction.Narration = ordernarration;
            db.Transaction.Add(transaction);



            var voucherno = transaction.VoucherNo;
            var vouchertypes = transaction.VoucherType;

            var transactinledgerid = orders.LedgerId;
            var coustomerid = orders.CustomerID;

            var transactinledger = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.Name).SingleOrDefault();
            var companyledger = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Name).SingleOrDefault();

            var transactinledgerCode = db.Ledger.Where(x => x.Id == transactinledgerid).Select(s => s.LedgerCode).SingleOrDefault();
            var costomereledgerCode = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.LedgerCode).SingleOrDefault();
            var cusromerledgerId = db.Ledger.Where(x => x.RefNo == coustomerid).Select(s => s.Id).SingleOrDefault();

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
            var addLedgerName = "Additional Income";
            var addLedgerId = db.Ledger.Where(x => x.Name == addLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var addLedgerCode = db.Ledger.Where(x => x.Id == addLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            var lessAmount = orders.LaseAmount;
            var lessLedgerName = "Cost Of Goods Sold";
            var lessLedgerId = db.Ledger.Where(x => x.Name == lessLedgerName && x.CompanyId == companyid).Select(s => s.Id).SingleOrDefault();
            var lessLedgerCode = db.Ledger.Where(x => x.Id == lessLedgerId).Select(s => s.LedgerCode).SingleOrDefault();

            List<TransactionDetails> transactiondata = new List<TransactionDetails>();
            if (orders.VatAmount != 0 && orders.TotalDiscount != 0 && orders.Addamount != 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
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

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = texesAmount,LedgerName = texesLedgerName,
                             LedgerNo = texesLedgerCode.ToString(),LedgerID = texesLedgerId }};
            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount != 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { CreditAmount = discountAmount,LedgerName = discountLedgerName,
                             LedgerNo = discountLedgerCode.ToString(), LedgerID = discountLedgerId },


                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount != 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid },
                      new TransactionDetails() { DebitAmount = addAmount,LedgerName = addLedgerName,
                             LedgerNo = addLedgerCode.ToString(),LedgerID = addLedgerId }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount != 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },
                      new TransactionDetails() { CreditAmount = lessAmount,LedgerName = lessLedgerName,
                             LedgerNo = lessLedgerCode.ToString(), LedgerID = lessLedgerId },

                      new TransactionDetails() { DebitAmount = orders.TotalAmount + orders.TotalDiscount,LedgerName = transactinledger,
                             LedgerNo = transactinledgerCode.ToString(),LedgerID = transactinledgerid }};

            }
            else if (orders.VatAmount == 0 && orders.TotalDiscount == 0 && orders.Addamount == 0 && orders.LaseAmount == 0)
            {
                transactiondata = new List<TransactionDetails>() {

                      new TransactionDetails() { CreditAmount = orderamount,LedgerName = companyledger,
                             LedgerNo = costomereledgerCode.ToString(), LedgerID = cusromerledgerId },


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


            result = "Success! Requiest Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
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