using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChandrimERP.Models;

namespace ChandrimERP.Controllers
{
    [Authorize]
    public class CountryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult GetCountryList()
        {
            List<Country> countries = db.Country.ToList();
            ViewBag.CountryOption = new SelectList(countries, "CountryName", "CountryName");
            return PartialView("CountryOptionPartial");
        }

        public ActionResult GetStateList(string CountryId)
        {

            List<State> stateList = db.State.Where(x => x.Country.CountryName == CountryId).ToList();

            ViewBag.StateOptions = new SelectList(stateList, "StateName", "StateName");

            return PartialView("StateOptionPartial");

        }
        public ActionResult GetCityList(string StateId)
        {

            List<City> CityList = db.City.Where(x =>x.State.StateName == StateId).ToList();

            ViewBag.CityOptions = new SelectList(CityList, "CityName", "CityName");

            return PartialView("CityOptionPartial");
        }
    }
}