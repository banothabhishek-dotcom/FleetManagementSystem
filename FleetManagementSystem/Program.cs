using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient();
builder.Services.AddSession();
builder.Services.AddScoped<IPasswordHasher<User_Details>, PasswordHasher<User_Details>>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();
async Task SeedAdminUserAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User_Details>>();

    var existingAdmin = await db.UserDetails.FirstOrDefaultAsync(u => u.Email == "admin@fleet.com");
    if (existingAdmin == null)
    {
        var admin = new User_Details
        {
            FirstName = "Admin",
            LastName = "User",
            PhoneNumber = "9999999999",
            Email = "admin@fleet.com",
            Role = "Admin",
            Password = hasher.HashPassword(null, "Admin@123")
        };

        await db.UserDetails.AddAsync(admin);
        await db.SaveChangesAsync();
    }
}


// ✅ Call the seeding method
SeedAdminUserAsync(app).GetAwaiter().GetResult();


app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();


app.UseAuthorization();

app.UseStaticFiles();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Landing}/{id?}")
    .WithStaticAssets();


app.Run();
