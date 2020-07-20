using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net;
using Microsoft.Ajax.Utilities;
using System.Linq.Dynamic;
using System.Globalization;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Accounts
        [Authorize(Roles = "companyAdmin, acc_journal")]
        [HttpGet]
        public ActionResult Journal()
        {

            var username = User.Identity.GetUserId();  

            var ledger = db.Ledger.Where(x=>x.Status==true).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => 
            new {Text =   s.LedgerCategory.ChartOfAccount.Name +" -> "
            + s.LedgerCategory.Name + " -> " + s.LedgerCode + "-" + s.Name, value = s.Id});
            
            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");
           
            var branch = db.Branch.Where(x => x.Status == true)
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }
        [HttpPost]
        public ActionResult Journal(Transaction transaction, TransactionDetails[] transactiondata)
        {
            string result = "Error! Order Is Not Complete!";
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            var rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Journal");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Journal").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
           // transaction.TrasactionalAmount = 
            transaction.VoucherType = "Journal";

            db.Transaction.Add(transaction);
            var transactionid = transaction.Id;
            var voucherno = transaction.VoucherNo;
            var orderid = transaction.Id;
            var comid = db.Branch.SingleOrDefault(s => s.Id == transaction.BranchID);
            foreach (var item in transactiondata)
            {
                var orderdetalsId = Guid.NewGuid();
                TransactionDetails T = new TransactionDetails();
                T.Id = Guid.NewGuid();
                T.VoucherNo = voucherno;
                T.VoucherType = transaction.VoucherType;
                T.LedgerName = item.LedgerName;
                T.LedgerNo = item.LedgerNo;
                T.DebitAmount = item.DebitAmount;
                T.CreditLedger = item.CreditLedger;
                T.Narration = transaction.Narration;
                T.DabitLedger = item.DabitLedger;
                T.CreditAmount = item.CreditAmount;
                T.TransactionDate = transaction.TransactionDate;
                T.TransactionID = transactionid;
                T.BranchID = transaction.BranchID;
                T.LedgerID = item.LedgerID;

                db.TransactionDetails.Add(T);
               
            }
            db.SaveChanges();


            result = "Success! Order Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, acc_received")]
        [HttpGet]
        public ActionResult Received()
        {

            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(x => x.Status == true).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s =>
            new {
                Text = s.LedgerCategory.ChartOfAccount.Name + " -> "
            + s.LedgerCategory.Name + " -> " + s.LedgerCode + "-" + s.Name,
                value = s.Id
            });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");
            var branch = db.Branch.Where(x => x.Status == true)
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }
        [HttpPost]
        public ActionResult Received(Transaction transaction, TransactionDetails[] transactiondata)
        {
            string result = "Error! Order Is Not Complete!";
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            var rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Received");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Received").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            // transaction.TrasactionalAmount = 
            transaction.VoucherType = "Received";

            db.Transaction.Add(transaction);
            var transactionid = transaction.Id;
            var voucherno = transaction.VoucherNo;
            var orderid = transaction.Id;
            var comid = db.Branch.SingleOrDefault(s => s.Id == transaction.BranchID);
            foreach (var item in transactiondata)
            {
                var orderdetalsId = Guid.NewGuid();
                TransactionDetails T = new TransactionDetails();
                T.Id = Guid.NewGuid();
                T.VoucherNo = voucherno;
                T.VoucherType = transaction.VoucherType;
                T.LedgerName = item.LedgerName;
                T.LedgerNo = item.LedgerNo;
                T.DebitAmount = item.DebitAmount;
                T.CreditLedger = item.CreditLedger;
                T.Narration = transaction.Narration;
                T.DabitLedger = item.DabitLedger;
                T.CreditAmount = item.CreditAmount;
                T.TransactionDate = transaction.TransactionDate;
                T.TransactionID = transactionid;
                T.BranchID = transaction.BranchID;
                T.LedgerID = item.LedgerID;

                db.TransactionDetails.Add(T);

            }
            db.SaveChanges();


            result = "Success! Order Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, acc_paymen")]
        [HttpGet]
        public ActionResult Payment()
        {

            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(x => x.Status == true).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s =>
            new {
                Text = s.LedgerCategory.ChartOfAccount.Name + " -> "
            + s.LedgerCategory.Name + " -> " + s.LedgerCode + "-" + s.Name,
                value = s.Id
            });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");

            var branch = db.Branch.Where(x => x.Status == true)
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }
        [HttpPost]
        public ActionResult Payment(Transaction transaction, TransactionDetails[] transactiondata)
        {
            string result = "Error! Order Is Not Complete!";
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            var rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Payment");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Payment").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            // transaction.TrasactionalAmount = 
            transaction.VoucherType = "Payment";

            db.Transaction.Add(transaction);
            var transactionid = transaction.Id;
            var voucherno = transaction.VoucherNo;
            var orderid = transaction.Id;
            var comid = db.Branch.SingleOrDefault(s => s.Id == transaction.BranchID);
            foreach (var item in transactiondata)
            {
                var orderdetalsId = Guid.NewGuid();
                TransactionDetails T = new TransactionDetails();
                T.Id = Guid.NewGuid();
                T.VoucherNo = voucherno;
                T.VoucherType = transaction.VoucherType;
                T.LedgerName = item.LedgerName;
                T.LedgerNo = item.LedgerNo;
                T.DebitAmount = item.DebitAmount;
                T.CreditLedger = item.CreditLedger;
                T.Narration = transaction.Narration;
                T.DabitLedger = item.DabitLedger;
                T.CreditAmount = item.CreditAmount;
                T.TransactionDate = transaction.TransactionDate;
                T.TransactionID = transactionid;
                T.BranchID = transaction.BranchID;
                T.LedgerID = item.LedgerID;

                db.TransactionDetails.Add(T);

            }
            db.SaveChanges();


            result = "Success! Order Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, acc_contra")]
        [HttpGet]
        public ActionResult Contra()
        {

            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(x => x.Status == true).Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s =>
            new {
                Text = s.LedgerCategory.ChartOfAccount.Name + " -> "
            + s.LedgerCategory.Name + " -> " + s.LedgerCode + "-" + s.Name,
                value = s.Id
            });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");
            var branch = db.Branch.Where(x => x.Status == true)
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }
        [HttpPost]
        public ActionResult Contra(Transaction transaction, TransactionDetails[] transactiondata)
        {
            string result = "Error! Order Is Not Complete!";
            transaction.UserId = User.Identity.GetUserId();
            transaction.Id = Guid.NewGuid();

            var rowcount = db.Transaction.Count(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Contra");

            if (rowcount > 0)
            {
                var voucherNo = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == transaction.UserId) && a.VoucherType == "Contra").Max(x => x.VoucherNo);
                transaction.VoucherNo = voucherNo + 1;
            }
            else
            {
                transaction.VoucherNo = 1000;
            }
            // transaction.TrasactionalAmount = 
            transaction.VoucherType = "Contra";

            db.Transaction.Add(transaction);
            var transactionid = transaction.Id;
            var voucherno = transaction.VoucherNo;
            var orderid = transaction.Id;
            var comid = db.Branch.SingleOrDefault(s => s.Id == transaction.BranchID);
            foreach (var item in transactiondata)
            {
                var orderdetalsId = Guid.NewGuid();
                TransactionDetails T = new TransactionDetails();
                T.Id = Guid.NewGuid();
                T.VoucherNo = voucherno;
                T.VoucherType = transaction.VoucherType;
                T.LedgerName = item.LedgerName;
                T.LedgerNo = item.LedgerNo;
                T.DebitAmount = item.DebitAmount;
                T.CreditLedger = item.CreditLedger;
                T.Narration = transaction.Narration;
                T.DabitLedger = item.DabitLedger;
                T.CreditAmount = item.CreditAmount;
                T.TransactionDate = transaction.TransactionDate;
                T.TransactionID = transactionid;
                T.BranchID = transaction.BranchID;
                T.LedgerID = item.LedgerID;

                db.TransactionDetails.Add(T);

            }
            db.SaveChanges();


            result = "Success! Order Is Complete!";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getLedger(Guid id)
        {

            List<Ledger> ledger = new List<Ledger>();
            ledger = db.Ledger.Where(x => x.Status == true).Where(x => x.Id == id).ToList();

            var ToReturn = ledger.Select(S => new
            {
                LedgerId = S.Id,
                LedgerCode = S.LedgerCode,
                LedgerName = S.Name,
            });
            return Json(ToReturn, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, acc_index")]
        public ActionResult Index()
        {
            List<JournalReportVM> allData = new List<JournalReportVM>();

            // here MyDatabaseEntities is our data context
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var o = db.Transaction.Where(a => a.Branch.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).OrderByDescending(a => a.CreatedOn)
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
        [HttpGet]
        public ActionResult chartTree()
        {
            return View();
        }
        public ActionResult JsTreeDemo()
        {
            return View();
        }
        public ActionResult Nodes()
        {
            var username = User.Identity.GetUserId();
            db.Configuration.ProxyCreationEnabled = false;
            var jsonData = new List<ChartTree>();
            jsonData = db.ChartTree.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            var nodes = jsonData.Select(S => new
            {
                id = S.id,
                parent = S.parent,
                text = S.text,
                type = S.type,
                isLedger = S.isLedger,
            });
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, acc_ledgerlist")]
        public ActionResult LedgerList()
        {
            return View();
        }
        public ActionResult LedgerListData()
        {
            var all = db.TransactionDetails.OrderBy(a => a.CreatedOn).ToList();
            var data = all.OrderBy(x=>x.LedgerNo).GroupBy(x => x.LedgerNo).Select(s => new
            {
                Branchces= s.FirstOrDefault().Branch.BranchName,
                LedgerNo = s.FirstOrDefault().LedgerNo,
                LedgerName = s.FirstOrDefault().LedgerName,
                DebitAmount = s.Sum(x=>x.DebitAmount),
                CreditAmount = s.Sum(x => x.CreditAmount),
                ClosinngBalance = (s.Sum(x => x.DebitAmount) - s.Sum(x => x.CreditAmount)),
                ledgerID = s.FirstOrDefault().LedgerID
            }).ToList();
             return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "companyAdmin, acc_ledgerdetails")]
        public ActionResult Ledgerdetails(Guid? id, string DateformDate, string DatetoDatae)
        {
            var username = User.Identity.GetUserId();

            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s =>
            new {
                Text = s.LedgerCategory.ChartOfAccount.Name + " -> "
            + s.LedgerCategory.Name + " -> " + s.LedgerCode + "-" + s.Name,
                value = s.Id
            });

            ViewBag.LedgerList = new SelectList(ledger, "value", "Text");

            var comName = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => s.CompanyName).SingleOrDefault();

            ViewBag.ComName = comName;
            ViewBag.lName = db.Ledger.Where(x => x.Id == id).Select(x => x.Name).SingleOrDefault();
            ViewBag.formDate = DateformDate;
            ViewBag.toDatae = DatetoDatae;
            string date = DateformDate + "To" + DatetoDatae;
            ViewBag.LID = id;
            return View();
        }
        public JsonResult LedgerdetailsData(Guid? id, string DateformDate, string DatetoDatae)
        {

            DateTime formDate = Convert.ToDateTime(DateformDate);
            DateTime toDatae = Convert.ToDateTime(DatetoDatae);

            var lID = id.ToString();
            var all = db.TransactionDetails;
            Guid[] data = all.OrderBy(x => x.LedgerNo).Where(x => x.LedgerID.ToString() == lID).GroupBy(x => x.TransactionID).Select(s =>s.FirstOrDefault().TransactionID).ToArray();
            var data2 = from x in all where data.Contains(x.TransactionID)
                        select new
                        {
                            Date = x.TransactionDate,
                            Name = x.LedgerName,
                            Vouchertype = x.VoucherType,
                            Voucher = x.VoucherType +" "+ x.VoucherNo,
                            Debit = x.CreditAmount,
                            Cradit = x.DebitAmount,
                            Narration = x.Narration,
                            id = x.LedgerID
                        };
            var data3 = data2.Where(x =>x.id.ToString() != lID)
                .Where(x=>x.Date >= formDate).Where(x => x.Date <= toDatae)
                .ToList();
            return Json(new { data = data3 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLedgerName(Guid id)
        {
            List<Ledger> ledger = new List<Ledger>();
            ledger = db.Ledger.Where(x => x.Id == id).ToList();

            var ToReturn = ledger.Select(S => new
            {
                LedgerId = S.Id,
                LedgerName = S.Name,
            });
            return Json(ToReturn, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CartList()
        {
            var username = User.Identity.GetUserId();
            var branch = db.ChartOfAccount
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).Include(x => x.LedgerCategory)
                .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            return View();
        }


        // GET: Ledgers/Create
        [Authorize(Roles = "companyAdmin, acc_createledger")]
        public ActionResult CreateLedger()
        {
            var username = User.Identity.GetUserId();
            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            var ledger = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s =>
            new {
                Text = s.LedgerCategory.ChartOfAccount.Name + " -> "
            + s.LedgerCategory.Name + " -> " + s.LedgerCode + "-" + s.Name,
                value = s.Id
            });
            ViewBag.ParentLedgerId = new SelectList(ledger, "value", "Text");

            var ledgercat = db.LedgerCategory.Where(a => a.ChartOfAccount.Company
            .ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s =>
           new {
               Text = s.ChartOfAccount.Name + " -> " + "-" + s.Name,
               value = s.Id
           });

            ViewBag.LedgerCategoryId = new SelectList(ledgercat, "value", "Text");
            return View();
        }

        // POST: Ledgers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLedger(Ledger ledger)
        {

            var username = User.Identity.GetUserId();

            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName", ledger.CompanyId);
            ViewBag.ParentLedgerId = new SelectList(db.Ledger, "Id", "Name", ledger.ParentLedgerId);
            ViewBag.LedgerCategoryId = new SelectList(db.LedgerCategory, "Id", "Name", ledger.LedgerCategoryId);

            if (ModelState.IsValid)
            {
                
                var isExist = IsDataExistt(ledger.Name);
                if (!isExist)
                {
                    var ledgeRrowcount =
                    db.Ledger.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));

                if (ledgeRrowcount > 0)
                {
                    var ledgerCode =
                        db.Ledger.Where(
                            a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                            .Max(x => x.LedgerCode);
                    ledger.LedgerCode = ledgerCode + 1;
                }
                else
                {
                    ledger.LedgerCode = 1000;
                }

                ledger.Id = Guid.NewGuid();
                db.Ledger.Add(ledger);

                var ledgerid = ledger.Id;
                var openingbalance = ledger.OpeningBalance;

                if (openingbalance != null || openingbalance != 0)
                {
                    Transaction tr = new Transaction();
                    tr.Id = Guid.NewGuid();
                    tr.UserId = username;
                    tr.BranchID =
                        db.Branch.Where(x => x.CompanyId == ledger.CompanyId)
                            .OrderBy(o => o.CreatedOn)
                            .Select(s => s.Id)
                            .ToList()
                            .FirstOrDefault();
                    tr.VoucherNo = 1000;
                    tr.VoucherType = "Opening Balance";
                    tr.TrasactionalAmount = Convert.ToDecimal(openingbalance);
                    tr.TransactionDate = DateTime.Now;

                    db.Transaction.Add(tr);


                    TransactionDetails td = new TransactionDetails();

                    var balance = openingbalance.ToString().ToCharArray();
                    if (balance[0] == '-')
                    {
                        td.CreditAmount = openingbalance;
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
                    td.LedgerName = ledger.Name;
                    td.LedgerNo = ledger.LedgerCode.ToString();
                    td.LedgerID = ledgerid;

                    db.TransactionDetails.Add(td);
                }



                ledgerid = ledger.Id;
                var catid = ledger.LedgerCategoryId;
                var lname = ledger.Name;
                var comid = ledger.CompanyId;
                ChartTree ctree = new ChartTree();
                ctree.id = ledgerid.ToString();
                ctree.parent = catid.ToString();
                ctree.text = lname;
                ctree.isLedger = true;
                ctree.type = "ledger";
                ctree.CompanyId = comid;

                db.ChartTree.Add(ctree);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            }

            return View(ledger);
        }
        [NonAction]
        public bool IsDataExistt(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.Ledger.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                        .ToList().Where(a => a.Name == data).FirstOrDefault();
                return v != null;
            }
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