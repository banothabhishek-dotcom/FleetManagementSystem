﻿using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
