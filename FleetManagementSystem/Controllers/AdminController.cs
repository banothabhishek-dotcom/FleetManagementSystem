using System;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User_Details> _passwordHasher;

        public AdminController(ApplicationDbContext db, IPasswordHasher<User_Details> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optionally, redirect to login or home page
            return RedirectToAction("Login", "Customer");
        }

        public IActionResult AdminPage()
        {
            ViewBag.HideFooter = true;
            return View();
        }
        
    }
}
