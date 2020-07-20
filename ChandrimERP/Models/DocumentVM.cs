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
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required, DisplayName("Document Code")]
        public string DocumentCode { get; set; }
        [Required, DisplayName("Document Name")]
        public string DocumentName { get; set; }
        [DisplayName("Document Details")]
        public string DocumentDetails { get; set; }
        public string RefType { get; set; }
        public Guid RefId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EXPDate { get; set; }
        public DateTime? UpdateDate { get; set; }
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
        [Display(Name = "Browse File")]
        [NotMapped]
        public HttpPostedFileBase[] files { get; set; }
        [DisplayName("Company"), ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Branch_Document> Branch_Document { get; set; }
        public virtual ICollection<Image> Images { get; set; }


        [NotMapped]
        public Guid NullId { get; set; }
      

    }
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public string Title { get; set; }

        [DisplayName("Document Path")]
        public string DocumentPath { get; set; }
        public string DocType { get; set; }
        public string RefId { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
