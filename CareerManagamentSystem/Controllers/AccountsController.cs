using CareerManagamentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CareerManagamentSystem.Controllers
{
    public class AccountsController : Controller
    {
        CareerSystemEntities1 db = new CareerSystemEntities1();
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Login(Users user)
        {
            
            var user1 = db.Users.FirstOrDefault(u => u.email == user.email && u.password == user.password);
            if (user1 != null)
            {
                //logindeki bilgileri panele tasiyoruz
                Session["userID"] = user1.UserID;   
                Session["username"] = user1.username;
                Session["email"] = user1.email;
                FormsAuthentication.SetAuthCookie(user1.username, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre";
                return View();
            }
        }
        public ActionResult SignIn()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Accounts");
        }

        [HttpGet]
        public ActionResult profile() // int userID
        {

            //var user = db.Users.FirstOrDefault(u => u.UserID == userID);
            //var employee = db.Employees.Where(e => e.EmployeeID == user.EmployeeID).FirstOrDefault();
            //// Birden fazla veri gonderme
            //ViewBag.User = user;
            //ViewBag.Employee = employee;
            
            return View();
        }


    }
}