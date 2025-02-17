using Microsoft.EntityFrameworkCore;

using RunGroopWebApp.Data;
using Microsoft.Extensions.DependencyInjection;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Repository;
using RunGroopWebApp.Helpers;
using RunGroopWebApp.Services;

using Microsoft.OpenApi.Models;
using RunGroopWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IRaceRepository, RaceRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RunGroops",
        Version = "v1",
        Description = "API Documentation for Your Project",
        //Contact = new OpenApiContact
        //{
        //    Name = "Your Name",
        //    Email = "your.email@example.com",
        //    Url = new Uri("https://yourwebsite.com")
        //}
    });
});

builder.Logging.ClearProviders(); // Remove all default providers
builder.Logging.AddConsole();    // Add the console provider (or any supported provider)
var isWindows = OperatingSystem.IsWindows();
builder.Logging.ClearProviders();

if (isWindows)
{
    builder.Logging.AddEventLog(); // Only add EventLog on Windows
}
builder.Logging.AddConsole();

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
     await Seed.SeedUsersAndRolesAsync(app); // Uncomment if needed
//    Seed.SeedData(app);
}



// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        Seed.SeedData(services);
        Console.WriteLine("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();

//    // Enable Swagger in Development mode
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Title v1");
//        c.RoutePrefix = string.Empty; // Makes Swagger UI available at the root URL
//    });
//}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
