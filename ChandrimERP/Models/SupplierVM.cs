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
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SupplierId { get; set; }
        [Required,DisplayName("Supplier Code")]
        public int SupplierCode { get; set; }
        [ DisplayName("Opening Blance")]
        public decimal OpeningBlance { get; set; }
        [Required, DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [Required, DisplayName("Job Title")]
        public string JobTitle { get; set; }
        [Required, DisplayName("First Name")]
        public string ContactFirstName { get; set; }
        [DisplayName("Last Name")]
        public string ContactLastName { get; set; }
        public Gender Genders { get; set; }
        [Required, DisplayName("Bussiness Phone")]
        public string BussinessPhone { get; set; }
        [DisplayName("Mobile Phone")]
        public string MobilePhone { get; set; }
        [DisplayName("Address Line1"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }
        [DisplayName("Address Line2"), DataType(DataType.MultilineText)]
        public string AddressLineTwo { get; set; }
        [DisplayName("National ID")]
        public string NationalId { get; set; }
        [DisplayName("TIN Number")]
        public string TinNumber { get; set; }
        [DisplayName("Fax Number")]
        public string FaxNumber { get; set; }

        [Required, DisplayName("Country/Region")]
        public string Country { get; set; }
        [DisplayName("State/Province")]
        public string State { get; set; }
        [Required, DisplayName("City")]
        public string City { get; set; }

        [DisplayName("Zip/PostalCode")]
        public string ZipOrPostalCode { get; set; }

        [DisplayName("Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        public Supplier()
        {
            ImageUrl = "~/Image/Supplier_Image/Image/user.png";
        }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Website"), DataType(DataType.Url)]
        public string Website { get; set; }

        [DisplayName("Notes"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }
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
        public virtual  ICollection<Product> Product { get; set; }
        public virtual ICollection<Branch_Supplier> Branch_Supplier { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<BankDetailsSupplier> BankDetailsSupplier { get; set; }
        public virtual ICollection<MobileBankingSupplier> MobileBankingSupplier { get; set; }
        public virtual ICollection<InternationalBankingSupplier> InternationalBankingSupplier { get; set; }
    }
}
