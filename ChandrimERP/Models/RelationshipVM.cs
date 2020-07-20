
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChandrimERP.Models
{
    public class ApplicationUser_Company
    {

        [Key]
        [Column(Order = 1)]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUser_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Company")]
        public Guid Company_Id { get; set; }

        public virtual Company Company { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
    public class ApplicationUser_Branch
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUser_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class Branch_Supplier
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Supplier")]
        public Guid Supplier_Id { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual Branch Branch { get; set; }
    }
    public class Branch_Employee
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Employee")]
        public Guid Employee_Id { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Branch Branch { get; set; }

    }
    public class Branch_SalesAgent
    {

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("SalesAgent")]
        public Guid SalesAgent_Id { get; set; }

        public virtual Employee SalesAgent { get; set; }
        public virtual Branch Branch { get; set; }
    }
    public class Branch_Product
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Product")]
        public Guid Product_Id { get; set; }

        public virtual Product Product { get; set; }
        public virtual Branch Branch { get; set; }

    }
    public class Branch_Customer
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Customer")]
        public Guid Customer_Id { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Branch Branch { get; set; }

    }
    public class Branch_Document
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Document")]
        public Guid Document_Id { get; set; }

        public virtual Document Document { get; set; }
        public virtual Branch Branch { get; set; }

    }

    public class Branch_Warehouse
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Warehouse")]
        public Guid Warehouse_Id { get; set; }

        public virtual Warehouse Warehouse { get; set; }
        public virtual Branch Branch { get; set; }

    }
    public class Branch_PService
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("PService")]
        public Guid PService_Id { get; set; }

        public virtual PService PService { get; set; }
        public virtual Branch Branch { get; set; }
    }
    public class Branch_Tailor
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Branch")]
        public Guid Branch_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Tailor")]
        public Guid Tailor_Id { get; set; }

        public virtual Tailor Tailor { get; set; }
        public virtual Branch Branch { get; set; }
    }
}