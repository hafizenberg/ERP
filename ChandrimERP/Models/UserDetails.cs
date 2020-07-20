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
    public class UserDetails
    {
        [Key,ForeignKey("ApplicationUser")]
        public  string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Genders { get; set; }
        public string Type { get; set; }
        public int? RetryAttempt { get; set; }
        public bool? Status { get; set; }
        public bool ? Islocked { get; set; }
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

        [DisplayName("Country/Region")]
        public string Country { get; set; }
        [DisplayName("State/Province")]
        public string State { get; set; }
        [ DisplayName("City")]
        public string City { get; set; }

        [DisplayName("Address Line1"), DataType(DataType.MultilineText)]
        public string AddressLineOne { get; set; }
        [DisplayName("Address Line2"), DataType(DataType.MultilineText)]
        public string AddressLineTwo { get; set; }
        [DisplayName("Photo"), DataType(DataType.ImageUrl)]
        public string PhotosUrl { get; set; }
        [DisplayName("Notes"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        public UserDetails()
        {
            PhotosUrl = "/Image/user_logo/user.png";
        }
    }
}