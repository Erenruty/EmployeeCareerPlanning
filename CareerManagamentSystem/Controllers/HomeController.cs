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

        

       
    }
}