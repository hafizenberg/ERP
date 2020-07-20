using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ChandrimERP.Models
{
    public class EmployeeVm
    {
        [Required(), DisplayName("Employee Code")]
        public int EmployeeCode { get; set; }
        public Guid EmployeeType { get; set; }
        [DisplayName("Email Address"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(), DisplayName("Job Title")]
        public string JobTitle { get; set; }

        [DisplayName("Bussiness Phone"), StringLength(20, MinimumLength = 6)]
        public string BussinessPhone { get; set; }
        [DisplayName("National ID")]
        public string NationalId { get; set; }
        [DisplayName("TIN Number")]
        public string TinNumber { get; set; }

        [DisplayName("Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        [Required(), DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(), DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(), DisplayName("Date Of Birth"), DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [DisplayName("Branch Name")]
        public Guid BranchId { get; set; }
        [Required()]
        public Gender Genders { get; set; }

        [DisplayName("Blood Group")]
        public BloodGroup BloodGroup { get; set; }

        [DisplayName("Home Phone"), StringLength(20, MinimumLength = 6)]
        public string HomePhone { get; set; }

        [Required(), DisplayName("Mobile Phone"),StringLength(20,MinimumLength = 6)]
        public string MobilePhone { get; set; }

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

        [DisplayName("Zip/Postal Code")]
        public string ZipOrPostalCode { get; set; }

        [DisplayName("Employee Joining Date"), DataType(DataType.Date)]
        public DateTime EmployeeJoiningDate { get; set; }
        [DisplayName("Salary Types")]
        public EmpSalaryTypes SalaryTypes { get; set; }
        [DisplayName("Salary")]
        public Decimal EmpBasicSalary { get; set; }
        [DisplayName("Overtime Allow Or Not")]
        public bool IsOvertime { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        [NotMapped]
        public string ImageShow { get; set; }
        public EmployeeVm()
        {
            ImageUrl = "~/Image/Employee_image/user.png";
        }
    }
}