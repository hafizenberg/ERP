using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandrimERP.Models
{
    public enum EmpSalaryTypes { Hourly, Daily, Weekly, Monthly, Yearly, Production }
    public enum Gender { Male, Female }
    public enum BusinessType {
        Trading,
        Wholesale,
        Inventory,
        Distribution,
        [Display(Name = "Departmental Store")]
        DepartmentalStore
    }
    public enum BloodGroup
    {
        [Display(Name = "A+")]
        APositive,
        [Display(Name = "A-")]
        ANegative,
        [Display(Name = "B+")]
        BPositive,
        [Display(Name = "B-")]
        BNegative,
        [Display(Name = "AB+")]
        AbPositive,
        [Display(Name = "AB-")]
        AbNegative,
        [Display(Name = "O+")]
        OPositive,
        [Display(Name = "O-")]
        ONegative
    }

      public enum FinancialYear
      {
        [Display(Name = "Jan-Dec")]
        XXXX_12_31,
        [Display(Name = "Feb-Jan")]
        XXXX_1_31,
        [Display(Name = "March-Feb")]
        XXXX_2_28,
        [Display(Name = "April-March")]
        XXXX_3_31,
        [Display(Name = "May-April")]
        XXXX_4_30,
        [Display(Name = "Jun-May")]
        XXXX_5_31,
        [Display(Name = "July-Jun")]
        XXXX_6_30,
        [Display(Name = "Aug-July")]
        XXXX_7_31,
        [Display(Name = "Sep-Aug")]
        XXXX_8_31,
        [Display(Name = "Oct-Sep")]
        XXXX_9_30,
        [Display(Name = "Nov-Oct")]
        XXXX_10_31,
        [Display(Name = "Dec-Nov")]
        XXXX_11_30
      }


    public enum Shelha
    {
        Yes,
        No
    }

    public enum IbadMohra
    {
        [Display(Name="Zipper")]
        Zippper,
        [Display(Name="Button")]
        Button,
        [Display(Name="Rancr Elas")]
        RancrElas
    }
}
