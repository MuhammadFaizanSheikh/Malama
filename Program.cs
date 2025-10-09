using ExcelFilesCompiler;
using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using ExcelFilesCompiler.UnitOfWork;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";  // Customize the header name (optional)
});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IFileUploader, FileUploader>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IEventStaffService, EventStaffService>();
builder.Services.AddScoped<IEventManagementService, EventManagementService>();
builder.Services.AddScoped<IImmunizationStationService, ImmunizationStationService>();
builder.Services.AddScoped<IImmunizationVaccineInfoService, ImmunizationVaccineInfoService>();
//builder.Services.AddScoped<IGenericRepository<SubContractorService>, GenericRepository<SubContractorService>>();
builder.Services.AddScoped<ISubContractorService, SubContractorService>();
builder.Services.AddScoped<IAccountRegistrationService, AccountRegistrationService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("*", builder =>
    {
        builder.SetIsOriginAllowed(origin => true) // Allow all origins dynamically
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Allow cookies and authentication
    });
});

//builder.Services.AddControllersWithViews()
//    .AddRazorRuntimeCompilation()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//    });


// Entity Framework and Identity
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(5); // Set token lifespan
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; // Set to None for cross-origin
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Authentication
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Migrate the database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    var env = services.GetRequiredService<IWebHostEnvironment>();

    await SeedData.Initialize(services, builder, env);
}

// Configure middleware

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("*"); // Apply CORS middleware globally   
app.UseSession();
app.UseAuthentication(); // Must be before UseAuthorization
app.UseAuthorization();

//if (env.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage(); // Enable detailed error messages in development
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error"); // Handle errors in production
//}

// Configure endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

app.Run();
