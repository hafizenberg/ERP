using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class BusinessController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "companyAdmin")]
        public ActionResult Index()
        {
            List<CompanyBranchWarehouse> allBranch = new List<CompanyBranchWarehouse>();

            // here MyDatabaseEntities is our data context
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var o = db.Company.Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username)).OrderByDescending(a => a.Id);
                foreach (var i in o)
                {
                    var branch = db.Branch.Where(a => a.CompanyId.Equals(i.Id)).ToList();
                    var warehouse = db.Warehouse.Where(a => a.CompanyId.Equals(i.Id)).ToList();
                    allBranch.Add(new CompanyBranchWarehouse { company = i, warehouse = warehouse, branch = branch });
                }
            }
            return View(allBranch.AsEnumerable());
        }
        [Authorize(Roles = "companyAdmin")]
        public ActionResult CompanyList()
        {
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            return View(companyList);
        }
        public ActionResult GetCompanyList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Company>();
                jsonData = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }

        }



        // GET: Business
        [Authorize(Roles = "systemAdmin")]
        public ActionResult AddNewBusiness()
        {
            AddBussinessVM model = new AddBussinessVM();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var users = roleManager.FindByName("companyAdmin").Users.Select(s => s.RoleId).First().ToString();
            var userlist = db.Users.Where(x => x.Roles.Any(v => v.RoleId == users)).ToList();
            ViewBag.Userlist = new SelectList(userlist, "Id", "Email");
            return View(new AddBussinessVM());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewBusiness(AddBussinessVM model)
        {
            if (ModelState.IsValid)
            {

                if (model.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageUpload.FileName);
                    string extension = Path.GetExtension(model.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.CompanyLogo = "~/Image/company_logo/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/company_logo/"), fileName);
                    model.ImageUpload.SaveAs(fileName);
                }


                Company com = new Company();
                com.Id = Guid.NewGuid();
                com.CompanyName = model.CompanyName;
                com.ContactFirstName = model.ContactFirstName;
                com.ContactLastName = model.ContactLastName;
                com.Genders = model.Genders;
                com.Phone = model.Phone;
                com.Email = model.Email;
                com.WebPage = model.WebPage;
                com.VatInformation = model.VatInformation;
                com.FinancialYearStart = model.FinancialYearStart;
                com.BusinessType = model.BusinessType;
                com.CompanyLogo = model.CompanyLogo;
                com.Country = model.Country;
                com.State = model.State;
                com.City = model.City;
                com.AddressLineOne = model.AddressLineOne;
                com.AddressLineTwo = model.AddressLineTwo;
                com.Status = true;
                com.Notes = model.Notes;

                db.Company.Add(com);
                //db.SaveChanges();

                var companyId = com.Id;
                var companyName = com.CompanyName;

                Branch branch = new Branch();
                branch.Id = Guid.NewGuid();
                branch.BranchName = model.BranchName;
                branch.CompanyId = companyId;
                branch.Status = true;

                db.Branch.Add(branch);
                // db.SaveChanges();


                Warehouse warehouse = new Warehouse();
                warehouse.Id = Guid.NewGuid();
                warehouse.WarehouseName = model.WarehouseName;
                warehouse.CompanyId = companyId;
                warehouse.Status = true;

                db.Warehouse.Add(warehouse);
                // db.SaveChanges();

                var Branch_Id = branch.Id;

                ApplicationUser_Company acom = new ApplicationUser_Company();
                acom.ApplicationUser_Id = model.UserId;
                acom.Company_Id = companyId;

                db.ApplicationUser_Company.Add(acom);
                //db.SaveChanges();

                ApplicationUser_Branch abranch = new ApplicationUser_Branch();
                abranch.ApplicationUser_Id = model.UserId;
                abranch.Branch_Id = Branch_Id;

                db.ApplicationUser_Branch.Add(abranch);

                List<ChartOfAccount> ch =
                    new List<ChartOfAccount>()
                    {
                        new ChartOfAccount() {  Name = "Assets",CompanyId = companyId },
                        new ChartOfAccount() {  Name = "Liabilities",CompanyId = companyId },
                        new ChartOfAccount() {  Name = "Income",CompanyId = companyId },
                        new ChartOfAccount() {  Name = "Capital",CompanyId = companyId },
                        new ChartOfAccount() {  Name = "Expenses",CompanyId = companyId }
                    };
                foreach (var item1 in ch)
                {
                    item1.isDefault = true;
                    item1.Id = Guid.NewGuid();
                    db.ChartOfAccount.Add(item1);

                }

                var lcatId = ch.Where(x => x.Name == "Assets").Select(s => s.Id).SingleOrDefault();
                var lcatId2 = ch.Where(x => x.Name == "Liabilities").Select(s => s.Id).SingleOrDefault();
                var lcatId3 = ch.Where(x => x.Name == "Income").Select(s => s.Id).SingleOrDefault();
                var lcatId4 = ch.Where(x => x.Name == "Capital").Select(s => s.Id).SingleOrDefault();
                var lcatId5 = ch.Where(x => x.Name == "Expenses").Select(s => s.Id).SingleOrDefault();

                List<LedgerCategory> lcat1 = new List<LedgerCategory>()
                {
                    new LedgerCategory() { Name = "Purchase Accounts", ChartOfAccountId = lcatId5 },
                    new LedgerCategory() { Name = "Current Liabilities", ChartOfAccountId = lcatId2 },
                    new LedgerCategory() { Name = "Current Assets", ChartOfAccountId = lcatId },
                    new LedgerCategory() {  Name = "Indirect Expense",ChartOfAccountId = lcatId5 },
                    new LedgerCategory() {  Name = "Fixed Assets",ChartOfAccountId = lcatId }
                };

                foreach (var item0 in lcat1)
                {
                    item0.isDefault = true;
                    item0.Id = Guid.NewGuid();
               
                    db.LedgerCategory.Add(item0);
                }


                var lgCat1 = lcat1.Where(x => x.Name == "Purchase Accounts").Select(s => s.Id).SingleOrDefault();
                var lgCat2 = lcat1.Where(x => x.Name == "Current Liabilities").Select(s => s.Id).SingleOrDefault();
                var lgCat3 = lcat1.Where(x => x.Name == "Current Assets").Select(s => s.Id).SingleOrDefault();
                var lgCat4 = lcat1.Where(x => x.Name == "Indirect Expense").Select(s => s.Id).SingleOrDefault();
                var lgCat5 = lcat1.Where(x => x.Name == "Fixed Assets").Select(s => s.Id).SingleOrDefault();

                List<LedgerCategory> lcat = new List<LedgerCategory>()
                {

                    new LedgerCategory() {  Name = "Loan Accounts",ChartOfAccountId = lcatId2 },
                    new LedgerCategory() {  Name = "Sales Representative",ChartOfAccountId = lcatId2 },
                    new LedgerCategory() {  Name = "Profit & Loss Accounts",ChartOfAccountId = lcatId2 },


                    new LedgerCategory() {  Name = "Paid Up Capital",ChartOfAccountId = lcatId4 },
                    new LedgerCategory() {  Name = "Owner's Equity",ChartOfAccountId = lcatId4 },

                    new LedgerCategory() {  Name = "Sales Accounts",ChartOfAccountId = lcatId3 },
                    new LedgerCategory() {  Name = "Indirect Income",ChartOfAccountId = lcatId3 },

                    new LedgerCategory() {  Name = "Direct Expense",ChartOfAccountId = lcatId5 },

                    new LedgerCategory() {  Name = "Building & Others",ChartOfAccountId = lcatId   ,ParentLedgerCatId = lgCat5},
                    new LedgerCategory() {  Name = "Elictrical Equipment", ChartOfAccountId = lcatId ,ParentLedgerCatId = lgCat5},
                    new LedgerCategory() {  Name = "Furniture & Equipment", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat5},
                    new LedgerCategory() {  Name = "Plant & Machinary", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat5},

                    new LedgerCategory() {  Name = "Cash in Hand", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},
                    new LedgerCategory() {  Name = "Bank Accounts", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},
                    new LedgerCategory() {  Name = "Loan, advance and deposit", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},
                    new LedgerCategory() {  Name = "Sundry Debtors", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},
                    new LedgerCategory() {  Name = "Advance A/C", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},
                    new LedgerCategory() {  Name = "Accounts Recieveable", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},
                    new LedgerCategory() {  Name = "Accounts Recieveable (Customer)", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},
                    new LedgerCategory() {  Name = "Inventory", ChartOfAccountId = lcatId  ,ParentLedgerCatId = lgCat3},

                    new LedgerCategory() {  Name = "Sundry Creditors", ChartOfAccountId = lcatId2  ,ParentLedgerCatId = lgCat2},
                    new LedgerCategory() {  Name = "Accounts Payable", ChartOfAccountId = lcatId2  ,ParentLedgerCatId = lgCat2},
                    new LedgerCategory() {  Name = "Accounts Payable (Supplier)", ChartOfAccountId = lcatId2  ,ParentLedgerCatId = lgCat2},

                    new LedgerCategory() {  Name = "Conveyance", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat4},
                    new LedgerCategory() {  Name = "Entertainment", ChartOfAccountId = lcatId5  ,ParentLedgerCatId = lgCat4},
                    new LedgerCategory() {  Name = "Others Exp.", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat4},
                    new LedgerCategory() {  Name = "Discount", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat4 },
                    new LedgerCategory() {  Name = "Staff Salary", ChartOfAccountId = lcatId5  ,ParentLedgerCatId = lgCat4},
                    new LedgerCategory() {  Name = "Telephone Bill", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat4 },

                    new LedgerCategory() {  Name = "Direct Raw Materials Purchase", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat1 },
                    new LedgerCategory() {  Name = "Indirect Raw Material Purchase", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat1 },
                    new LedgerCategory() {  Name = "Finished Goods Purchase", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat1 },
                    new LedgerCategory() {  Name = "Purchase Others Expense", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat1},



                };

                foreach (var item2 in lcat)
                {
                    item2.isDefault = true;
                    item2.Id = Guid.NewGuid();
                    db.LedgerCategory.Add(item2);
                }


                var lgCat6 = lcat.Where(x => x.Name == "Direct Raw Materials Purchase").Select(s => s.Id).SingleOrDefault();
                var lgCat7 = lcat.Where(x => x.Name == "Indirect Raw Material Purchase").Select(s => s.Id).SingleOrDefault();
                var lgCat8 = lcat.Where(x => x.Name == "Finished Goods Purchase").Select(s => s.Id).SingleOrDefault();

                List<LedgerCategory> lcat3 = new List<LedgerCategory>()
                {
                    new LedgerCategory() {Name = "Direct Raw Materials", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat6},
                    new LedgerCategory() {Name = "Indirect Raw Material", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat7},
                    new LedgerCategory() {Name = "Work in Progress", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat6},
                    new LedgerCategory() {Name = "Finished Goods", ChartOfAccountId = lcatId5 ,ParentLedgerCatId = lgCat8}
                };

                foreach (var item2 in lcat3)
                {
                    item2.isDefault = true;
                    item2.Id = Guid.NewGuid();
                    db.LedgerCategory.Add(item2);
                }

                var lgId = lcat.Where(x => x.Name == "Cash in Hand").Select(s => s.Id).SingleOrDefault();
                var lgId2 = lcat.Where(x => x.Name == "Loan, advance and deposit").Select(s => s.Id).SingleOrDefault();
                var lgId3 = lcat.Where(x => x.Name == "Sales Accounts").Select(s => s.Id).SingleOrDefault();
                var lgId4 = lcat3.Where(x => x.Name == "Finished Goods").Select(s => s.Id).SingleOrDefault();
                var lgId5 = lcat.Where(x => x.Name == "Accounts Recieveable").Select(s => s.Id).SingleOrDefault();
                var lgId6 = lcat.Where(x => x.Name == "Advance A/C").Select(s => s.Id).SingleOrDefault();
                var lgId7 = lcat.Where(x => x.Name == "Inventory").Select(s => s.Id).SingleOrDefault();
                var lgId8 = lcat.Where(x => x.Name == "Building & Others").Select(s => s.Id).SingleOrDefault();
                var lgId9 = lcat.Where(x => x.Name == "Elictrical Equipment").Select(s => s.Id).SingleOrDefault();
                var lgId10 = lcat.Where(x => x.Name == "Furniture & Equipment").Select(s => s.Id).SingleOrDefault();
                var lgId11 = lcat.Where(x => x.Name == "Plant & Machinary").Select(s => s.Id).SingleOrDefault();
                var lgId12 = lcat1.Where(x => x.Name == "Fixed Assets").Select(s => s.Id).SingleOrDefault();
                var lgId13 = lcat.Where(x => x.Name == "Accounts Payable").Select(s => s.Id).SingleOrDefault();
                var lgId14 = lcat1.Where(x => x.Name == "Current Liabilities").Select(s => s.Id).SingleOrDefault();
                var lgId15 = lcat.Where(x => x.Name == "Indirect Income").Select(s => s.Id).SingleOrDefault();
                var lgId16 = lcat1.Where(x => x.Name == "Indirect Expense").Select(s => s.Id).SingleOrDefault();
                var lgId17 = lcat.Where(x => x.Name == "Owner's Equity").Select(s => s.Id).SingleOrDefault();

                var lgId18 = lcat.Where(x => x.Name == "Conveyance").Select(s => s.Id).SingleOrDefault();
                var lgId19 = lcat.Where(x => x.Name == "Entertainment").Select(s => s.Id).SingleOrDefault();
                var lgId20 = lcat.Where(x => x.Name == "Discount").Select(s => s.Id).SingleOrDefault();
                var lgId21 = lcat.Where(x => x.Name == "Staff Salary").Select(s => s.Id).SingleOrDefault();
                var lgId22 = lcat.Where(x => x.Name == "Telephone Bill").Select(s => s.Id).SingleOrDefault();
                var lgId23 = lcat.Where(x => x.Name == "Others Exp.").Select(s => s.Id).SingleOrDefault();

                var lgId24 = lcat.Where(x => x.Name == "Purchase Others Expense").Select(s => s.Id).SingleOrDefault();

                List<Ledger> lg = new List<Ledger>()
                {
                    new Ledger() {  Name = "Cash A/C",LedgerCategoryId = lgId ,LedgerCode = 1000 },
                    new Ledger() {  Name = "Petty Cash", LedgerCategoryId = lgId ,  LedgerCode = 1001},

                    new Ledger() {  Name = "Advance Against Expenses", LedgerCategoryId = lgId2  ,LedgerCode = 1004},
                    new Ledger() {  Name = "Advance Against Salary", LedgerCategoryId = lgId2 ,LedgerCode = 1005 },
                    new Ledger() {  Name = "Advance Tax", LedgerCategoryId = lgId2 , LedgerCode = 1032 },

                    new Ledger() {  Name = "Sales", LedgerCategoryId = lgId3 ,LedgerCode = 1002 },
                    new Ledger() {  Name = "Sales to Customers (Cash)", LedgerCategoryId = lgId3 , LedgerCode = 1037 },


                    new Ledger() {  Name = "Purchase", LedgerCategoryId = lgId4 , LedgerCode = 1003 },
                    new Ledger() {  Name = "Cost Of Goods Purchase", LedgerCategoryId = lgId24, LedgerCode = 1039},

                    new Ledger() {  Name = "Accounts Recieveable", LedgerCategoryId = lgId5 , LedgerCode = 1006},
                    new Ledger() {  Name = "Prepaid Expenses", LedgerCategoryId = lgId6 , LedgerCode = 1007},
                    new Ledger() {  Name = "Closing Inventory", LedgerCategoryId = lgId7 , LedgerCode = 1008},
                    new Ledger() {  Name = "Building & Others", LedgerCategoryId = lgId8 , LedgerCode = 1009},
                    new Ledger() {  Name = "Elictrical Equipment", LedgerCategoryId = lgId9 , LedgerCode = 1010},
                    new Ledger() {  Name = "Furniture & Equipment", LedgerCategoryId = lgId10 , LedgerCode = 1011},
                    new Ledger() {  Name = "Plant & Machinary", LedgerCategoryId = lgId11 , LedgerCode = 1012},
                    new Ledger() {  Name = "Accumulated Depreciation", LedgerCategoryId = lgId12 , LedgerCode = 1013},

                    new Ledger() {  Name = "Account Payable", LedgerCategoryId = lgId13 , LedgerCode = 1014},
                    new Ledger() {  Name = "Payroll Payable", LedgerCategoryId = lgId14 , LedgerCode = 1015},
                    new Ledger() {  Name = "Texes Payable", LedgerCategoryId = lgId14 , LedgerCode = 1016},

                    new Ledger() {  Name = "Additional Income", LedgerCategoryId = lgId15 , LedgerCode = 1017},
                    new Ledger() {  Name = "Sales Return Allowances", LedgerCategoryId = lgId15 , LedgerCode = 1018},

                    new Ledger() {  Name = "Cost Of Goods Sold", LedgerCategoryId = lgId16 , LedgerCode = 1019},
                    new Ledger() {  Name = "Advertising Expense", LedgerCategoryId = lgId16 , LedgerCode = 1020},
                    new Ledger() {  Name = "Bank Fees", LedgerCategoryId = lgId16 , LedgerCode = 1021},
                    new Ledger() {  Name = "Depreciation Expense", LedgerCategoryId = lgId16 , LedgerCode = 1022},
                    new Ledger() {  Name = "Payroll Tex Expense", LedgerCategoryId = lgId16 , LedgerCode = 1023},
                    new Ledger() {  Name = "Rent Expense", LedgerCategoryId = lgId16 , LedgerCode = 1024},
                    new Ledger() {  Name = "Supplies Expense", LedgerCategoryId = lgId23 , LedgerCode = 1025},
                    new Ledger() {  Name = "Utilities Expense", LedgerCategoryId = lgId23 , LedgerCode = 1026},
                    new Ledger() {  Name = "Wages Expense", LedgerCategoryId = lgId21 , LedgerCode = 1027},
                    new Ledger() {  Name = "Conveyance", LedgerCategoryId = lgId18 ,  LedgerCode = 1028},
                    new Ledger() {  Name = "Entertainment", LedgerCategoryId = lgId19 , LedgerCode = 1029},
                    new Ledger() {  Name = "Discount", LedgerCategoryId = lgId20 , LedgerCode = 1030},
                    new Ledger() {  Name = "Trade Discount", LedgerCategoryId = lgId20 ,  LedgerCode = 1038},
                    new Ledger() {  Name = "Telephone Bill", LedgerCategoryId = lgId22 ,  LedgerCode = 1031},
                    new Ledger() {  Name = "IT and Internet Expenses", LedgerCategoryId = lgId16 ,  LedgerCode = 1033},

                    new Ledger() {  Name = "Owner's Equity", LedgerCategoryId = lgId17 ,  LedgerCode = 1035},
                    new Ledger() {  Name = "Drawings", LedgerCategoryId = lgId17 ,  LedgerCode = 1036},
                };

                foreach (var item3 in lg)
                {
                    item3.CompanyId = companyId;
                    item3.isDefault = true;
                    item3.Status = true;
                    item3.Id = Guid.NewGuid();
                    db.Ledger.Add(item3);
                }

                await db.SaveChangesAsync();


                var companyidrood = com.Id;
                var treerood = com.Id.ToString();
                var tree1 = ch.Where(x => x.Name == "Assets").Select(s => s.Id).SingleOrDefault().ToString();
                var tree2 = ch.Where(x => x.Name == "Liabilities").Select(s => s.Id).SingleOrDefault().ToString();
                var tree3 = ch.Where(x => x.Name == "Income").Select(s => s.Id).SingleOrDefault().ToString();
                var tree4 = ch.Where(x => x.Name == "Capital").Select(s => s.Id).SingleOrDefault().ToString();
                var tree5 = ch.Where(x => x.Name == "Expenses").Select(s => s.Id).SingleOrDefault().ToString();

                var tree6 = lcat1.Where(x => x.Name == "Fixed Assets").Select(s => s.Id).SingleOrDefault().ToString();
                var tree7 = lcat1.Where(x => x.Name == "Current Assets").Select(s => s.Id).SingleOrDefault().ToString();

                var tree8 = lcat.Where(x => x.Name == "Loan Accounts").Select(s => s.Id).SingleOrDefault().ToString();
                var tree9 = lcat.Where(x => x.Name == "Sales Representative").Select(s => s.Id).SingleOrDefault().ToString();
                var tree10 = lcat.Where(x => x.Name == "Profit & Loss Accounts").Select(s => s.Id).SingleOrDefault().ToString();
                var tree11 = lcat1.Where(x => x.Name == "Current Liabilities").Select(s => s.Id).SingleOrDefault().ToString();

                var tree12 = lcat.Where(x => x.Name == "Paid Up Capital").Select(s => s.Id).SingleOrDefault().ToString();
                var tree13 = lcat.Where(x => x.Name == "Owner's Equity").Select(s => s.Id).SingleOrDefault().ToString();

                var tree14 = lcat.Where(x => x.Name == "Sales Accounts").Select(s => s.Id).SingleOrDefault().ToString();
                var tree15 = lcat.Where(x => x.Name == "Indirect Income").Select(s => s.Id).SingleOrDefault().ToString();

                var tree16 = lcat.Where(x => x.Name == "Direct Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree17 = lcat1.Where(x => x.Name == "Indirect Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree18 = lcat1.Where(x => x.Name == "Purchase Accounts").Select(s => s.Id).SingleOrDefault().ToString();
                var tree85 = lcat.Where(x => x.Name == "Purchase Others Expense").Select(s => s.Id).SingleOrDefault().ToString();

                var tree19 = lcat.Where(x => x.Name == "Building & Others").Select(s => s.Id).SingleOrDefault().ToString();
                var tree20 = lcat.Where(x => x.Name == "Elictrical Equipment").Select(s => s.Id).SingleOrDefault().ToString();
                var tree21 = lcat.Where(x => x.Name == "Furniture & Equipment").Select(s => s.Id).SingleOrDefault().ToString();
                var tree22 = lcat.Where(x => x.Name == "Plant & Machinary").Select(s => s.Id).SingleOrDefault().ToString();



                var tree24 = lcat.Where(x => x.Name == "Cash in Hand").Select(s => s.Id).SingleOrDefault().ToString();
                var tree25 = lcat.Where(x => x.Name == "Bank Accounts").Select(s => s.Id).SingleOrDefault().ToString();
                var tree26 = lcat.Where(x => x.Name == "Loan, advance and deposit").Select(s => s.Id).SingleOrDefault().ToString();
                var tree27 = lcat.Where(x => x.Name == "Sundry Debtors").Select(s => s.Id).SingleOrDefault().ToString();
                var tree28 = lcat.Where(x => x.Name == "Advance A/C").Select(s => s.Id).SingleOrDefault().ToString();
                var tree29 = lcat.Where(x => x.Name == "Accounts Recieveable").Select(s => s.Id).SingleOrDefault().ToString();
                var tree33 = lcat.Where(x => x.Name == "Accounts Recieveable (Customer)").Select(s => s.Id).SingleOrDefault().ToString();
                var tree30 = lcat.Where(x => x.Name == "Inventory").Select(s => s.Id).SingleOrDefault().ToString();

                var tree31 = lcat.Where(x => x.Name == "Sundry Creditors").Select(s => s.Id).SingleOrDefault().ToString();
                var tree32 = lcat.Where(x => x.Name == "Accounts Payable").Select(s => s.Id).SingleOrDefault().ToString();
                var tree81 = lcat.Where(x => x.Name == "Accounts Payable (Supplier)").Select(s => s.Id).SingleOrDefault().ToString();

                var tree34 = lcat.Where(x => x.Name == "Conveyance").Select(s => s.Id).SingleOrDefault().ToString();
                var tree35 = lcat.Where(x => x.Name == "Entertainment").Select(s => s.Id).SingleOrDefault().ToString();
                var tree36 = lcat.Where(x => x.Name == "Others Exp.").Select(s => s.Id).SingleOrDefault().ToString();
                var tree37 = lcat.Where(x => x.Name == "Discount").Select(s => s.Id).SingleOrDefault().ToString();
                var tree38 = lcat.Where(x => x.Name == "Staff Salary").Select(s => s.Id).SingleOrDefault().ToString();
                var tree39 = lcat.Where(x => x.Name == "Telephone Bill").Select(s => s.Id).SingleOrDefault().ToString();

                var tree40 = lcat.Where(x => x.Name == "Direct Raw Materials Purchase").Select(s => s.Id).SingleOrDefault().ToString();
                var tree41 = lcat.Where(x => x.Name == "Indirect Raw Material Purchase").Select(s => s.Id).SingleOrDefault().ToString();
                var tree42 = lcat.Where(x => x.Name == "Finished Goods Purchase").Select(s => s.Id).SingleOrDefault().ToString();
                var tree43 = lcat3.Where(x => x.Name == "Direct Raw Materials").Select(s => s.Id).SingleOrDefault().ToString();
                var tree44 = lcat3.Where(x => x.Name == "Indirect Raw Material").Select(s => s.Id).SingleOrDefault().ToString();
                var tree45 = lcat3.Where(x => x.Name == "Work in Progress").Select(s => s.Id).SingleOrDefault().ToString();
                var tree46 = lcat3.Where(x => x.Name == "Finished Goods").Select(s => s.Id).SingleOrDefault().ToString();

                //ledger item
                var tree47 = lg.Where(x => x.Name == "Cash A/C").Select(s => s.Id).SingleOrDefault().ToString();
                var tree48 = lg.Where(x => x.Name == "Petty Cash").Select(s => s.Id).SingleOrDefault().ToString();

                var tree49 = lg.Where(x => x.Name == "Advance Against Expenses").Select(s => s.Id).SingleOrDefault().ToString();
                var tree50 = lg.Where(x => x.Name == "Advance Against Salary").Select(s => s.Id).SingleOrDefault().ToString();
                var tree51 = lg.Where(x => x.Name == "Advance Tax").Select(s => s.Id).SingleOrDefault().ToString();

                var tree52 = lg.Where(x => x.Name == "Sales").Select(s => s.Id).SingleOrDefault().ToString();
                var tree53 = lg.Where(x => x.Name == "Sales to Customers (Cash)").Select(s => s.Id).SingleOrDefault().ToString();


                var tree54 = lg.Where(x => x.Name == "Purchase").Select(s => s.Id).SingleOrDefault().ToString();
                var tree86 = lg.Where(x => x.Name == "Cost Of Goods Purchase").Select(s => s.Id).SingleOrDefault().ToString();

                var tree55 = lg.Where(x => x.Name == "Accounts Recieveable").Select(s => s.Id).SingleOrDefault().ToString();
                var tree56 = lg.Where(x => x.Name == "Prepaid Expenses").Select(s => s.Id).SingleOrDefault().ToString();
                var tree57 = lg.Where(x => x.Name == "Closing Inventory").Select(s => s.Id).SingleOrDefault().ToString();
                var tree58 = lg.Where(x => x.Name == "Building & Others").Select(s => s.Id).SingleOrDefault().ToString();
                var tree59 = lg.Where(x => x.Name == "Elictrical Equipment").Select(s => s.Id).SingleOrDefault().ToString();
                var tree60 = lg.Where(x => x.Name == "Furniture & Equipment").Select(s => s.Id).SingleOrDefault().ToString();
                var tree61 = lg.Where(x => x.Name == "Plant & Machinary").Select(s => s.Id).SingleOrDefault().ToString();
                var tree62 = lg.Where(x => x.Name == "Accumulated Depreciation").Select(s => s.Id).SingleOrDefault().ToString();

                var tree63 = lg.Where(x => x.Name == "Account Payable").Select(s => s.Id).SingleOrDefault().ToString();
                var tree64 = lg.Where(x => x.Name == "Payroll Payable").Select(s => s.Id).SingleOrDefault().ToString();
                var tree65 = lg.Where(x => x.Name == "Texes Payable").Select(s => s.Id).SingleOrDefault().ToString();

                var tree66 = lg.Where(x => x.Name == "Additional Income").Select(s => s.Id).SingleOrDefault().ToString();
                var tree67 = lg.Where(x => x.Name == "Sales Return Allowances").Select(s => s.Id).SingleOrDefault().ToString();

                var tree68 = lg.Where(x => x.Name == "Cost Of Goods Sold").Select(s => s.Id).SingleOrDefault().ToString();
                var tree69 = lg.Where(x => x.Name == "Advertising Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree70 = lg.Where(x => x.Name == "Bank Fees").Select(s => s.Id).SingleOrDefault().ToString();
                var tree71 = lg.Where(x => x.Name == "Depreciation Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree72 = lg.Where(x => x.Name == "Payroll Tex Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree73 = lg.Where(x => x.Name == "Rent Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree74 = lg.Where(x => x.Name == "Supplies Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree75 = lg.Where(x => x.Name == "Utilities Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree76 = lg.Where(x => x.Name == "Wages Expense").Select(s => s.Id).SingleOrDefault().ToString();
                var tree77 = lg.Where(x => x.Name == "Conveyance").Select(s => s.Id).SingleOrDefault().ToString();
                var tree78 = lg.Where(x => x.Name == "Entertainment").Select(s => s.Id).SingleOrDefault().ToString();
                var tree79 = lg.Where(x => x.Name == "Discount").Select(s => s.Id).SingleOrDefault().ToString();
                var tree80 = lg.Where(x => x.Name == "Telephone Bill").Select(s => s.Id).SingleOrDefault().ToString();

                var tree82 = lg.Where(x => x.Name == "IT and Internet Expenses").Select(s => s.Id).SingleOrDefault().ToString();

                var tree83 = lg.Where(x => x.Name == "Owner's Equity").Select(s => s.Id).SingleOrDefault().ToString();
                var tree84 = lg.Where(x => x.Name == "Drawings").Select(s => s.Id).SingleOrDefault().ToString();
                var tree87 = lg.Where(x => x.Name == "Trade Discount").Select(s => s.Id).SingleOrDefault().ToString();



                List<ChartTree> cTree = new List<ChartTree>() {
                    new ChartTree() {id =treerood, text  = companyName, parent ="#",CompanyId=companyidrood, type= "root"},
                    new ChartTree() {id =tree1, text  ="Assets", parent =treerood,  CompanyId=companyidrood, type= "chart"},
                    new ChartTree() {id =tree2 , text  ="Liabilities", parent =treerood, CompanyId=companyidrood, type= "chart"},
                    new ChartTree() {id =tree3 , text  ="Income", parent =treerood, CompanyId=companyidrood, type= "chart"},
                    new ChartTree() {id =tree4 , text  ="Capital", parent =treerood, CompanyId=companyidrood, type= "chart"},
                    new ChartTree() {id =tree5 , text  ="Expenses", parent =treerood, CompanyId=companyidrood, type= "chart"},

                    new ChartTree() {id =tree6.ToString() , text  ="Fixed Assets", parent =tree1, CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree7.ToString() , text  ="Current Assets", parent =tree1, CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree8.ToString() , text  ="Loan Accounts", parent =tree2, CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree9.ToString() , text  ="Sales Representative", parent =tree2, CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree10.ToString() , text  ="Profit & Loss Accounts", parent =tree2, CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree11.ToString() , text  ="Current Liabilities", parent =tree2, CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree12.ToString() , text  ="Paid Up Capital", parent =tree4, CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree13.ToString() , text  ="Owner's Equity", parent =tree4, CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree14.ToString() , text  ="Sales Accounts", parent =tree3, CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree15.ToString() , text  ="Indirect Income", parent =tree3, CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree16.ToString() , text  ="Direct Expense", parent =tree5, CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree17.ToString() , text  ="Indirect Expense", parent =tree5, CompanyId=companyidrood, type= "cat"},
                     new ChartTree() {id =tree18.ToString() , text  ="Purchase Accounts", parent =tree5, CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree19.ToString() , text  ="Building & Others", parent =tree6.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree20.ToString() , text  ="Elictrical Equipment", parent =tree6.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree21.ToString() , text  ="Furniture & Equipment", parent =tree6.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree22.ToString() , text  ="Plant & Machinary", parent =tree6.ToString(), CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree24.ToString() , text  ="Cash in Hand", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree25.ToString() , text  ="Bank Accounts", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree26.ToString() , text  ="Loan, advance and deposit", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree27.ToString() , text  ="Sundry Debtors", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree28.ToString() , text  ="Advance A/C", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree29.ToString() , text  ="Accounts Recieveable", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree33.ToString() , text  ="Accounts Recieveable (Customer)", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree30.ToString() , text  ="Inventory", parent =tree7.ToString(), CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree31.ToString() , text  ="Sundry Creditors", parent =tree11.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree32.ToString() , text  ="Accounts Payable", parent =tree11.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree81.ToString() , text  ="Accounts Payable (Supplier)",  parent =tree11.ToString(), CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree34.ToString() , text  ="Conveyance", parent =tree17.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree35.ToString() , text  ="Entertainment", parent =tree17.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree36.ToString() , text  ="Others Exp.", parent =tree17.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree37.ToString() , text  ="Discount", parent =tree17.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree38.ToString() , text  ="Staff Salary", parent =tree17.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree39.ToString() , text  ="Telephone Bill", parent =tree17.ToString(), CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree40.ToString() , text  ="Direct Raw Materials Purchase", parent =tree18.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree41.ToString() , text  ="Indirect Raw Material Purchase", parent =tree18.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree42.ToString() , text  ="Finished Goods Purchase", parent =tree18.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree85.ToString() , text  ="Purchase Others Expense", parent =tree18.ToString(), CompanyId=companyidrood, type= "cat"},

                    new ChartTree() {id =tree43.ToString() , text  ="Direct Raw Materials", parent =tree40.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree44.ToString() , text  ="Indirect Raw Material", parent =tree41.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree45.ToString() , text  ="Work in Progress", parent =tree40.ToString(), CompanyId=companyidrood, type= "cat"},
                    new ChartTree() {id =tree46.ToString() , text  ="Finished Goods", parent =tree42.ToString(), CompanyId=companyidrood, type= "cat"},


                    new ChartTree() {id =tree47.ToString() , text  ="Cash A/C", parent =tree24.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree48.ToString() , text  ="Petty Cash", parent =tree24.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree49.ToString() , text  ="Advance Against Expenses", parent =tree26.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree50.ToString() , text  ="Advance Against Salary", parent =tree26.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree51.ToString() , text  ="Advance Tax", parent =tree26.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree52.ToString() , text  ="Sales", parent =tree14.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree53.ToString() , text  ="Sales to Customers (Cash)", parent =tree14.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree54.ToString() , text  ="Purchase", parent =tree46.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                     new ChartTree() {id =tree86.ToString() , text  ="Cost Of Goods Purchase", parent =tree85.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree55.ToString() , text  ="Accounts Recieveable", parent =tree29.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree56.ToString() , text  ="Prepaid Expenses", parent =tree28.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree57.ToString() , text  ="Closing Inventory", parent =tree30.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree58.ToString() , text  ="Building & Others", parent =tree19.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree59.ToString() , text  ="Elictrical Equipment", parent =tree20.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree60.ToString() , text  ="Furniture & Equipment", parent =tree21.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree61.ToString() , text  ="Plant & Machinary", parent =tree22.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree62.ToString() , text  ="Accumulated Depreciation", parent =tree6.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree63.ToString() , text  ="Account Payable", parent =tree32.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree64.ToString() , text  ="Payroll Payable", parent =tree32.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree65.ToString() , text  ="Texes Payable", parent =tree32.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree66.ToString() , text  ="Additional Income", parent =tree15.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree67.ToString() , text  ="Sales Return Allowances", parent =tree15.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree68.ToString() , text  ="Cost Of Goods Sold", parent =tree17.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree69.ToString() , text  ="Advertising Expense", parent =tree17.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree70.ToString() , text  ="Bank Fees", parent =tree17.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree71.ToString() , text  ="Depreciation Expense", parent =tree17.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree72.ToString() , text  ="Payroll Tex Expense", parent =tree17.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree73.ToString() , text  ="Rent Expense", parent =tree17.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree74.ToString() , text  ="Supplies Expense", parent =tree36.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree75.ToString() , text  ="Utilities Expense", parent =tree36.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree76.ToString() , text  ="Wages Expense", parent =tree38.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree77.ToString() , text  ="Conveyance", parent =tree34.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree78.ToString() , text  ="Entertainment", parent =tree35.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree79.ToString() , text  ="Discount", parent =tree37.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree87.ToString() , text  ="Trade Discount", parent =tree37.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree80.ToString() , text  ="Telephone Bill", parent =tree39.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id=tree82.ToString() , text  ="IT and Internet Expenses", parent =tree17.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},

                    new ChartTree() {id =tree83.ToString() , text  ="Owner's Equity", parent =tree13.ToString(), CompanyId=companyidrood , isLedger=true, type= "ledger"},
                    new ChartTree() {id =tree84 , text  ="Drawings", parent =tree13, CompanyId=companyidrood, isLedger=true, type= "ledger"}
                };
                foreach (var item4 in cTree)
                {
                    
                    db.ChartTree.Add(item4);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Please Check Again Something Wrong!!!";
            }

            return View(model);
        }
        [Authorize(Roles = "companyAdmin")]
        public ActionResult BranchList()
        {
            var username = User.Identity.GetUserId();
            var branchList = db.Branch
                .Where(a => a.ApplicationUser_Branch
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            return View(branchList);
        }
        public ActionResult GetBranchList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                //var jsonData = new List<Branch>();
                var jsonData = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new
                {
                    Id = s.Id,
                    CompanyName = s.Company.CompanyName,
                    BranchName = s.BranchName,
                    BranchStatus = s.Status,
                    Address = s.Address

                }).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }
        [Authorize(Roles = "companyAdmin")]
        public ActionResult AddNewBranch()
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
        [ValidateAntiForgeryToken]
        public ActionResult AddNewBranch(AddBranchVM model)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelError("BranchNameExist", "Branch Name is already exist");
                Branch branch = new Branch();
                var isExist = IsDataExist(model.BranchName);
                if (!isExist)
                {
                    branch.Id = Guid.NewGuid();
                    branch.BranchName = model.BranchName;
                    branch.Address = model.Address;
                    branch.CompanyId = model.CompanyId;
                    db.Branch.Add(branch);
                    var Branch_Id = branch.Id;

                    ApplicationUser_Branch abranch = new ApplicationUser_Branch();
                    abranch.ApplicationUser_Id = User.Identity.GetUserId();
                    abranch.Branch_Id = Branch_Id;

                    db.ApplicationUser_Branch.Add(abranch);
                    db.SaveChanges();
                    return RedirectToAction("BranchList", "Business");
                }
            }
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", model.CompanyId);
            return View();
        }
        [NonAction]
        public bool IsDataExist(string data)
        {

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                        .ToList().Where(a => a.BranchName.ToUpper() == data.ToUpper()).FirstOrDefault();
                return v != null;
            }
        }
        public ActionResult UpdateBranch(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branch.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", branch.CompanyId);
            ViewBag.CompanyName = db.Branch.Where(x => x.Id == id).Select(s => s.Company.CompanyName).FirstOrDefault();
            
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBranch(Branch branch)
        {
            if (ModelState.IsValid)
            {
                Branch s = new Branch();
                s = db.Branch.FirstOrDefault(f => f.Id == branch.Id);
                s.Address = branch.Address;
                s.Status = branch.Status;
                //db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BranchList");
            }
            return View(branch);
        }
        [Authorize(Roles = "companyAdmin")]
        public ActionResult WarehouseList()
        {
            var username = User.Identity.GetUserId();
            var warehouseList = db.Warehouse.Include(a => a.Company.ApplicationUser_Company)
                .Where(b => b.Company.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            return View(warehouseList);
        }
        public ActionResult GetWarehouseList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                //var jsonData = new List<Branch>();
                var jsonData = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => new
                {
                    Id = s.Id,
                    ComName = s.Company.CompanyName,
                    WarehouseName = s.WarehouseName,
                    warhouseStatus = s.Status,
                    Address = s.Address

                }).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }
        }
        [Authorize(Roles = "companyAdmin")]
        public ActionResult AddNewWarehouse()
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
        [ValidateAntiForgeryToken]
        public ActionResult AddNewWarehouse(AddWarehouseVM model)
        {
            Warehouse warehouse = new Warehouse();
            if (ModelState.IsValid)
            {
                ModelState.AddModelError("WarehouseNameExist", "Warehouse Name is already exist");
                var isExist = IsDataExistt(model.WarehouseName);
                if (!isExist)
                {
                    warehouse.Id = Guid.NewGuid();
                    warehouse.WarehouseName = model.WarehouseName;
                    warehouse.Address = model.Address;
                    warehouse.CompanyId = model.CompanyId;
                    db.Warehouse.Add(warehouse);
                    db.SaveChanges();
                    return RedirectToAction("WarehouseList", "Business");
                }
            }
            var username = User.Identity.GetUserId();
            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();
            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName", model.CompanyId);
            return View();
        }

        [NonAction]
        public bool IsDataExistt(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.Warehouse.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                        .ToList().Where(a => a.WarehouseName.ToUpper() == data.ToUpper()).FirstOrDefault();
                return v != null;
            }
        }
        public ActionResult UpdateWarehouse(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouse.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", warehouse.CompanyId);
            ViewBag.CompanyName = db.Warehouse.Where(x => x.Id == id).Select(s => s.Company.CompanyName).FirstOrDefault();
            return View(warehouse);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateWarehouse(Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                //var entry = db.Entry(branch).State = EntityState.Modified;
                var entry = db.Entry(warehouse);
                entry.State = EntityState.Modified;
                entry.Property(e => e.CompanyId).IsModified = false;
                entry.Property(e => e.WarehouseName).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("WarehouseList");
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", warehouse.CompanyId);
            return View(warehouse);
        }
        [HttpGet]
        public ActionResult UpdateCompany(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCompany(Company company)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Company.Attach(company);

                var oldfilepath = db.Company.Where(x => x.Id == company.Id).Select(s => s.CompanyLogo).FirstOrDefault();
                if (company.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(company.ImageUpload.FileName);
                    string extension = Path.GetExtension(company.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    company.CompanyLogo = "~/Image/company_logo/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/company_logo/"), fileName);
                    company.ImageUpload.SaveAs(fileName);
                    string fullpath = Request.MapPath(oldfilepath);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                }

                var entry = db.Entry(company);
                entry.State = EntityState.Modified;

                entry.Property(e => e.CompanyName).IsModified = false;
                entry.Property(e => e.FinancialYearStart).IsModified = false;
                entry.Property(e => e.BusinessType).IsModified = false;
                entry.Property(e => e.CreatedOn).IsModified = false;
                entry.Property(e => e.Status).IsModified = false;
                entry.Property(e => e.Islocked).IsModified = false;
                entry.Property(e => e.LockedDateTime).IsModified = false;
                if (company.ImageUpload == null)
                {
                    entry.Property(e => e.CompanyLogo).IsModified = false;
                }
                ViewBag.ConMess = "Data Saved Successfully";

                db.SaveChanges();
                return View(company);
            }
            //  return View(company);
        }

    }
}