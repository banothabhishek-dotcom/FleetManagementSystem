using System.Net.Http;
using System.Net.Http.Json;
using FleetManagementSystem.Data;

//using FleetManagementSystem.Models;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CustomerController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string BaseUrl = "https://localhost:7114/api/Account";
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
            return View(dto);
        }

        HttpContext.Session.SetString("UserRole", user.Role);
        HttpContext.Session.SetString("DriverName", user.FirstName + " " + user.LastName);

        // 🔀 Redirect based on role
        if (user.Role == "Customer")
        {
            return RedirectToAction("CustomerPage");
        }
        else if (user.Role == "Driver")
        {
            return RedirectToAction("DriverPage");
        }
        else if (user.Role == "Admin")
        {
            return RedirectToAction("AdminPage");
        }

        // Default fallback
        return RedirectToAction("Login");
    }

    public IActionResult Login()
    {
        ViewBag.HideFooter = true;
        return View();
    }

   public IActionResult CustomerHistory()
    {
        return View("~/Views/Customer/CustomerHistory.cshtml");
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
    public async Task<IActionResult> UpdateProfile(User_Details model)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return RedirectToAction("Login");

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        user.Password = model.Password;

        await _db.SaveChangesAsync();
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
