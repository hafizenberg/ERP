using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ChandrimERP.Models
{
    public class InventoryIncomming
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public int? InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        public int? ItemQuantity { get; set; }
        public decimal? ItemUnitCost { get; set; }
        public string BatchOrSerial { get; set; }
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
        [DisplayName("Status")]
        public bool Status { get; set; }
        [ForeignKey("Company")]
        public Guid? CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid? WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }

    public class InventoryOutGoing
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public int? InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        public int ItemQuantity { get; set; }
        public decimal? ItemUnitCost { get; set; }
        public string BatchOrSerial { get; set; }
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
        [DisplayName("Status")]
        public bool Status { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }

    public class Inventory
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public int BalanceQuantity { get; set; }
        public int? AleartLavel { get; set; }
        public decimal? ItemMRP { get; set; }
        public decimal? ItemUnitCost { get; set; }
        public decimal? ItemEditionalCost { get; set; }
        public decimal? ItemTotalCost { get; set; }
        public decimal? ItemAvrageCost { get; set; }
        public DateTime? UpdateDate { get; set; }
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
        [DisplayName("Status")]
        public bool Status { get; set; }
        [ForeignKey("Product")]
        public Guid ProId { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid? WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Product Product { get; set; }

    }
    public class InventoryMovement
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? AleartLavel { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }

        public string InvoiceType { get; set; }
        public int InvoiceNo { get; set; }

        public int? InQuantity { get; set; }
        public int? OutQuantity { get; set; }
        public string BatchOrSerial { get; set; }
        public string Note { get; set;}

        public string UserName { get; set; }


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
        [ForeignKey("Product")]
        public Guid ProId { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid? WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual Product Product { get; set; }
    }
    public class InventoryValueChange
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public decimal Current_unit_price { get; set; }
        public decimal Change_unit_price { get; set; }
        public decimal Current_MRP_price { get; set; }
        public decimal Change_MRP_price { get; set; }

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
        [DisplayName("Status")]
        public bool Status { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
    public class InventoryUpcomming
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public int InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        public int ItemQuantity { get; set; }
        public decimal? ItemUnitCost { get; set; }
        public string BatchOrSerial { get; set; }
        public DateTime? ExpectedDate { get; set; }
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
        [DisplayName("Status")]
        public bool Status { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
    public class InventoryUpgoing
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public int InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        public int ItemQuantity { get; set; }
        public decimal? ItemUnitCost { get; set; }
        public string BatchOrSerial { get; set; }
        public DateTime? ExpectedDate { get; set; }
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
        [DisplayName("Status")]
        public bool Status { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
}

    public class InventoryExpiary
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCode { get; set; }
        public string BatchOrSerial { get; set; }
        public int BalanceQuantity { get; set; }
        [DisplayName("Product Manufactur Date"), DataType(DataType.Date)]
        public DateTime ProductManufactureDate { get; set; }

        [DisplayName("Product Expire Date"), DataType(DataType.Date)]
        public DateTime ProductExpireDate { get; set; }
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
        [DisplayName("Status")]
        public bool Status { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("Warehouse")]
        public Guid WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
