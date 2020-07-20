using System;
using Microsoft.Owin;
using Owin;
using ChandrimERP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(ChandrimERP.Startup))]
namespace ChandrimERP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRoles();
        }

        private void createRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();

            string[] roleData = { "systemAdmin","companyAdmin","user", "acc_paymen", "acc_received", "acc_journal", "acc_contra", "acc_index", "acc_ledgerlist", "acc_ledgerdetails", "sup_index", "sup_create", "pro_index", "pro_create", "inv_index", "inv_Inventory", "cus_index", "cus_create", "ord_salesorder", "ord_purchaseorder", "sal_b_salesinvoicereport", "sal_b_salesorder", "sal_b_salesinvoice", "sal_b_salesreturn", "sal_b_t_salesinvoicereport", "sal_b_t_salesorder", "sal_b_t_salesinvoice", "sal_b_t_salesreturn", "pur_b_purchaseinvoicereport", "pur_b_purchaseorder", "pur_b_purchaseinvoice", "pur_b_purchasereturn", "emp_employee", "emp_index", "fix_a_index", "fix_a_create", "fix_a_c_create", "fil_index", "fil_uploadesfile", "dre_m_index", "dre_m_create", "acc_createledger","led_cat_create","cha_o_a_create" };

            foreach (var items in roleData)
            {
                if (!roleManager.RoleExists(items))
                {
                    foreach (var item in roleData)
                    {
                        role.Id = Guid.NewGuid().ToString();
                        role.Name = item;
                        roleManager.Create(role);
                    }
                }
            }
            var user = new ApplicationUser();
            if (UserManager.FindByName("chandrimsoft24@gmail.com") == null)
            {
                user.UserName = "chandrimsoft24@gmail.com";
                user.Email = "chandrimsoft24@gmail.com";
                user.EmailConfirmed = true;
                user.UserDetails = new   UserDetails() { FirstName = "Chandrim Soft", LastName = "Ltd.", Genders = 0, PhotosUrl = "/Image/user_logo/user.png"};
                string userPWD = "chandrimsoft24";
                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "systemAdmin");
                }
            }
        }
    }
}

