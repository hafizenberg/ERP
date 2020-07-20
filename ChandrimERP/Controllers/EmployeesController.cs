using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Employees
        [Authorize(Roles = "companyAdmin, emp_employee")]
        public ActionResult Employee()
        {
            var username = User.Identity.GetUserId();
            var empTypeList = db.EmployeeType.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.EmpTypeList = new SelectList(empTypeList, "Id", "Name");
            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName");
            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");
            
            return View(new EmployeeVm());
        }
        //post Employees
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Employee(EmployeeVm model, HttpPostedFileBase ImageUpload)
        {
            var username = User.Identity.GetUserId();
            var branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
            .ToList();
            ViewBag.BranchId = new SelectList(branch, "Id", "BranchName", model.BranchId);
            var empTypeList = db.EmployeeType.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
             .ToList();
            ViewBag.EmpTypeList = new SelectList(empTypeList, "Id", "Name",model.EmployeeType);

            var company = db.Company.Where(a => a.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
           .ToList();
            ViewBag.CompanyId = new SelectList(company, "Id", "CompanyName");

            if (ModelState.IsValid)
            {
                var isExist = IsDataExisttwo(model.NationalId);
                if (isExist)
                {
                    ModelState.AddModelError("NationalIdExist", "National id is already exist.");
                    branch = db.Branch.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                    .ToList();
                    ViewBag.BranchId = new SelectList(branch, "Id", "BranchName", model.BranchId);
                    empTypeList = db.EmployeeType.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                     .ToList();
                    ViewBag.EmpTypeList = new SelectList(empTypeList, "Id", "Name", model.EmployeeType);
                    return View(model);
                }
                var com = db.Branch.SingleOrDefault(c => c.Id == model.BranchId);
                    var comid = com.CompanyId;
                    Employee emp = new Employee();

                if (ImageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(s => s.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");
                    string directoryPath = "~/UploadedFiles/" + companyname + "/Employee_image/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                        Directory.CreateDirectory(Server.MapPath(directoryPath));

                    string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName);
                    string extension = Path.GetExtension(ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    ImageUpload.SaveAs(fileName);
                }
                
                var rowcount = db.Employee.Count(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username));
                if (rowcount > 0)
                {
                    var empcode = db.Employee.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Max(x => x.EmployeeCode);
                    emp.EmployeeCode = empcode + 1;
                }
                else
                {
                    emp.EmployeeCode = 1000;
                }

                emp.Id = Guid.NewGuid();
                emp.CompanyId =comid;
                emp.EmailAddress = model.EmailAddress;
                emp.JobTitle = model.JobTitle;
                emp.BussinessPhone = model.BussinessPhone;
                emp.ImageUrl = model.ImageUrl;
                emp.Notes = model.Notes;
                emp.Status = model.Status;
                emp.EmpTypeId = model.EmployeeType;

                db.Employee.Add(emp);
                db.SaveChanges();
                var Id = emp.Id;

                Branch_Employee be = new Branch_Employee();

                be.Employee_Id = Id;
                be.Branch_Id = model.BranchId;
                db.Branch_Employee.Add(be);

                EmployeePersionalInfo epi = new EmployeePersionalInfo();
                epi.EmployeesId = Id;
                epi.FirstName = model.FirstName;
                epi.LastName = model.LastName;
                epi.DateOfBirth = model.DateOfBirth;
                epi.Genders = model.Genders;
                epi.BloodGroup = model.BloodGroup;
                epi.HomePhone = model.HomePhone;
                epi.MobilePhone = model.MobilePhone;
                epi.NationalId = model.NationalId;
                epi.TinNumber = model.TinNumber;
                epi.Country = model.Country;
                epi.State = model.State;
                epi.City = model.City;
                epi.AddressLineOne = model.AddressLineOne;
                epi.AddressLineTwo = model.AddressLineTwo;
                epi.ZipOrPostalCode = model.ZipOrPostalCode;
                db.EmployeePersionalInfo.Add(epi);

                EmployeePropInfo epri = new EmployeePropInfo();
                epri.EmployeesId = Id;

                epri.EmployeeJoiningDate = model.EmployeeJoiningDate;
                epri.SalaryTypes = model.SalaryTypes;
                epri.EmpBasicSalary = model.EmpBasicSalary;
                epri.IsOvertime = model.IsOvertime;
                db.EmployeePropInfo.Add(epri);

                var lcat = db.LedgerCategory.Where(a => a.ChartOfAccount.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().Where(x => x.Name == "Staff Salary").Select(s => s.Id).SingleOrDefault();

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
                lg.Email = model.EmailAddress;
                lg.Country = model.Country;
                lg.Name = model.FirstName+ ' ' + model.LastName +' '+'(' + model.JobTitle + ')';
                lg.Address = model.Country + ',' + model.State + ',' + model.City;
                lg.EffectInventory = false;
                lg.EffectPayrool = true;
                lg.Country = model.Country;
                lg.State = model.State;
                lg.City = model.City;
                lg.PhoneNo = model.MobilePhone;
                lg.RefType = "Employee";
                lg.RefNo = Id;
                lg.CompanyId = comid;
                lg.LedgerCategoryId = lcat;
                lg.isDefault = true;

                db.Ledger.Add(lg);


                var id = lg.Id;
                var parent = lcat;
                var name = lg.Name;
                ChartTree ctree = new ChartTree();
                ctree.id = id.ToString();
                ctree.parent = parent.ToString();
                ctree.text = name;
                ctree.isLedger = true;
                ctree.type = "ledger";
                ctree.CompanyId = comid;

                db.ChartTree.Add(ctree);

                db.SaveChanges();

                ModelState.Clear();
                return RedirectToAction("Index", "Employees");
            }
            else
            {
                ViewBag.Message = "Please Check Again Something Wrong!!!";
            }
            return View(model);
        }
        [NonAction]
        public bool IsDataExisttwo(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.EmployeePersionalInfo
                        .Where(a => a.Employees.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList().FirstOrDefault(a => a.NationalId == data);
                return v != null;
            }
        }
        // GET: Employees
        [Authorize(Roles = "companyAdmin, emp_index")]
        public ActionResult Index()
        {
            var employee = db.Employee.Include(e => e.Company);
            return View(employee.ToList());
        }

        public ActionResult GetEmployeeList()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Employee>();
                jsonData = db.Employee.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Where(x=>x.Status==true).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }

        }
        public ActionResult GetEmployeeListData()
        {
            try
            {
                var username = User.Identity.GetUserId();
                db.Configuration.ProxyCreationEnabled = false;
                var jsonData = new List<Employee>();
                jsonData = db.Employee.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).ToList();
                return Json(new { data = jsonData }, JsonRequestBehavior.AllowGet);
            }
            catch (EntityException ex)
            {
                return Content(" Connection to Database Failed." + ex);
            }

        }

        // GET: Employees/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmployeeCode,EmailAddress,JobTitle,BussinessPhone,ImageUrl,Notes,Status,Islocked,LockedDateTime,CreatedOn,CompanyId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.Id = Guid.NewGuid();
                db.Employee.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", employee.CompanyId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult UpdateEmployee(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.pId = db.EmployeePersionalInfo.Where(x => x.Employees.Id == id).Select(s => s.Id).SingleOrDefault();
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", employee.CompanyId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmployee(Employee employee, HttpPostedFileBase imageUpload)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                Employee s = new Employee();
                s = db.Employee.FirstOrDefault(f => f.Id == employee.Id);
                var oldfilepath = db.Company.Where(x => x.Id == employee.Id).Select(sd => sd.CompanyLogo).FirstOrDefault();
                if (imageUpload != null)
                {
                    var companyname = db.Company.Where(x => x.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username)).Select(d => d.CompanyName).FirstOrDefault();
                    companyname = companyname.Replace(".", "").Replace(" ", "-").Replace(">", "-").Replace("<", "-").Replace("\"", "-").Replace("?", "-").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("*", "-").Replace("|", "-");

                    string directoryPath = "~/UploadedFiles/" + companyname + "/Employee_image/Image/";
                    bool folderExists = Directory.Exists(Server.MapPath(directoryPath));
                    if (!folderExists)
                    {
                        Directory.CreateDirectory(Server.MapPath(directoryPath));
                    }
                    string fileName = Path.GetFileNameWithoutExtension(imageUpload.FileName);
                    string extension = Path.GetExtension(imageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    employee.ImageUrl = directoryPath + fileName;
                    fileName = Path.Combine(Server.MapPath(directoryPath), fileName);
                    imageUpload.SaveAs(fileName);
                    string fullpath = Request.MapPath(oldfilepath);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                }
                s.ImageUrl = employee.ImageUrl;
                s.EmailAddress = employee.EmailAddress;
                s.JobTitle = employee.JobTitle;
                s.BussinessPhone = employee.BussinessPhone;
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName", employee.CompanyId);
            return View(employee);
        }
        public ActionResult UpdateEmployeePersonal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeePersionalInfo employee = db.EmployeePersionalInfo.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmployeePersonal(EmployeePersionalInfo employee)
        {
            var username = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                EmployeePersionalInfo s = new EmployeePersionalInfo();
                s = db.EmployeePersionalInfo.FirstOrDefault(f => f.Id == employee.Id);
                s.FirstName = employee.FirstName;
                s.LastName = employee.LastName;
                s.HomePhone = employee.HomePhone;
                s.MobilePhone = employee.MobilePhone;
                s.TinNumber = employee.TinNumber;
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UpdateEmployee");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Employee employee = db.Employee.Find(id);
            db.Employee.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult addemptype()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addEmptypes(EmployeeType empTypes)
        {
            string message = "";
            var isExist = IsDataExist(empTypes.Name);
            if (isExist)
            {
                message = "Employee type is already exist.";
            }
            else
            {
                empTypes.Id = Guid.NewGuid();
                db.EmployeeType.Add(empTypes);
                db.SaveChanges();
                message = "Employee type is added successfully";
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }
        [NonAction]
        public bool IsDataExist(string data)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = User.Identity.GetUserId();
                var v = db.EmployeeType.Where(a => a.Company.ApplicationUser_Company.Any(c => c.ApplicationUser_Id == username))
                        .ToList().Where(a => a.Name.ToUpper() == data.ToUpper()).FirstOrDefault();
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
