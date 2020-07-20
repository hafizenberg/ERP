
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
    public class Customer
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CustomerId { get; set; }
        [DisplayName("Is Corporate")]
        public bool IsCorporate { get; set; }
        [DisplayName("Customer Company")]
        public string CompanyName { get;set;}
        [ DisplayName("Customer Code")]
        public int CustomerCode { get; set; }
        [DisplayName("Opening Blance")]
        public decimal? OpeningBlance { get; set; }
        public decimal? BlanceLimit { get; set; }
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
        [DisplayName("Web Address")]
        public string WebPage { get; set; }
        [ DisplayName("Country/Region")]
        public string Country { get; set; }
        [DisplayName("State/Province")]
        public string State { get; set; }
        [ DisplayName("City")]
        public string City { get; set; }
        [DisplayName("Address Line One"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }
        [DisplayName("Address Line Two"), DataType(DataType.MultilineText)]
        public string AddressLineTwo { get; set; }
        [DisplayName("Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [DisplayName("Notes"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public DateTime? OrderDate { get; set; }
        [DisplayName("Is locked")]
        public bool Islocked { get; set; }
        [DisplayName("Locked Date & Time")]
        public DateTime? LockedDateTime { get; set; }
        [DisplayName("Status Value")]
        public string StatusValue { get; set; }
        [DisplayName("National ID")]
        public string NationalId { get; set; }
        [DisplayName("TIN Number")]
        public string TinNumber { get; set; }
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
        [NotMapped]
        public HttpPostedFileBase imageUpload { get; set; }
        public Customer()
        {
            ImageUrl = "~/Image/Customer/Image/user.png";
        }
        //  public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<BankDetailsCustomer> BankDetailsCustomer { get; set; }
        public virtual ICollection<MobileBankingCustomer> MobileBankingCustomer { get; set; }
        public virtual ICollection<InternationalBankingCustomer> InternationalBankingCustomer { get; set; }
        public virtual ICollection<Branch_Customer> Branch_Customer { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<SalesOrder> SalesOrder { get; set; }
        public virtual ICollection<TailorOrder> TailorOrder { get; set; }
    }
}
