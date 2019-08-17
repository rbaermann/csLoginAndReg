using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using loginAndReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace loginAndReg.Controllers
{
    public class HomeController : Controller
    {
        private UserContext dbContext;

        public HomeController(UserContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpPost("register")]
        public RedirectToActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                    return RedirectToAction("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("InSession", newUser.UserId);
                return RedirectToAction("Success");
            }

                return RedirectToAction("Index");
        }

        [HttpGet("login")]
        public ViewResult Login()
        {
            return View("Login");
        }

        [HttpPost("logging")]
        public RedirectToActionResult CheckingUser(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);

                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Email does not excist");
                    return RedirectToAction("Login");
                }
                
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Wrong password");
                    return RedirectToAction("Login");
                }

                HttpContext.Session.SetInt32("InSession", userInDb.UserId);
                return RedirectToAction("Success");
            }

            return RedirectToAction("Login");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            if(HttpContext.Session.GetString("InSession") == null)
            {
                return RedirectToAction("Index");
            }
            return View("Success");
        }
    }
}
