using ChandrimERP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ChandrimERP.Controllers
{
    public class ImageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Image
        public ActionResult Index()
        {
            Image img = new Image();
            return View(img);
        }
        [HttpPost]
        public ActionResult Index(Image ImageModel, HttpPostedFileBase files)
        {
            string massage = "No Data";
            if (files !=null)
            {
                string fileName = Path.GetFileNameWithoutExtension(files.FileName);
                string extension = Path.GetExtension(files.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                ImageModel.ImagePath = "~/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                files.SaveAs(fileName);
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Image.Add(ImageModel);
                    db.SaveChanges();
                    massage = "Data Upload Seccessfully";
                }
                ModelState.Clear();

            }
            else
            {
                massage = "Data Upload Faild";
            }
            

            ViewBag.Message = massage;
            return RedirectToAction("Index");
        }
    }
}