using ChandrimERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChandrimERP.Controllers
{

    public class StudentController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult createStudent(Student std5)
        {
            context.Student.Add(std5);
            context.SaveChanges();
            string message = "SUCCESS";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }
        public JsonResult getStudent(string id)
        {
            List<Student> students = new List<Student>();
            students = context.Student.ToList();
            return Json(students, JsonRequestBehavior.AllowGet);
        }
        // POST: OrderDetails/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStudent(int id)
        {
            Student student = context.Student.Find(id);
            context.Student.Remove(student);
            context.SaveChanges();
            string message = "SUCCESS";
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }
        //public JsonResult Delete(int ID)
        //{
        //    return Json(student.Delete(ID), JsonRequestBehavior.AllowGet);
        //}
    }
}