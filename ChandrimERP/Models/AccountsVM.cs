
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChandrimERP.Models
{
    public class Ledger {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int LedgerCode { get; set; }
        [ForeignKey("LedgerCategory")]
        public Guid LedgerCategoryId { get; set;}
        [DisplayName("Parent Ledger")]
        public Guid? ParentLedgerId { get; set; }
        [DisplayName("Effect Inventory")]
        public bool EffectInventory { get; set; }
        [DisplayName("Effect Payroll")]
        public bool EffectPayrool { get; set;}
        public decimal? OpeningBalance { get; set;}
        [DisplayName("Address"), DataType(DataType.MultilineText)]
        public string Address { get; set;}
        public string Country { get; set;}
        public string State { get; set;}
        public string City { get; set; }
        [DisplayName("Phone No"), DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public bool isDefault { get; set; }
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
        public string RefType { get; set; }
        public Guid? RefNo { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }       
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<SalesOrder> SalesOrder { get; set; }
        public virtual ICollection<TailorOrder> TailorOrder { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual ICollection<FixedAssetCategory> FixedAssetCategory { get; set; }
        public virtual LedgerCategory LedgerCategory { get; set;}
        public virtual Company Company { get; set; }
        public virtual ICollection<TransactionDetails> TransactionDetailses { get; set; }
    }

    public class LedgerCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [DisplayName("Category Name"),Required]
        public string Name { get; set; }
        [DisplayName("Chart Of Account"),ForeignKey("ChartOfAccount")]
        public Guid ChartOfAccountId { get; set; }
        [DisplayName("Parent Ledger Category")]
        public Guid? ParentLedgerCatId { get; set; }
        public bool isDefault { get; set; }
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
        public virtual ChartOfAccount ChartOfAccount { get; set; }
        public virtual ICollection<Ledger> Ledger { get; set; }
    }
    public class ChartOfAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [DisplayName("Chart Of Account")]
        public string Name { get; set; }
        [DisplayName("Parent Chart Of Account")]
        public Guid? ParentNode { get; set;}
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public bool isDefault { get; set; }
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
        public virtual ICollection<LedgerCategory> LedgerCategory { get; set; }
        public virtual Company Company { get; set; }
    }
    public  class ChartTree
    {
        [Key]
        public string id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string type { get; set; }
        public string parent { get; set; }
        public bool isLedger { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }

    }
    //public  class ChartTree
    //{
    //    public List<ChartTree> Childrens { get; set; }
    //}

    public class JsTreeModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string state { get; set; }
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
        public string li_attr { get; set; }
        public string a_attr { get; set; }
    }

    public class Voucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [DisplayName("Voucher Type")]
        public string VoucherType { get; set; }
        [DisplayName("Voucher Name")]
        public string Name { get; set;}
        public string Prefix { get; set;}
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
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
        public virtual Company Company { get; set; }
    }
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public int VoucherNo { get; set; }
        public string VoucherType { get; set; }
        public Decimal TrasactionalAmount { get; set; }
        public string Narration { get; set; }
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
        [ForeignKey("Branch")]
        public Guid BranchID { get; set; }
        public virtual Branch Branch { get; set;}
        public DateTime TransactionDate { get; set; }
        public virtual ICollection<TransactionDetails> TransactionDetailses { get; set; }
    }
    public class TransactionDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int VoucherNo { get; set; }
        public string VoucherType { get; set; }
        public string LedgerName { get; set; }
        public string LedgerNo { get; set; }
        public string DabitLedger { get; set; }
        public string CreditLedger { get; set; }
        public Decimal? DebitAmount { get; set; }
        public Decimal? CreditAmount { get; set; }
        public string Narration { get; set; }

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
        public DateTime TransactionDate { get; set; }
        [ForeignKey("Branch")]
        public Guid BranchID { get; set; }
        public virtual Branch Branch { get; set; }
        [ForeignKey("Ledger")]
        public Guid LedgerID { get; set; }
        public virtual Ledger Ledger { get; set; }
        [ForeignKey("Transaction")]
        public Guid TransactionID { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
    public class GetLedgerDetails
    {
        [DisplayName("From Date"), Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }
        [DisplayName("To Date"),Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
    }
    public class JournalReportVM
    {

        public Transaction Transaction { get; set; }
        public List<TransactionDetails> TransactionDetails { get; set; }
    }
}