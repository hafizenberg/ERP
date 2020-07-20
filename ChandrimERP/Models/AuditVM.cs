using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChandrimERP.Models
{
    public class userLogData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime AccessDate { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogOutTime { get; set; }
        public decimal TotalTime { get; set; }
        public long? IPAddress { get; set; }
        public string MacAddress { get; set; }
    }
}