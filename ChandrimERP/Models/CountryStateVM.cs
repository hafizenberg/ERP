using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandrimERP.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        [DisplayName("Sort Name")]
        public string SortName { get; set; }
        [DisplayName("Country Name")]
        public string CountryName { get; set; }
        [DisplayName("Phone Code")]
        public int PhoneCode { get; set; }
        public virtual ICollection<State> States { get; set; }

    }

    public class State
    {
        public int StateId { get; set; }
        [DisplayName("State Name")]
        public string StateName { get; set; }
        public int? CountryId { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<City> Cities { get; set; }

    }

    public class City
    {
        public int CityId { get; set; }
        [DisplayName("City Name")]
        public string CityName { get; set; }
        public int? StateId { get; set; }
        public virtual State State { get; set; }
    }
}
