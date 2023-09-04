using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentalmandu.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RentalmanduContextConnection") ?? throw new InvalidOperationException("Connection string 'RentalmanduContextConnection' not found.");

builder.Services.AddDbContext<HajurKoCarRentalDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<HajurKoCarRentalUser>(options => options.SignIn.RequireConfirmedAccount = false).AddDefaultTokenProviders().AddRoles<IdentityRole>().AddEntityFrameworkStores<HajurKoCarRentalDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
