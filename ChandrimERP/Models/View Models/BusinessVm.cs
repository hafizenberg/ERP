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
    public class AddBussinessVM
    {
        [Required]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [Required]
        [DisplayName("Warehouse Name")]
        public string WarehouseName { get; set; }
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
        [DisplayName("Company Logo"),Required]
        public string CompanyLogo { get; set; }
        public string UserId { get; set; }

        //End New Field........

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

        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        [NotMapped]
        public string ImageShow { get; set; }
        public AddBussinessVM(){
            CompanyLogo = "~/Image/company_logo/default.jpg";
        }
    }
    public class AddBranchVM
    {

        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Address"),DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Company")]
        public Guid CompanyId { get; set; }
    }
    public class AddWarehouseVM
    {

        [DisplayName("Warehouse Name")]
        public string WarehouseName { get; set; }
        [DisplayName("Address"),DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Company")]
        public Guid CompanyId { get; set; }
    }
    public class CompanyBranchWarehouse
    {
        public Company company { get; set; }
        public List<Branch> branch { get; set; }
        public List<Warehouse> warehouse { get; set; }
    }
}