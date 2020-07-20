using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ChandrimERP.Models
{
    public class HelloTestVm
    {
        public List<Customer> Customers { get; set; }
        public Product Products { get; set; }
        public List<Supplier> Suppliers { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
