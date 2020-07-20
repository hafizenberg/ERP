using ChamdrimERP.Models;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace ChamdrimERP.Controllers
{
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Report
        public ActionResult Index()
        {
            var username = User.Identity.GetUserId();
            var name = db.Company.Where(x => x.ApplicationUser_Company.Any(a => a.ApplicationUser_Id == username)).ToList();
            ViewBag.CompanyName = new SelectList(name, "Id", "CompanyName");
            return View();
        }
        public ActionResult ProfitAndLoss(Guid id,string dateformDate, string datetoDatae)
        {
            DateTime formDate = Convert.ToDateTime(dateformDate);
            DateTime toDatae = Convert.ToDateTime(datetoDatae);
            var cid_income = db.ChartOfAccount.Where(x => x.CompanyId == id && x.Name == "Income").Select(s => s.Id).SingleOrDefault();
            var cid_expense = db.ChartOfAccount.Where(x => x.CompanyId == id && x.Name == "Expenses").Select(s => s.Id).SingleOrDefault();

            ViewBag.CompanyName = db.Company.Where(x => x.Id == id).Select(s => s.CompanyName).SingleOrDefault();
            ViewBag.DateRange = " For the period from " + formDate.ToString("dd-MMM-yyyy") + " to " + toDatae.ToString("dd-MMM-yyyy");

            var Income = db.LedgerCategory.Where(x => x.ChartOfAccountId == cid_income).Select( p =>  new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = db.TransactionDetails.Where(a=>a.TransactionDate >= formDate && a.TransactionDate <= toDatae).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.DebitAmount)).FirstOrDefault(),
                CrBalance = db.TransactionDetails.Where(a => a.TransactionDate >= formDate && a.TransactionDate <= toDatae).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.CreditAmount)).FirstOrDefault(),
            }).ToList();
            ViewBag.Income = Income;
            ViewBag.IncomeSum = Income.Sum(s=>s.CrBalance)- Income.Sum(s => s.DrBalance);
            var Expenses = db.LedgerCategory.Where(x => x.ChartOfAccountId == cid_expense).Select(p => new ProfitAndLoss()
            {
                Name = p.Name,
                DrBalance = db.TransactionDetails.Where(a => a.TransactionDate >= formDate && a.TransactionDate <= toDatae).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.DebitAmount)).FirstOrDefault(),
                CrBalance = db.TransactionDetails.Where(a => a.TransactionDate >= formDate && a.TransactionDate <= toDatae).Where(x => x.Ledger.LedgerCategoryId == p.Id).GroupBy(x => x.Ledger.LedgerCategory.Id).Select(q => q.Sum(x => x.CreditAmount)).FirstOrDefault(),
            }).ToList();
            ViewBag.Expenses = Expenses;
            ViewBag.ExpensesSum = Expenses.Sum(s => s.DrBalance) - Expenses.Sum(s => s.CrBalance);
            return View();
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