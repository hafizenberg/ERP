using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ChandrimERP.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ChandrimERP.Models
{
    public class SalesOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        [ForeignKey("Customer")]
        public Guid CustomerID { get; set; }
        
        [ForeignKey("Ledger")]
        public Guid LedgerId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        [ForeignKey("Branch")]
        public Guid BranchId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        [ForeignKey("SalesAgent")]
        public Guid? SalesAgentId { get; set; }
        public string Narration { get; set; }
        public int TotalQNT { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal InvoicedAmount { get; set; }
        public decimal? LaseAmount { get; set; }
        public decimal? Addamount { get; set; }
        public string UserId { get; set; }
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
        public virtual Customer Customer { get; set; }
        public virtual Ledger Ledger { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual SalesAgent SalesAgent { get; set; }
        public virtual ICollection<SalesOrderDetail> SalesOrderDetail { get; set; }
    }
    public class SalesOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("SalesOrder")]
        public Guid OrderID { get; set; }
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public string MeasureUnit { get; set; }
        public string Barcode { get; set; }
        public int? BonusQuantity { get; set; }
        public string BatchOrSerial { get; set; }
        public decimal NetTotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? VAT { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Product Product { get; set; }

    }
    public class SalesOrderVM
    {
        public string UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }
        public int InvoiceNo { get; set; }
        public string CustomerID { get; set; }
        public string LedgerId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        public string BranchId { get; set; }
        public string WarehouseId { get; set; }
        public string SalesAgentId { get; set; }
        [DisplayName("Closing Balance")]
        public string LastOrderDate { get; set; }
        [DisplayName("Curr Bal.")]
        public decimal PrevousDues { get; set; }
        public string ScanCode { get; set; }
        public string ScanProductName { get; set; }
        //[Range(0,1000000, ErrorMessage = "Quantity must be between 0-1000000")] 
        public int SetQuantity { get; set; }
        public int SetRate { get; set; }
        public decimal SetDisc { get; set; }
        public int SetBonusQuantity { get; set; }
        public int SetBatchOrSerial { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public int BonusQuantity { get; set; }
        public string BatchOrSerial { get; set; }
        public decimal NetTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Narration { get; set; }
        public int TotalQNT { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal NetTotalAmount { get; set; }
        public decimal LessAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal AddAmount { get; set; }
        public decimal InvoicedAmount { get; set; }
        public List<SalesOrderDetail> SalesOrderDetails { get; set; }

        [DisplayName("Company")]
        public string CompanyName { get; set; }
        [DisplayName("Customer Code")]
        public int? CustomerCode { get; set; }
        [DisplayName("Opening Balance")]
        public decimal? OpeningBlance { get; set; }
        [Required, DisplayName("First Name")]
        public string ContactFirstName { get; set; }
        [Required, DisplayName("Last Name")]
        public string ContactLastName { get; set; }
        [DisplayName("Genders")]
        public Gender Genders { get; set; }
        [Required, DisplayName("Phone Number")]
        public string Phone { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DisplayName("Web Address"), DataType(DataType.Url)]
        public string WebPage { get; set; }
        [DisplayName("Country/Region")]
        public string Country { get; set; }
        [DisplayName("State/Province")]
        public string State { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("Address Line1"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }
        [DisplayName("Address Line2"), DataType(DataType.MultilineText)]
        public string AddressLineTwo { get; set; }
        [DisplayName("Notes"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }

        public Guid CompanyId { get; set; }

        public decimal? PayAmount { get; set; }
        public Guid? PaymentType { get; set; }
    }

    // Create Vm for Sales Invoice Report
    public class SalesInvoiceReportVM
    {

        public SalesOrder order { get; set; }
        public List<SalesOrderDetail> orderdetail { get; set; }
    }
}