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
    public class EasyAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: EasyAccount
        public ActionResult SupplierPayment()
        {
            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x=>x.LedgerCategory.Name == "Cash in Hand" || x.LedgerCategory.Name == " Bank Accounts").Select(s =>
            new {
                Text = s.LedgerCode + "-" + s.Name,
                value = s.Id
            });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");

            var supplier = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Accounts Payable (Supplier)").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.SupplierList = new SelectList(supplier, "value", "Text");

            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }
        [HttpPost]
        public ActionResult SupplierPayment( JournalVm model )
        {
            string massage = " Data can not Submitted ";
            try
            {
                if (model.Amount != 0)
                {
                    Transaction ts = new Transaction();
                    ts.Id = Guid.NewGuid();
                    ts.UserId = User.Identity.GetUserId();

                    var rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == ts.UserId) && a.VoucherType == "Payment");

                    if (rowcount > 0)
                    {
                        var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == ts.UserId) && a.VoucherType == "Payment").Max(x => x.VoucherNo);
                        ts.VoucherNo = voucherNo + 1;
                    }
                    else
                    {
                        ts.VoucherNo = 1000;
                    }
                    ts.VoucherType = "Payment";
                    ts.TransactionDate = model.Date;
                    ts.Narration = model.Narration;
                    ts.TrasactionalAmount = model.Amount;
                    ts.BranchID = model.BranchName;
                    db.Transaction.Add(ts);
                    var transactionid = ts.Id;
                    var voucherno = ts.VoucherNo;
                    var SupLedgerName = db.Ledger.Where(x => x.Id == model.SupplierName).Select(x=>x.Name).FirstOrDefault();
                    var SupLedgerNameNo = db.Ledger.Where(x => x.Id == model.SupplierName).Select(x => x.LedgerCode).FirstOrDefault();
                    var PayLedgerName = db.Ledger.Where(x => x.Id == model.PaymentType).Select(x => x.Name).FirstOrDefault();
                    var PayLedgerNameNo = db.Ledger.Where(x => x.Id == model.PaymentType).Select(x => x.LedgerCode).FirstOrDefault();
                    List<TransactionDetails> td = new List<TransactionDetails>() {
                         new TransactionDetails() { LedgerName = SupLedgerName,LedgerNo = SupLedgerNameNo.ToString(), DebitAmount = model.Amount,LedgerID = model.SupplierName },
                         new TransactionDetails() { LedgerName = PayLedgerName,LedgerNo = PayLedgerNameNo.ToString(), CreditAmount = model.Amount,LedgerID = model.PaymentType   }
                    };
                    foreach (var T in td)
                    {
                        T.Id = Guid.NewGuid();
                        T.VoucherNo = voucherno;
                        T.VoucherType = ts.VoucherType;
                        T.Narration = ts.Narration;
                        T.TransactionDate = ts.TransactionDate;
                        T.TransactionID = transactionid;
                        T.BranchID = ts.BranchID;
                        db.TransactionDetails.Add(T);
                    }
                    db.SaveChanges();
                    string Successmassage = "Success Data Submitted";
                    ViewBag.SuccessMassage = Successmassage;
                    ModelState.Clear();
                }

            }
            catch (EntityException ex)
            {
                return Content(" " + ex);
            }

            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Cash in Hand" || x.LedgerCategory.Name == " Bank Accounts").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text",model.PaymentType);

            var supplier = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Accounts Payable (Supplier)").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.SupplierList = new SelectList(supplier, "value", "Text",model.SupplierName);

            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName",model.BranchName);
            ViewBag.ErrorMassage = massage;
            ModelState.Clear();
            return View(model);

        }
        public ActionResult Expenses()
        {
            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Cash in Hand" || x.LedgerCategory.Name == " Bank Accounts").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");

            var lcatid = db.LedgerCategory.Where(a => a.ChartOfAccount.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.Name == "Indirect Expense").Select(s => s.Id).SingleOrDefault();
            var expenses = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Indirect Expense" || x.LedgerCategory.ParentLedgerCatId == lcatid).Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.ExpensesList = new SelectList(expenses, "value", "Text");

            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }
        [HttpPost]
        public ActionResult Expenses(JournalVm model)
        {
            string massage = " Data can not Submitted ";
            try
            {
                if (model.Amount != 0)
                {

                    Transaction ts = new Transaction();
                    ts.Id = Guid.NewGuid();
                    ts.UserId = User.Identity.GetUserId();

                    var rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == ts.UserId) && a.VoucherType == "Payment");

                    if (rowcount > 0)
                    {
                        var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == ts.UserId) && a.VoucherType == "Payment").Max(x => x.VoucherNo);
                        ts.VoucherNo = voucherNo + 1;
                    }
                    else
                    {
                        ts.VoucherNo = 1000;
                    }
                    ts.VoucherType = "Payment";
                    ts.TransactionDate = model.Date;
                    ts.Narration = model.Narration;
                    ts.TrasactionalAmount = model.Amount;
                    ts.BranchID = model.BranchName;
                    db.Transaction.Add(ts);
                    var transactionid = ts.Id;
                    var voucherno = ts.VoucherNo;
                    var ExpensesLedgerName = db.Ledger.Where(x => x.Id == model.Expenses).Select(x => x.Name).FirstOrDefault();
                    var ExpensesLedgerNameNo = db.Ledger.Where(x => x.Id == model.Expenses).Select(x => x.LedgerCode).FirstOrDefault();

                    var PayLedgerName = db.Ledger.Where(x => x.Id == model.PaymentType).Select(x => x.Name).FirstOrDefault();
                    var PayLedgerNameNo = db.Ledger.Where(x => x.Id == model.PaymentType).Select(x => x.LedgerCode).FirstOrDefault();

                    List<TransactionDetails> td = new List<TransactionDetails>() {
                         new TransactionDetails() { LedgerName = ExpensesLedgerName,LedgerNo = ExpensesLedgerNameNo.ToString(), DebitAmount = model.Amount,LedgerID = model.SupplierName },
                         new TransactionDetails() { LedgerName = PayLedgerName,LedgerNo = PayLedgerNameNo.ToString(), CreditAmount = model.Amount,LedgerID = model.PaymentType   }
                    };
                    foreach (var T in td)
                    {
                        T.Id = Guid.NewGuid();
                        T.VoucherNo = voucherno;
                        T.VoucherType = ts.VoucherType;
                        T.Narration = ts.Narration;
                        T.TransactionDate = ts.TransactionDate;
                        T.TransactionID = transactionid;
                        T.BranchID = ts.BranchID;
                        db.TransactionDetails.Add(T);
                    }

                    db.SaveChanges();
                    string Successmassage = "Success Data Submitted";
                    ViewBag.SuccessMassage = Successmassage;
                    ModelState.Clear();
                }

            }
            catch (EntityException ex)
            {
                return Content(" " + ex);
            }

            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Cash in Hand" || x.LedgerCategory.Name == " Bank Accounts").Select(s =>
               new {
                   Text = s.LedgerCode + "-" + s.Name,
                   value = s.Id
               });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text", model.PaymentType);


            var catid = db.LedgerCategory.Where(a => a.Name == "Indirect Expense").Select(s => s.Id).FirstOrDefault();
            var expenses = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Indirect Expense" || x.LedgerCategory.ParentLedgerCatId == catid).Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.ExpensesList = new SelectList(expenses, "value", "Text", model.Expenses);

            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName", model.BranchName);
            ViewBag.ErrorMassage = massage;
            ModelState.Clear();
            return View(model);

        }
        public ActionResult Receivd()
        {
            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Cash in Hand" || x.LedgerCategory.Name == " Bank Accounts").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");

            var customer = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Accounts Recieveable (Customer)").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.CustomerList = new SelectList(customer, "value", "Text");

            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }
        [HttpPost]
        public ActionResult Receivd(JournalVm model)
        {
            string massage = " Data can not Submitted ";
            try
            {
                if (model.Amount != 0)
                {

                    Transaction ts = new Transaction();
                    ts.Id = Guid.NewGuid();
                    ts.UserId = User.Identity.GetUserId();

                    var rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == ts.UserId) && a.VoucherType == "Received");

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
                    ts.TransactionDate = model.Date;
                    ts.Narration = model.Narration;
                    ts.TrasactionalAmount = model.Amount;
                    ts.BranchID = model.BranchName;
                    db.Transaction.Add(ts);
                    var transactionid = ts.Id;
                    var voucherno = ts.VoucherNo;
                    var CustomerName = db.Ledger.Where(x => x.Id == model.CustomerName).Select(x => x.Name).FirstOrDefault();
                    var CusLedgerNameNo = db.Ledger.Where(x => x.Id == model.CustomerName).Select(x => x.LedgerCode).FirstOrDefault();

                    var PayLedgerName = db.Ledger.Where(x => x.Id == model.PaymentType).Select(x => x.Name).FirstOrDefault();
                    var PayLedgerNameNo = db.Ledger.Where(x => x.Id == model.PaymentType).Select(x => x.LedgerCode).FirstOrDefault();

                    List<TransactionDetails> td = new List<TransactionDetails>() {
                         new TransactionDetails() { LedgerName = CustomerName,LedgerNo = CusLedgerNameNo.ToString(),  CreditAmount = model.Amount,LedgerID = model.CustomerName },
                         new TransactionDetails() { LedgerName = PayLedgerName,LedgerNo = PayLedgerNameNo.ToString(),DebitAmount  = model.Amount,LedgerID = model.PaymentType   }
                    };
                    foreach (var T in td)
                    {
                        T.Id = Guid.NewGuid();
                        T.VoucherNo = voucherno;
                        T.VoucherType = ts.VoucherType;
                        T.Narration = ts.Narration;
                        T.TransactionDate = ts.TransactionDate;
                        T.TransactionID = transactionid;
                        T.BranchID = ts.BranchID;
                        db.TransactionDetails.Add(T);
                    }
                    ModelState.Clear();
                    db.SaveChanges();
                    string Successmassage = "Success Data Submitted";
                    ViewBag.SuccessMassage = Successmassage;
                }

            }
            catch (EntityException ex)
            {
                return Content(" " + ex);
            }

            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Cash in Hand" || x.LedgerCategory.Name == " Bank Accounts").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text", model.PaymentType);

            var customer = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x => x.LedgerCategory.Name == "Accounts Recieveable (Customer)").Select(s =>
              new {
                  Text = s.LedgerCode + "-" + s.Name,
                  value = s.Id
              });

            ViewBag.CustomerList = new SelectList(customer, "value", "Text", model.CustomerName);

            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName", model.BranchName);
            ViewBag.ErrorMassage = massage;
            ModelState.Clear();
            return View(model);
        }
        public ActionResult GetClosingBalance(Guid? Id)
        {
            if (Id != null)
            {
                var all = db.TransactionDetails.OrderBy(a => a.CreatedOn).Where(x => x.Ledger.Id == Id).ToList();
                var data = all.OrderBy(x => x.LedgerNo).GroupBy(x => x.LedgerNo).Select(s => new
                {
                    ClosinngBalance = (s.Sum(x => x.DebitAmount) - s.Sum(x => x.CreditAmount))
                }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        public ActionResult PaymentReport()
        {

            List<JournalReportVM> allData = new List<JournalReportVM>();

            // here MyDatabaseEntities is our data context
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var o = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username) && a.VoucherType == "Payment").OrderByDescending(a => a.CreatedOn)
                .Include(a => a.TransactionDetailses);

                foreach (var i in o)
                {
                    var TransactionDetailses = db.TransactionDetails.Where(a => a.TransactionID.Equals(i.Id)).ToList();
                    allData.Add(new JournalReportVM { Transaction = i, TransactionDetails = TransactionDetailses });
                }
            }
            return View(allData.AsEnumerable());
        }
        public ActionResult ReceivedReport()
        {
            List<JournalReportVM> allData = new List<JournalReportVM>();

            // here MyDatabaseEntities is our data context
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var o = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username) && a.VoucherType == "Received").OrderByDescending(a => a.CreatedOn)
                .Include(a => a.TransactionDetailses);

                foreach (var i in o)
                {
                    var TransactionDetailses = db.TransactionDetails.Where(a => a.TransactionID.Equals(i.Id)).ToList();
                    allData.Add(new JournalReportVM { Transaction = i, TransactionDetails = TransactionDetailses });
                }
            }
            return View(allData.AsEnumerable());
        }
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction data = db.Transaction.Find(id);
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        public ActionResult GetCustomerInformation(Guid? id)
        {
            var cusId = db.Ledger.Where(x => x.Id == id).Select(s=>s.RefNo).FirstOrDefault();

            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Customers.Where(a=>a.CustomerId == cusId).Select(s => new
            {
                CustomerCode = s.CustomerCode,
                ContactFirstName = s.ContactFirstName,
                ContactLastName = s.ContactLastName,
                PhoneNo = s.Phone
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSupplierInformation(Guid? id)
        {
            var supId = db.Ledger.Where(x => x.Id == id).Select(s => s.RefNo).FirstOrDefault();
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Supplier.Where(a => a.SupplierId == supId).Select(s => new
            {
                SupplierCode = s.SupplierCode,
                ContactFirstName = s.ContactFirstName,
                ContactLastName = s.ContactLastName,
                PhoneNo = s.MobilePhone
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
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