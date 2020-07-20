using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Collections;
using ChandrimERP.Models;

namespace ChandrimERP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual UserDetails UserDetails { get; set; }
        public virtual ICollection<ApplicationUser_Company> ApplicationUser_Company { get; set; }
        public virtual ICollection<ApplicationUser_Branch> ApplicationUser_Branch { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("ChandrimConext", throwIfV1Schema: false)
        {
        }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<BankDetailsCustomer> BankDetailsCustomer { get; set; }
        public DbSet<BankDetailsEmployee> BankDetailsEmployee { get; set; }
        public DbSet<BankDetailsSupplier> BankDetailsSupplier { get; set; }
        public DbSet<BankDetailsCompany> BankDetailsCompany { get; set; }
        public DbSet<BankDetailsBranch> BankDetailsBranch { get; set; }
        public DbSet<MobileBankingCustomer> MobileBankingCustomer { get; set; }
        public DbSet<MobileBankingEmployee> MobileBankingEmployee { get; set; }
        public DbSet<MobileBankingSupplier> MobileBankingSupplier { get; set; }
        public DbSet<MobileBankingCompany> MobileBankingCompany { get; set; }
        public DbSet<MobileBankingBranch> MobileBankingBranch { get; set; }
        public DbSet<InternationalBankingCustomer> InternationalBankingCustomer { get; set; }
        public DbSet<InternationalBankingEmployee> InternationalBankingEmployee { get; set; }
        public DbSet<InternationalBankingSupplier> InternationalBankingSupplier { get; set; }
        public DbSet<InternationalBankingCompany> InternationalBankingCompany { get; set; }
        public DbSet<InternationalBankingBranch> InternationalBankingBranch { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeePersionalInfo> EmployeePersionalInfo { get; set; }
        public DbSet<EmployeePropInfo> EmployeePropInfo { get; set; }
        public DbSet<EmployeeNomineeInfo> EmployeeNomineeInfo { get; set; }
        public DbSet<EmployeePayrollPolicy> EmployeePayrollPolicy { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductMeasureUnit> ProductMeasureUnit { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<ProductSubCategory> ProductSubCategory { get; set; }
        public DbSet<ProductBrand> ProductBrand { get; set; }
        public DbSet<ProductRack> ProductRack { get; set; }
        public DbSet<PService> PService { get; set; }
        public DbSet<PserviceImage> PserviceImage { get; set; }

        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<userLogData> userLogData { get; set; }


        public DbSet<SalesAgent> SalesAgent { get; set; }
        public DbSet<TailorOrder> TailorOrder { get; set; }
        public DbSet<TailorOrderDetail> TailorOrderDetail { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<SalesOrder> SalesOrder { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetail { get; set; }
        public DbSet<SeawingOrderDetails> SeawingOrderDetails { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
        public DbSet<Ledger> Ledger { get; set; }
        public DbSet<LedgerCategory> LedgerCategory { get; set; }
        public DbSet<ChartOfAccount> ChartOfAccount { get; set; }
        public DbSet<ChartTree> ChartTree { get; set; }
        public DbSet<Voucher> Voucher { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TransactionDetails> TransactionDetails { get; set; }

        public DbSet<Student> Student { get; set; }
        public DbSet<Image> Image { get; set; }
       // public DbSet<FileUpload1> FileUpload1 { get; set; }

        public DbSet<ApplicationUser_Company> ApplicationUser_Company { get; set; }
        public DbSet<ApplicationUser_Branch> ApplicationUser_Branch { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Branch_Warehouse> Branch_Warehouse { get; set; }
        public DbSet<Branch_Supplier> Branch_Supplier { get; set; }
        public DbSet<Branch_Employee> Branch_Employee { get; set; }
        public DbSet<Branch_Product> Branch_Product { get; set; }
        public DbSet<Branch_Customer> Branch_Customer { get; set; }
        public DbSet<Branch_Document> Branch_Document { get; set; }
        public DbSet<EmployeeType> EmployeeType { get; set; }

        public DbSet<Tailor> Tailor { get; set; }
        public DbSet<Branch_PService> Branch_PService { get; set; }
        public DbSet<Branch_Tailor> Branch_Tailor { get; set; }
        public DbSet<FixedAsset> FixedAsset { get; set; }
        public DbSet<FixedAssetCategory> FixedAssetCategory { get; set; }
        public DbSet<DressModel> DressModel { get; set; }

        public DbSet<InventoryIncomming> InventoryIncomming { get; set; }
        public DbSet<InventoryOutGoing> InventoryOutGoing { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryMovement> InventoryMovement { get; set; }
        public DbSet<InventoryValueChange> InventoryValueChange { get; set; }
        public DbSet<InventoryUpcomming> InventoryUpcomming { get; set; }
        public DbSet<InventoryUpgoing> InventoryUpgoing { get; set; }
        public DbSet<InventoryExpiary> InventoryExpiary { get; set; }
        public IEnumerable ApplicationUsers { get; internal set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
