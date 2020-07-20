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
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required(), DisplayName("Employee Code")]
        public int EmployeeCode { get; set; }
        [Required(), DisplayName("Email Address"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required(), DisplayName("Job Title")]
        public string JobTitle { get; set; }
        [DisplayName("Bussiness Phone"), StringLength(20, MinimumLength = 6)]
        public string BussinessPhone { get; set; }
        [DisplayName("Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        [DisplayName("Is locked")]
        public bool? Islocked { get; set; }
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
        public Employee()
        {
            ImageUrl = "~/Image/Employee/user.png";
        }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("EmployeeType")]
        public Guid EmpTypeId { get; set; }
        public virtual ICollection<EmployeePersionalInfo> EmployeePersionalInfo { get; set; }
        public virtual ICollection<EmployeePropInfo> EmployeePropInfo { get; set; }
        public virtual ICollection<EmployeeNomineeInfo> EmployeeNomineeInfo { get; set; }
        public virtual ICollection<EmployeePayrollPolicy> EmployeePayrollPolicy { get; set; }
        public virtual ICollection<Branch_Employee> Branch_Employee { get; set; }
        public virtual Company Company { get; set; }
        public virtual EmployeeType EmployeeType { get; set; }
        public virtual ICollection<BankDetailsEmployee> BankDetailsEmployee { get; set; }
        public virtual ICollection<MobileBankingEmployee> MobileBankingEmployee { get; set; }
        public virtual ICollection<InternationalBankingEmployee> InternationalBankingEmployee { get; set; }
        public virtual ICollection<EmpAttendanceInTime> EmpAttendanceInTime { get; set; }
        public virtual ICollection<EmpAttendanceOutTime> EmpAttendanceOutTime { get; set; }
    }

    public class EmployeeType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual Company Company { get; set; }
    }
    public class EmployeePersionalInfo
    {
        public int Id { get; set; }
        [Required(), DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required(), DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required(), DisplayName("Date Of Birth"), DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }
        [Required()]
        public Gender Genders { get; set; }
        [DisplayName("Blood Group")]
        public BloodGroup BloodGroup { get; set; }
        [DisplayName("Home Phone"), StringLength(20, MinimumLength = 6)]
        public string HomePhone { get; set; }
        [Required(), DisplayName("Mobile Phone"), StringLength(20, MinimumLength = 6)]
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
        [DisplayName("National ID")]
        public string NationalId { get; set; }
        [DisplayName("TIN Number")]
        public string TinNumber { get; set; }
        [ForeignKey("Employees")]
        public Guid EmployeesId { get; set; }
        public virtual Employee Employees { get; set; }
    }
    public class EmployeePropInfo
    {
        public int Id { get; set; }
        [DisplayName("Employee Joining Date"),DataType(DataType.Date)]
        public DateTime EmployeeJoiningDate { get; set; }
        [DisplayName("Salary Types")]
        public EmpSalaryTypes SalaryTypes { get; set; }
        [DisplayName("Salary")]
        public Decimal EmpBasicSalary { get; set; }
        [DisplayName("Overtime Allow Or Not")]
        public bool IsOvertime { get; set; }
        [ForeignKey("Employees")]
        public Guid EmployeesId { get; set; }
        public virtual Employee Employees { get; set; }
    }
    public class EmployeeNomineeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required(), DisplayName("Nominee Name")]
        public string NomineeName { get; set; }
        [DisplayName("Nominee Details"), DataType(DataType.MultilineText)]
        public string NomineeDetails { get; set; }
        [DisplayName("Signature"), DataType(DataType.ImageUrl)]
        public string Signature { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required, DisplayName("Country/Region")]
        public string Country { get; set; }
        [DisplayName("State/Province")]
        public string State { get; set; }
        [Required, DisplayName("City")]
        public string City { get; set; }
        [DisplayName("Image"), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        [DisplayName("Address Line1"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }
        [DisplayName("Address Line2"), DataType(DataType.MultilineText)]
        public string AddressLineTwo { get; set; }
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
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        [NotMapped]
        public HttpPostedFileBase SignUpload { get; set; }
        
        public EmployeeNomineeInfo()
        {
            ImageUrl = "~/Image/EmpNominee/Image/user.png";
            Signature = "~/Image/EmpNominee/Signature/signature.png";
        }
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }
        public virtual Employee Employee { get; set; }
    }

    public class EmployeePayrollPolicy
    {
        public int Id { get; set; }
        [Required, DisplayName("Payroll Policy Name")]
        public string PpolicyName { get; set; }
        [Required, DisplayName("Pay")]
        public string Ppay { get; set; }
        [Required, DisplayName("Paying Status")]
        public string Pstatus { get; set; }
        [Required, DisplayName("Paying Account")]
        public string Paccount { get; set; }
        [ForeignKey("Employees")]
        public Guid EmployeesId { get; set; }
        public virtual Employee Employees { get; set; }
    }
    public class SalesAgent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual ICollection<SalesOrder> SalesOrder { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<TailorOrder> TailorOrder { get; set; }
    }

    public class EmpAttendance
    {
        public Guid Id { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public Decimal TotalHour { get; set; }
        public Decimal OverTime { get; set; }
        public Decimal WorkingHour { get; set; }
    }

    public class EmpAttendanceInTime
    {
        public Guid Id { get; set; }
        [DisplayName("In Time")]
        public DateTime InTime { get; set; }
        [ForeignKey("Employees")]
        public Guid EmployeesId { get; set; }
        public virtual Employee Employees { get; set; }

    }
    public class EmpAttendanceOutTime
    {
        public Guid Id { get; set; }
        [DisplayName("Out Time")]
        public DateTime OutTime { get; set; }
        [ForeignKey("Employees")]
        public Guid EmployeesId { get; set; }
        public virtual Employee Employees { get; set; }
    }
    public class OverTimePolicy
    {
        public int Id { get; set; }
        [DisplayName("Policy name")]
        public string PolicyName { get; set; }
        public int OverTimeRate { get; set; }
    }

    public class EmpDepartment
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
    }

    public class PaySlips
    {
        public Guid Id { get; set; }
        public Guid EmpDepartmentId { get; set; }
        public Decimal AmountPaid { get; set; }
        public string DescriptionOfNeed { get; set; }
        [ForeignKey("Employees")]
        public Guid ReceivedBy { get; set; }       
        [ForeignKey("Employees")]
        public Guid? ApprovedBy { get; set; }
        [ForeignKey("Employees")]
        public Guid? VerifiedBy { get; set; }
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
        public virtual Employee Employees { get; set; }
    }

    public class EmpLeave
    {
        public Guid Id { get; set; }
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
        public DateTime LeaveFromDate { get; set; }
        public DateTime LeaveToDate { get; set; }
        [ForeignKey("Employees")]
        public Guid ReceivedBy { get; set; }
        [ForeignKey("Employees")]
        public Guid? ApprovedBy { get; set; }
        [ForeignKey("Employees")]
        public Guid? VerifiedBy { get; set; }
        public virtual Employee Employees { get; set; }
    }
}
