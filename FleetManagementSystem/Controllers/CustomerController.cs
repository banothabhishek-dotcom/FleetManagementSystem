using System.Net.Http;
using System.Net.Http.Json;
using FleetManagementSystem.Data;

//using FleetManagementSystem.Models;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CustomerController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string BaseUrl = "http://localhost:5259/api/Account";
    private readonly ApplicationDbContext _db;
    public CustomerController(ApplicationDbContext db, IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
    }


    public IActionResult Registration()
    {
        ViewBag.HideFooter = true;
        return View();
    }
    public IActionResult CustomerPage()
    {
        ViewBag.HideFooter = false;
        return View("~/Views/Customer/CustomerPage.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Registration(RegisterDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync($"{BaseUrl}/register", dto);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Registration failed: {error}");
        return View(dto);
    }

  
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync($"{BaseUrl}/login", dto);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid credentials");
            ViewBag.HideFooter = true;
            return View(dto);
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        HttpContext.Session.SetString("JwtToken", result.Token);
        HttpContext.Session.SetString("UserEmail", dto.Email);

        // 🔍 Fetch user from database to get role
        var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "User not found");
            ViewBag.HideFooter = true;
            return View(dto);
        }

        //stroing role on server
        HttpContext.Session.SetString("UserRole", user.Role);
        HttpContext.Session.SetString("DriverName", user.FirstName + " " + user.LastName);

        // 🔀 Redirect based on role
        if (user.Role == "Customer")
        {
            return RedirectToAction("CustomerPage","Customer");
        }
        else if (user.Role == "Driver")
        {
            return RedirectToAction("DriverPage","Driver");
        }
        else if (user.Role == "Admin")
        {
            return RedirectToAction("AdminPage","Admin");
        }

        // Default fallback
        return RedirectToAction("Login");
    }

    public IActionResult Login()
    {
        ViewBag.HideFooter = true;
        return View();
    }
    [HttpPost]
    public IActionResult Logout()
    {
        // Clear all session data
        HttpContext.Session.Clear();

        // Optionally, redirect to login or home page
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> CustomerHistory()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Customer");
        }

        var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return RedirectToAction("Login", "Customer");
        }

        var phone = user.PhoneNumber?.Trim().ToLower();
       
        var history = _db.Trips
            .Where(t => t.VehicleId != null && t.PhoneNumber.Trim().ToLower() == phone)
            .OrderByDescending(t => t.BookingTime)
            .ToList();

        ViewBag.HideFooter = true; // ✅ You can still set this here

        return View("~/Views/Customer/CustomerHistory.cshtml", history);
    }

    public async Task<IActionResult> CustomerProfile()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login");
        }

        var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        return View("~/Views/Customer/CustomerProfile.cshtml", user);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(User_Details updatedUser)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

        var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return RedirectToAction("Login");

        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.PhoneNumber = updatedUser.PhoneNumber;

        _db.UserDetails.Update(user);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Profile details updated!";
        return RedirectToAction("CustomerProfile");
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string NewPassword, string ConfirmPassword)
    {
        if (NewPassword != ConfirmPassword)
        {
            TempData["ErrorMessage"] = "Passwords do not match.";
            return RedirectToAction("CustomerProfile");
        }

        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

        var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return RedirectToAction("Login");

        var passwordHasher = new PasswordHasher<User_Details>();
        user.Password = passwordHasher.HashPassword(user, NewPassword);

        _db.UserDetails.Update(user);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Password changed successfully!";
        return RedirectToAction("CustomerProfile");
    }


    public IActionResult AdminPage()
    {
        return View("~/Views/Admin/AdminPage.cshtml");
    }
    public IActionResult DriverPage()
    {
        return View("~/Views/Driver/DriverPage.cshtml");
    }

}
