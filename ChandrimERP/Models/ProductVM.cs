
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ChandrimERP.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; }
        [DisplayName("Product Code")]
        public int ProductCode { get; set; }
        [Required,DisplayName("Product Name")]
        public string ProductName { get; set; }
        [DisplayName("Model Name")]
        public string ModelName { get; set; }
        [DisplayName("Supplier"),ForeignKey("Supplier")]
        public Guid SupplierId { get; set; }
        [DisplayName("Product Category")]
        [ForeignKey("ProductCategory")]
        public int? ProductCategoryId { get; set; }
        [DisplayName("Product Sub Category")]
        public int? ProductSubCategoryId { get; set; }
        [DisplayName("Product Measure Unit")]
        [ForeignKey("ProductMeasureUnit")]
        public int ProductMeasureUnitId { get; set; }
        [DisplayName("Product Brand")]
        [ForeignKey("ProductBrand")]
        public int? ProductBrandId { get; set; }
        [DisplayName("Product Rack")]
        [ForeignKey("ProductRack")]
        public int? ProductRackId { get; set; }
        [DisplayName("Product Type")]
        public string ProductType { get; set; }
        [DisplayName("Product Description")]
        public string ProductDescription { get; set; }
        [DisplayName("Product Size")]
        public string ProductSize { get; set; }
        [DisplayName("Image"),DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [DisplayName("Product Color")]
        public string ProductColor { get; set; }
        [DisplayName("Product Weight")]
        public string ProductWeight { get; set; }
        [DisplayName("Opening Quantity")]
        public int? ProductQuantity { get; set; }
        [DisplayName("Alert Level")]
        public int? ProductVolume { get; set; }
        //Must be change
        [DisplayName("Product MRP"),Required]
        public decimal ProductPrice { get; set; }
        [DisplayName("Product Unit Cost"), Required]
        public decimal ProductUnitCost { get; set; }
        [DisplayName("Product Manufacture")]
        public string ProductManufacture { get; set; }
        [DisplayName("Product Manufactur Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ProductManufactureDate { get; set; }
        [DisplayName("Product Expire Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ProductExpireDate { get; set; }
        public decimal Vat { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Is locked")]
        public bool Islocked { get; set; }
        [DisplayName("Locked Date & Name")]
        public DateTime? LockedDateTime { get; set; }
        [DisplayName("Status Value")]
        public string StatusValue { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        public Product()
        {
            ImageUrl = "~/Image/Product/product.png";
        }

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

        [DisplayName("Barcode")]
        public string Barcode { get; set; }
        [DisplayName("Batch Or Serial")]
        public string BatchOrSerial { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Supplier Supplier { get; set; }
        public  virtual ProductMeasureUnit ProductMeasureUnit { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductBrand ProductBrand { get; set; }
        public virtual ProductRack ProductRack { get; set; }
        public virtual ICollection<Branch_Product> Branch_Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<SalesOrderDetail> SalesOrderDetail { get; set; }
        public virtual ICollection<TailorOrderDetail> TailorOrderDetail { get; set; }
        public virtual ICollection<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
        public virtual ICollection<Inventory> Inventory { get; set; }
        public virtual ICollection<InventoryMovement> InventoryMovement { get; set; }
    }

    public class ProductMeasureUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }

    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<ProductSubCategory> ProductSubCategory { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }

    public class ProductSubCategory
    {
        public int Id { get; set; }
        [ForeignKey("ProductCategory")]
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }

    public class ProductBrand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }

    public class ProductRack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
    public class PService
    {
        public Guid Id { get; set;}
        [DisplayName("Service Code")]
        public int PServiceCode { get; set;}
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal VAT { get; set; }
        public DateTime? updateDate { get; set;}
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
        [DisplayName("Service Cost")]
        public decimal ServiceCost { get; set; }
        [DisplayName("Estimated Time")]
        public string EstimatedTime { get; set; }
        public bool Status { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
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
        public virtual ICollection<Branch_PService> Branch_PService { get; set; }
        public virtual ICollection<PserviceImage> PserviceImage { get; set; }
    }
    public class PserviceImage
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("PService")]
        public Guid PServiceId { get; set; }
        public String ImageId { get; set;}
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        [NotMapped]
        public HttpPostedFileBase imageUpload { get; set; }
        public PserviceImage()
        {
            ImageId = "~/Image/Product/product.png";
        }
        public virtual PService PService { get; set; }
    }
}
