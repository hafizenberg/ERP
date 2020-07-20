
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using System.ComponentModel.DataAnnotations.Schema;


namespace ChandrimERP.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [Required, DisplayName("First Name")]
        public string ContactFirstName { get; set; }
        [Required, DisplayName("Last Name")]
        public string ContactLastName { get; set; }
        public Gender Genders { get; set; }
        [Required, DisplayName("Phone Number")]
        public string Phone { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Web Address")]
        public string WebPage { get; set; }
        //Start New field...........
        [DisplayName("Vat Information")]
        public string VatInformation { get; set; }
        public FinancialYear FinancialYearStart { get; set; }
        [DisplayName("Business Type")]
        public BusinessType BusinessType { get; set; }
        [DisplayName("Company Logo")]
        public string CompanyLogo { get; set; }
        //End New Field........

        [Required, DisplayName("Country/Region")]
        public string Country { get; set; }
        [DisplayName("State/Province")]
        public string State { get; set; }
        [Required, DisplayName("City")]
        public string City { get; set; }
        [DisplayName("Address Line1"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }
        [DisplayName("Address Line2"), DataType(DataType.MultilineText)]
        public string AddressLineTwo { get; set; }
        [DisplayName("Image")]
        public string ImageUrl { get; set; }
        [DisplayName("Notes"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Is Locked")]
        public bool Islocked { get; set; }
        [DisplayName("Locked Date & Time")]
        public DateTime? LockedDateTime { get; set; }

        private DateTime _createdOn = DateTime.MinValue;
        //public DateTime CreatedOn;
        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedOn
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }

        public virtual ICollection<Branch> Branch { get; set; }
        public virtual ICollection<Warehouse> Warehouse { get; set; }
        public virtual ICollection<ApplicationUser_Company> ApplicationUser_Company { get; set; }
        public virtual ICollection<Supplier> Supplier { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<SalesAgent> SalesAgent { get; set; }
        public virtual  ICollection<Product> Product { get; set; }
        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<Tailor> Tailor { get; set; }
        public virtual ICollection<EmployeeType> EmployeeType { get; set; }
        public virtual ICollection<BankDetailsCompany> BankDetailsCompany { get; set; }
        public virtual ICollection<MobileBankingCompany> MobileBankingCompany { get; set; }
        public virtual ICollection<InternationalBankingCompany> InternationalBankingCompany { get; set; }
        public virtual ICollection<ProductMeasureUnit> ProductMeasureUnit { get; set; }
        public virtual ICollection<ProductCategory> ProductCategory { get; set; }
        public virtual ICollection<ProductRack> ProductRack { get; set; }
        public virtual ICollection<ProductBrand> ProductBrand { get; set; }
        public virtual ICollection<PService> PService { get; set; }
        public virtual ICollection<ChartOfAccount> ChartOfAccounts{ get; set; }
        public virtual ICollection<ChartTree> ChartTree { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
        public virtual ICollection<Ledger> Ledger { get; set; }
        public virtual ICollection<DressModel> DressModel { get; set; }
        public virtual ICollection<InventoryIncomming> InventoryIncomming { get; set; }
        public virtual ICollection<InventoryOutGoing> InventoryOutGoing { get; set; }
        public virtual ICollection<Inventory> Inventory { get; set; }
        public virtual ICollection<InventoryMovement> InventoryMovement { get; set; }
        public virtual ICollection<Document> Document { get; set; }

        public virtual ICollection<InventoryValueChange> InventoryValueChange { get; set; }
        public virtual ICollection<InventoryUpcomming> InventoryUpcomming { get; set; }
        public virtual ICollection<InventoryUpgoing> InventoryUpgoing { get; set; }
        public virtual ICollection<InventoryExpiary> InventoryExpiary { get; set; }
        public virtual ICollection<FixedAsset> FixedAssets { get; set; }
        public virtual ICollection<FixedAssetCategory> FixedAssetCategories { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Is locked")]
        public bool Islocked { get; set; }
        [DisplayName("Locked Date & Name")]
        public DateTime? LockedDateTime { get; set; }

        private DateTime _createdOn = DateTime.MinValue;
        //public DateTime CreatedOn;
        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedOn
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<ApplicationUser_Branch> ApplicationUser_Branch { get; set; }
        public virtual ICollection<Branch_Supplier> Branch_Supplier { get; set; }
        public virtual ICollection<Branch_Employee> Branch_Employee { get; set; }
        public virtual ICollection<Branch_SalesAgent> Branch_SalesAgent { get; set; }
        public virtual ICollection<Branch_Product> Branch_Product { get; set; }
        public virtual ICollection<Branch_Customer> Branch_Customer { get; set; }
        public virtual ICollection<Branch_Warehouse> Branch_Warehouse { get; set; }
        public virtual ICollection<Branch_Document> Branch_Document { get; set; }
        public virtual ICollection<Branch_Tailor> Branch_Tailor { get; set; }
        public virtual ICollection<BankDetailsBranch> BankDetailsBranch { get; set; }
        public virtual ICollection<MobileBankingBranch> MobileBankingBranch { get; set; }
        public virtual ICollection<InternationalBankingBranch> InternationalBankingBranch { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<SalesOrder> SalesOrder { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual ICollection<TailorOrder> TailorOrder { get; set; }
        public virtual ICollection<FixedAsset> FixedAssets { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
        public virtual ICollection<TransactionDetails> TransactionDetailses { get; set; }
    }
    public class Warehouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [DisplayName("Warehouse Name")]
        public string WarehouseName { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Is locked")]
        public bool Islocked { get; set; }
        [DisplayName("Locked Date & Name")]
        public DateTime? LockedDateTime { get; set; }

        private DateTime _createdOn = DateTime.MinValue;
        //public DateTime CreatedOn;
        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedOn
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<Branch_Warehouse> Branch_Warehouse { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<SalesOrder> SalesOrder { get; set; }
        public virtual ICollection<TailorOrder> TailorOrder { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual ICollection<InventoryIncomming> InventoryIncomming { get; set; }
        public virtual ICollection<InventoryOutGoing> InventoryOutGoing { get; set; }
        public virtual ICollection<Inventory> Inventory { get; set; }
        public virtual ICollection<InventoryMovement> InventoryMovement { get; set; }

        public virtual ICollection<InventoryValueChange> InventoryValueChange { get; set; }
        public virtual ICollection<InventoryUpcomming> InventoryUpcomming { get; set; }
        public virtual ICollection<InventoryUpgoing> InventoryUpgoing { get; set; }
        public virtual ICollection<InventoryExpiary> InventoryExpiary { get; set; }
        public virtual ICollection<FixedAsset> FixedAssets { get; set; }
    }
}