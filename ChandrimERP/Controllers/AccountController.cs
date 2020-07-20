using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;
using System.Web.Helpers;
using System.Web.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Core;
using System.Configuration;

namespace ChandrimERP.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        //Register Action
        [Authorize(Roles = "systemAdmin")]
        [HttpGet]
        public ActionResult Registation()
        {

            return View();
        }

        //Register Action Post
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registation(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email,UserDetails = new UserDetails() { FirstName =model.FirstName, LastName =model.LastName,Genders = model.Genders} };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action(
                       "ConfirmEmail", "Account",
                       new { userId = user.Id, code = code },
                       protocol: Request.Url.Scheme);
                    var addrole = UserManager.AddToRole(user.Id, "companyAdmin");
                    await UserManager.SendEmailAsync(user.Id,
                       "Confirm your account",
                       "Please confirm your account by clicking this link:  <a href=\"" + callbackUrl + "\">here</a>");
                    // ViewBag.Link = callbackUrl;   // Used only for initial demo.

                    return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = "systemAdmin, companyAdmin")]
        [HttpGet]
        public ActionResult AddUser()
        {
            var username = User.Identity.GetUserId();

            var companyList = db.Company
                .Where(a => a.ApplicationUser_Company
                .Any(c => c.ApplicationUser_Id == username))
                .ToList();

            ViewBag.CompanyId = new SelectList(companyList, "Id", "CompanyName");
            return View();
        }
        //Register Action Post
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, UserDetails = new UserDetails() { FirstName = model.FirstName, LastName = model.LastName, Genders = model.Genders } };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action(
                       "ConfirmEmail", "Account",
                       new { userId = user.Id, code = code },
                       protocol: Request.Url.Scheme);
                    var addrole = UserManager.AddToRole(user.Id, "user");
                    await UserManager.SendEmailAsync(user.Id,
                       "Confirm your account",
                       "Please confirm your account by clicking this link:  <a href=\"" + callbackUrl + "\">here</a>");
                    // ViewBag.Link = callbackUrl;   // Used only for initial demo.

                    ApplicationUser_Company acom = new ApplicationUser_Company();
                    acom.ApplicationUser_Id = user.Id;
                    acom.Company_Id = model.CompanyId;

                    db.ApplicationUser_Company.Add(acom);
                    db.SaveChanges();

                    return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }
            ViewBag.CompanyId = new SelectList(db.Company, "Id", "CompanyName",model.CompanyId);
            return View(model);
        }
        [Authorize(Roles = "systemAdmin, companyAdmin")]
        [HttpGet]
        public ActionResult Setrole()
        {
            var username = User.Identity.GetUserId();
            var compid = db.ApplicationUser_Company.Where(x => x.ApplicationUser_Id == username).Select(s => s.Company_Id).SingleOrDefault();
            var userlist = db.Users.Where(x => x.ApplicationUser_Company.Any( c => c.Company_Id == compid)).Where(s=>s.Id != username).ToList();

            var rolelist = db.Roles.Where(x => x.Name != "systemAdmin" && x.Name != "companyAdmin" && x.Name != "user");

            ViewBag.Userlist = userlist;
            ViewBag.Rolelist = new SelectList(rolelist, "Name", "Name");

            return View();
        }
        [HttpGet]
        public ActionResult SetRoletouser(Guid? id)
        {
            var UserName = db.Users.Where(x => x.Id == id.ToString()).Select(s => s.Email).FirstOrDefault();
           // IList<string> result = null;

                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                var result = UserManager.GetRoles(user.Id);
                var rolelist = db.Roles.Where(x => x.Name != "systemAdmin" && x.Name != "companyAdmin" && x.Name != "user").OrderBy(x => x.Name).Select(s=>s.Name).ToList();
                var data = rolelist.Except(result.Where(x => rolelist.Contains(x))).ToList();

              ViewBag.Roles = data; 

            return View();
        }
        [HttpPost]
        public ActionResult SetRoletouser(Guid? id, string[] rolelist)
            {
            try
            {
                if (rolelist != null)
                {
                    var userid = id.ToString();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    UserManager.AddToRoles(userid, rolelist);
                }
                return RedirectToAction("Setrole");

            }
            catch(EntityException ex)
            {
                return Content(" Select Some Data" + ex);
            }
        }
        [HttpGet]
        public ActionResult GetRolesbyUser(Guid? id)
        {
            var UserName = db.Users.Where(x => x.Id == id.ToString()).Select(s => s.Email).FirstOrDefault();
            IList<string> result = null;
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                 result= UserManager.GetRoles(user.Id).ToList();

                ViewBag.Roles = new SelectList(result, "Name", "Name"); 
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "systemAdmin, companyAdmin")]
        [HttpGet]
        public ActionResult Removerole(Guid? id)
        {
            var UserName = db.Users.Where(x => x.Id == id.ToString()).Select(s => s.Email).FirstOrDefault();
            IList<string> result = null;

                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                result = UserManager.GetRoles(user.Id).ToList();
                ViewBag.Roles = result;


            return View();
        }
        [HttpPost]
        public ActionResult Removerole(Guid? id, string[] rolelist)
        {
            try
            {
                if (rolelist != null)
                {
                    var userid = id.ToString();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                    UserManager.RemoveFromRoles(userid, rolelist);
                    ViewBag.ResultMessage = "Role removed from this user successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "This user doesn't belong to selected role.";
                }
                return RedirectToAction("Setrole");
            }
            catch (EntityException ex)
            {
                return Content(" Select Some Data" + ex);
            }
          
        }
        [HttpGet]
        public ActionResult Emailverify()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Emailverify(ActiveAccountViewModel model)
        {
            string message = "";
            string successmessage = "";

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            //var user =  db.Users.Where(a => a.Email == model.Email).FirstOrDefault();

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action(
                       "ConfirmEmail", "Account",
                       new { userId = user.Id, code = code },
                       protocol: Request.Url.Scheme);

                    await UserManager.SendEmailAsync(user.Id,
                       "Confirm your account",
                       "Please confirm your account by clicking this link:  <a href=\"" + callbackUrl + "\">here</a>");
                    successmessage = "Request successfully submitted!";

                }
                else message = "Your Account  already verified";
            }
            else message = "Your Account  not exist";
            ViewBag.SMessage = successmessage;
            ViewBag.Message = message;
            return View();
        }
        //Login Action
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Login(LoginViewModel login, string returnUrl)
        {
            string message = "";
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var data = db.Users.Where(a => a.Email == login.Email).FirstOrDefault();
                var pass = db.Users.Where(a => a.PasswordHash == login.Password).FirstOrDefault();
                if (data != null)
                {
                    if (!data.EmailConfirmed)
                    {
                        ViewBag.Message = "Please verify your e-mail first";
                        return View();
                    }
                    if (login.Password==null)
                    {
                        return View();
                    }
                    var result = await SignInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, shouldLockout: true);
                    var users = await UserManager.FindByNameAsync(login.Email);
                    if (await UserManager.IsLockedOutAsync(users.Id))
                    {
                         message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", "5 Minite");
                        ViewBag.Message = message;
                        return View();
                    }
                    if (await UserManager.GetAccessFailedCountAsync(users.Id) != 0)
                    {
                        int accessFailedCount = await UserManager.GetAccessFailedCountAsync(users.Id);
                        int attemptsLeft =
                               5
                               -
                                accessFailedCount;
                        message = string.Format(
                               "Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);
                        ViewBag.Message = message;
                        return View();
                    }
                    
                    switch (result)
                    {
                        case SignInStatus.Success:
                            {                               
                                var user = await UserManager.FindAsync(login.Email,login.Password);
                                if (user.UserDetails.Islocked==true)
                                {
                                    message = "Your Account is Locked";
                                    ViewBag.Message = message;
                                    return View();
                                }
                                var totalComapny = db.Company.Count(a=>a.ApplicationUser_Company.Any(x=>x.ApplicationUser_Id == user.Id));
                                var totalLockCompany = db.Company.Where(a => a.ApplicationUser_Company.Any(x => x.ApplicationUser_Id == user.Id)).Where(z=>z.Islocked==true).Count();
                                if (totalComapny <= totalLockCompany)
                                {
                                    message = "Your Company Account is Locked";
                                    ViewBag.Message = message;
                                    return View();
                                }
                                user.LockoutEndDateUtc = DateTime.UtcNow;
                                await UserManager.ResetAccessFailedCountAsync(users.Id);
                                UserManager.Update(user);
                            }
                            return RedirectToLocal(returnUrl);
                        case SignInStatus.LockedOut:
                            {
                                var user = await UserManager.FindAsync(login.Email, login.Password);
                                if (user.EmailConfirmed == false)
                                {
                                    user.LockoutEnabled = true;
                                    UserManager.Update(user);
                                }
                               
                               
                            }
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = login.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            message = "Please try agin";
                            ViewBag.Message = message;
                            return View();
                    }

                   
                }
                else
                {
                    message = "You are not Registerd User!! Please Registerd";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                string successmessage = "";

                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    message = "Your Account  not exist or  it will not verified Account ";
                    ViewBag.Message = message;
                    return View();                    
                }
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking  <a href=\"" + callbackUrl + "\">here</a>");
                successmessage = "Request successfully submitted!";
                ViewBag.SMessage = successmessage;
                return View();
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}