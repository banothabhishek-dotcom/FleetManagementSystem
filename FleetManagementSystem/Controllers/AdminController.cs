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
       

        public IActionResult AdminPage()
        {
            ViewBag.HideFooter = true;
            return View();
        }
        
    }
}
