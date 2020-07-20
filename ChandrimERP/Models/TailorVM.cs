using ChandrimERP.Models;
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
    public class Tailor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
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
        public virtual ICollection<Branch_Tailor> Branch_Tailor { get; set; }
    }
    
    public class TailorOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int InvoiceNo { get; set; }
        [ForeignKey("Customer")]
        public Guid CustomerID { get; set; }
        [ForeignKey("Ledger")]
        public Guid LedgerId { get; set; }
        public string InvoiceType { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
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
        public Guid BranchId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        [ForeignKey("SalesAgent")]
        public Guid? SalesAgentId { get; set; }
        public string Narration { get; set; }
        public int? TotalQNT { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? SeawingAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal InvoicedAmount { get; set; }
        public decimal? LaseAmount { get; set; }
        public decimal? Addamount { get; set; }
        public string UserId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Ledger Ledger { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual SalesAgent SalesAgent { get; set; }
        public virtual ICollection<TailorOrderDetail> TailorOrderDetail { get; set; }
        public virtual ICollection<SeawingOrderDetails> SeawingOrderDetails { get; set; }
    }
    public class TailorOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("TailorOrder")]
        public Guid OrderID { get; set; }
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public int ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public string MeasureUnit { get; set; }
        public int? BonusQuantity { get; set; }
        public string BatchOrSerial { get; set; }
        public decimal NetTotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? VAT { get; set; }
        public virtual TailorOrder TailorOrder { get; set; }
        public virtual Product Product { get; set; }
    }

    public class SeawingOrderDetails
    {
        public Guid Id { get; set; }
        [DisplayName("Shelha")]
        public Shelha? Shelha { get; set; }
        [DisplayName("Ibad Mohra")]
        public IbadMohra? IbadMohra { get; set; }

        [DisplayName("Tul Long")]
        public string TulLong { get; set; }
        [DisplayName("Arrad Body")]
        public string ArradBody { get; set; }
        [DisplayName("Kocchor Waist")]
        public string KocchorWaist { get; set; }
        [DisplayName("Ibad")]
        public string Ibad { get; set; }
        [DisplayName("Hip")]
        public string Hip { get; set; }
        [DisplayName("Khetab Shoulder")]
        public string KhetabShoulder { get; set; }
        [DisplayName("Ordun Hand")]
        public string OrdunHand { get; set; }
        [DisplayName("Orgoba")]
        public string Orgoba { get; set; }
        [DisplayName("Throat")]
        public string Throat { get; set; }
        [DisplayName("Gher Enclorure")]
        public string GherEnclorure { get; set; }

        public string Shoulder { get; set; }
        public string Neck { get; set; }
        public string Bust { get; set; }
        public string Waist { get; set; }
        [DisplayName("Arm Hole")]
        public string ArmHole { get; set; }
        public string Muscle { get; set; }
        public string Elbow { get; set; }
        public string Cuff { get; set; }
        [DisplayName("Sleeve Length")]
        public string SleeveLength { get; set; }
        public string Length { get; set; }
        [DisplayName("Lenght From Shoulder To Hip")]
        public string LenghtFromShoulderToHip { get; set; }
        [DisplayName("Pant Length")]
        public string PantLength { get; set; }
        [DisplayName("Leg Opening")]
        public string LegOpening { get; set; }
        public string Note { get; set; }
        [DisplayName("Quantity")]
        public int Qty { get; set; }
        [DisplayName("Rate")]
        public int Rate { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
        [ForeignKey("TailorOrder")]
        public Guid TailorOrderId { get; set; }
        public virtual TailorOrder TailorOrder { get; set; }


    }
    // Create Vm for Sales Invoice Report
    public class SalesInvoiceReportVMtailor
    {

        public TailorOrder TailorOrder { get; set; }
        public List<TailorOrderDetail> TailorOrderDetail { get; set; }
    }
    public class TailorOrderVM
    {

        public string UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public int InvoiceNo { get; set; }
        public string CustomerID { get; set; }
        public string LedgerId { get; set; }
        public string InvoiceType { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }
        public string BranchId { get; set; }
        public string WarehouseId { get; set; }
        public string SalesAgentId { get; set; }
        [DisplayName("Due Date")]
        public string LastOrderDate { get; set; }
        [DisplayName("Curr Bal.")]
        public decimal PrevousDues { get; set; }
        public string ScanCode { get; set; }
        public string Barcode { get; set; }
        public string ScanProductName { get; set; }
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
        public decimal? SeawingAmount { get; set; }
        public string Narration { get; set; }
        public int TotalQNT { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal NetTotalAmount { get; set; }
        public decimal LessAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal AddAmount { get; set; }
        public decimal InvoicedAmount { get; set; }
        public Shelha Shelha { get; set; }
        public IbadMohra IbadMohra { get; set; }
        public List<TailorOrderDetail> TailorOrderDetail { get; set; }
        [DisplayName("Opening Blance")]
        public decimal? OpeningBlance { get; set; }
        [Required, DisplayName("First Name")]
        public string ContactFirstName { get; set; }
        [Required, DisplayName("Last Name")]
        public string ContactLastName { get; set; }
        [DisplayName("Genders")]
        public Gender Genders { get; set; }
        [Required, DisplayName("Phone Number")]
        public string Phone { get; set; }

        [DisplayName("Address Line"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }

        public Guid CompanyId { get; set; }

        public decimal? PayAmount { get; set; }
        public Guid? PaymentType { get; set; }
    }

    public class DressModel
    {
        public Guid Id { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        [DisplayName("Image"),DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }

        public DressModel()
        {
            ImageUrl = "~/Image/DressModel/user.png";
        }
        public virtual Company Company { get; set; }
    }
}