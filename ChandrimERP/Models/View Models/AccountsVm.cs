using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandrimERP.Models
{
    public class JournalVm
    {
        [DataType(DataType.Date),Required]
        public DateTime Date { get; set; }
        [ Required]
        public decimal Amount { get; set; }
        [Display(Name ="Branch Name"), Required]
        public Guid BranchName { get; set; }
        [Display(Name = "Customer Name")]
        public Guid CustomerName { get; set; }
        [Display(Name = "Expenses")]
        public Guid Expenses { get; set; }
        [Display(Name = "Supplier Name")]
        public Guid SupplierName { get; set; }
        [Display(Name = "Payment Type")]
        public Guid PaymentType { get; set; }
        [DataType(DataType.MultilineText)]
        public string Narration { get; set; }

    }

    public class JournalVmDetails
    {
        //Input Field date, dropdownlist ledger name, Ledger No. , Credit Amount , Debit Amount. 
        public DateTime Date { get; set; }
        public string LedgerName { get; set; }
        public Guid LedgerNo { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Narration { get; set; }
    }
}
