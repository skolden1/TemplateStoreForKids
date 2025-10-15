using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MallarEmelieMVC.Data;
using MallarEmelieMVC.Areas.Identity.Data;
using MallarEmelieMVC.Areas.Identity.Models;
using MallarEmelieMVC.Data.Model;
using MallarEmelieMVC.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("IdentityUserContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityUserContextConnection' not found."); ;

//Appdbcontext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<IdentityUserContext>(options => options.UseSqlServer(connectionString));

//Email service
var email = builder.Configuration["EmailSettings:Email"];
var password = builder.Configuration["EmailSettings:Password"];

builder.Services.AddRazorTemplating();
builder.Services.AddScoped(_ => new EmailService(email, password));

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

//seed för kategorier
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!dbContext.Categories.Any())
    {
        dbContext.Categories.AddRange(
            new Category { CategoryName = "Matematik" },
            new Category { CategoryName = "Språk" },
            new Category { CategoryName = "Naturvetenskap" },
            new Category { CategoryName = "Konst" },
            new Category { CategoryName = "TAKK/AKK Kakor" }
        );

        dbContext.SaveChanges();
    }
}



// seed admin user och roll
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserTable>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    

    var configuration = app.Services.GetRequiredService<IConfiguration>();
    string adminEmail = configuration["AdminUser:Email"];
    string adminPassword = configuration["AdminUser:Password"];

    // om admin roll ej finns skapa den
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newUser = new IdentityUserTable
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "Admin",
            DateJoined = DateTime.Now
        };

        var createUserResult = await userManager.CreateAsync(newUser, adminPassword);
        if (!createUserResult.Succeeded)
        {
            throw new Exception("Det gick inte att skapa en admin användare!");
        }
        adminUser = newUser;
    }

    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
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
