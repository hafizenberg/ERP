using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandrimERP.Models
{
    public class BankDetailsEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for bank
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Check name")]
        public string CheckName { get; set; }
        [DisplayName("MICR Code")]
        public int MicrCode { get; set; }

        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

    }
    public class BankDetailsCustomer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for bank
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Check name")]
        public string CheckName { get; set; }
        [DisplayName("MICR Code")]
        public int MicrCode { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

    }
    public class BankDetailsSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for bank
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Check name")]
        public string CheckName { get; set; }
        [DisplayName("MICR Code")]
        public int MicrCode { get; set; }


        [ForeignKey("Supplier")]
        public Guid SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

    }
    public class BankDetailsBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for bank
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Check name")]
        public string CheckName { get; set; }
        [DisplayName("MICR Code")]
        public int MicrCode { get; set; }
        
        [ForeignKey("Branch")]
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; }

    }
    public class BankDetailsCompany
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for bank
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Check name")]
        public string CheckName { get; set; }
        [DisplayName("MICR Code")]
        public int MicrCode { get; set; }

        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }

    }
    public class MobileBankingEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for Mobile banking
        [DisplayName("Mobile Banking Type")]
        public string MobileBankingType { get; set; }
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
       
    }
    public class MobileBankingCustomer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for Mobile banking
        [DisplayName("Mobile Banking Type")]
        public string MobileBankingType { get; set; }
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
       
    }
    public class MobileBankingSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for Mobile banking
        [DisplayName("Mobile Banking Type")]
        public string MobileBankingType { get; set; }
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }
        
        [ForeignKey("Supplier")]
        public Guid SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        
    }
    public class MobileBankingBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for Mobile banking
        [DisplayName("Mobile Banking Type")]
        public string MobileBankingType { get; set; }
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }
     
        [ForeignKey("Branch")]
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; }
       
    }
    public class MobileBankingCompany
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //payment methods for Mobile banking
        [DisplayName("Mobile Banking Type")]
        public string MobileBankingType { get; set; }
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }
        
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
    public class InternationalBankingEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //Payment method for International Gateway
        [DisplayName("Method Type")]
        public string MethodType { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
       
    }
    public class InternationalBankingCustomer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //Payment method for International Gateway
        [DisplayName("Method Type")]
        public string MethodType { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
       
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
       
    }
    public class InternationalBankingSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //Payment method for International Gateway
        [DisplayName("Method Type")]
        public string MethodType { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
       
        [ForeignKey("Supplier")]
        public Guid SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
       
    }
    public class InternationalBankingBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //Payment method for International Gateway
        [DisplayName("Method Type")]
        public string MethodType { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
       
        [ForeignKey("Branch")]
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        
    }
    public class InternationalBankingCompany
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //Payment method for International Gateway
        [DisplayName("Method Type")]
        public string MethodType { get; set; }
        [DisplayName("Email"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }

    public class Currency
    {
        //USD, US Dollar,  United States,  $#,###.##,  2,  float,  dollar, cents,  100 cents = dollar
        [DisplayName("Currency Code")]
        public string CurrencyCode { get; set; }
        [DisplayName("Currency Name")]
        public string CurrencyName { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("Format")]
        public string Format { get; set; }
        [DisplayName("Decimal Point")]
        public string DecimalPoints { get; set; }
        [DisplayName("Currency Regime")]
        public string CurrencyRegime { get; set; }
        [DisplayName("Major Unit")]
        public string MajorUnit { get; set; }
        [DisplayName("Minor Unit")]
        public string MinorUnit { get; set; }
        [DisplayName("Equivalence")]
        public string Equivalence { get; set; }
    }
}
