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
    public class FixedAsset
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Asset Code")]
        public string AssetCode { get; set; }
        [DisplayName("Asset Value")]
        public decimal AssetValue { get; set; }
        [DisplayName("Asset Life")]
        public decimal AssetLife { get; set; }
        [DisplayName("Depreciation Rate")]
        public decimal DepreciationRate { get; set; }
        [DisplayName("Depreciation Effect Form"), DataType(DataType.Date)]
        public DateTime DepreciationEffectFrom { get; set; }
        [DisplayName("Accumulate Depreciation")]
        public decimal AccumulateDepriciation { get; set; }
        [DisplayName("Written Down Value")]
        public decimal WrittenDownValue { get; set; }
        [DisplayName("Salvage Value")]
        public decimal SalvageValue { get; set; }
        public string Node { get; set; }
        [DisplayName("Supplier")]
        public Guid? SupplierId { get; set; }
        [DisplayName("Purchase Date"),DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }
        [DisplayName("Warrenty Details")]
        public string WarrentyDetails { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
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

        [DisplayName("Branch"),ForeignKey("Branch")]
        public Guid? BranchId { get; set; }
        [DisplayName("Company"),ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [DisplayName("Warehouse"),ForeignKey("Warehouse")]
        public Guid? WarehouseId { get; set; }
        public virtual Company Company { get; set; }
        
        public virtual Branch Branch { get; set; }
        public virtual Warehouse Warehouse { get; set; }

        [DisplayName("Fixed Asset Category"),ForeignKey("FixedAssetCategory")]
        public Guid FixedAssetCategoryId { get; set; }
        public virtual FixedAssetCategory FixedAssetCategory { get; set; }
    }

    public class FixedAssetCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Ledger"),ForeignKey("Ledger")]
        public Guid LedgerId { get; set; }
        public virtual Ledger Ledger { get; set; }

        [DisplayName("Company"),ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<FixedAsset> FixedAssets { get; set; } 
    }
}