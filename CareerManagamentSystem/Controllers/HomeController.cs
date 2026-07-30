using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CareerManagamentSystem.Models;
namespace CareerManagamentSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CareerSystemEntities1 db = new CareerSystemEntities1();
            List<Employees> employees = db.Employees.ToList();
            return View(employees);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}