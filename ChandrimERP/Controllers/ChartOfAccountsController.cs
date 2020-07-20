using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class ChartOfAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ChartOfAccounts
        public ActionResult Index()
        {
            return View(db.ChartOfAccount.ToList());
        }

        // GET: ChartOfAccounts/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChartOfAccount ChartOfAccount = db.ChartOfAccount.Find(id);
            if (ChartOfAccount == null)
            {
                return HttpNotFound();
            }
            return View(ChartOfAccount);
        }

        // GET: ChartOfAccounts/Create
        [Authorize(Roles = "companyAdmin, cha_o_a_create")]
        public ActionResult Create()
        {
            var username = User.Identity.GetUserId();
            var chartOfAccountList = db.ChartOfAccount
                .Where(a => a.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.List = new SelectList(chartOfAccountList, "Id", "Name");
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();

            var product = db.Products.Select(s => new { Text = s.ProductCode + "-" + s.ProductName, value = s.ProductId });
            ViewBag.ProductList = new SelectList(product, "value", "Text");

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            return View();
        }

        // POST: ChartOfAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChartOfAccount chartOfAccount)
        {
            var username = User.Identity.GetUserId();
            var coac = db.ChartOfAccount.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.List = new SelectList(coac, "Id", "Name", chartOfAccount.ParentNode);
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", chartOfAccount.CompanyId);
            if (ModelState.IsValid)
            {
                var isExist = IsDataExistt(chartOfAccount.Name);
                if (isExist)
                {
                    chartOfAccount.Id = Guid.NewGuid();
                    db.ChartOfAccount.Add(chartOfAccount);

                    var ledgerid = chartOfAccount.Id;
                    var catid = chartOfAccount.CompanyId;
                    var lname = chartOfAccount.Name;
                    var comid = chartOfAccount.Company.Id;
                    ChartTree ctree = new ChartTree();
                    ctree.id = ledgerid.ToString();
                    ctree.parent = catid.ToString();
                    ctree.text = lname;
                    ctree.type = "chart";
                    ctree.CompanyId = comid;
                    db.ChartTree.Add(ctree);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(chartOfAccount);
        }
        [NonAction]
        public bool IsDataExistt(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.ChartOfAccount
                        .Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.Name == data);
                return v != null;
            }
        }
        // GET: ChartOfAccounts/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChartOfAccount ChartOfAccount = db.ChartOfAccount.Find(id);
            if (ChartOfAccount == null)
            {
                return HttpNotFound();
            }
            return View(ChartOfAccount);
        }

        // POST: ChartOfAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ParentNode,CreatedOn")] ChartOfAccount ChartOfAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ChartOfAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ChartOfAccount);
        }

        // GET: ChartOfAccounts/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChartOfAccount ChartOfAccount = db.ChartOfAccount.Find(id);
            if (ChartOfAccount == null)
            {
                return HttpNotFound();
            }
            return View(ChartOfAccount);
        }

        // POST: ChartOfAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ChartOfAccount ChartOfAccount = db.ChartOfAccount.Find(id);
            db.ChartOfAccount.Remove(ChartOfAccount);
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
