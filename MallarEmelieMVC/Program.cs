using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MallarEmelieMVC.Data;
using MallarEmelieMVC.Areas.Identity.Data;
using MallarEmelieMVC.Areas.Identity.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("IdentityUserContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityUserContextConnection' not found."); ;

//Appdbcontext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<IdentityUserContext>(options => options.UseSqlServer(connectionString));


// Add Identity services to pass two types of identity
builder.Services.AddIdentity<IdentityUserTable, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<IdentityUserContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
