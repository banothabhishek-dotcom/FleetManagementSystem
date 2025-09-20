using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CustomerController(ApplicationDbContext db)
        {
            _db = db;   
        }
        public IActionResult Login()
        {
            ViewBag.HideFooter = true;
            return View();
        }
        public IActionResult Registration()
        {
            ViewBag.HideFooter = true;
            return View();
        }
        [HttpPost]
        public IActionResult RegistrationDetails(User_Details model)
        {
            if (ModelState.IsValid)
            {
                _db.UserDetails.AddAsync(model);
                _db.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
        }
        public IActionResult CustomerPage()
        {
            ViewBag.HideFooter = false;
            return View();
        }

        public IActionResult CustomerHistory()
        {
            ViewBag.HideFooter = true;
            return View();
        }
        public IActionResult CustomerProfile()
        {
            ViewBag.HideFooter = true;
            return View();
        }
    }
}
