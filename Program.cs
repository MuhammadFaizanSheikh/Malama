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
using Microsoft.AspNetCore.SignalR;
using Malama.Controllers.Services.ContainerTempMonitoringServices;
using Malama.Authorization;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Malama.Interfaces;
using Malama.Controllers.Services;
using Malama.AutoMapper;
using Malama.Services.Pdf.Sf600;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";  // Customize the header name (optional)
});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddScoped<IGenericRepository<SubmissionTokenRecord>, GenericRepository<SubmissionTokenRecord>>();
builder.Services.AddScoped<IFileUploader, FileUploader>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IEventStaffService, EventStaffService>();
builder.Services.AddScoped<IEventManagementService, EventManagementService>();
builder.Services.AddScoped<IImmunizationStationService, ImmunizationStationService>();
builder.Services.AddScoped<ILabStationService, LabStationService>();
builder.Services.AddScoped<IImmunizationVaccineInfoService, ImmunizationVaccineInfoService>();
builder.Services.AddScoped<IContainerMonitoringService, ContainerMonitoringService>();
//builder.Services.AddScoped<IGenericRepository<SubContractorService>, GenericRepository<SubContractorService>>();
builder.Services.AddScoped<ISubContractorService, SubContractorService>();
builder.Services.AddScoped<IUserEventMappingService, UserEventMappingService>();
builder.Services.AddScoped<IAccountRegistrationService, AccountRegistrationService>();
builder.Services.AddScoped<IDawsonUserService, DawsonUserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
builder.Services.AddScoped<ISf600TemplateProvider, Sf600TemplateProvider>();
builder.Services.AddSingleton<ISf600PdfFontProvider, Sf600PdfFontProvider>();
builder.Services.AddScoped<ISf600PdfOverlayWriter, Sf600PdfOverlayWriter>();
builder.Services.AddScoped<ISf600ImmunizationPdfGenerator, Sf600ImmunizationPdfGenerator>();
builder.Services.AddScoped<IEventUsersService, EventUsersService>();
builder.Services.AddScoped<ISubmissionTokenService, SubmissionTokenService>();
builder.Services.AddScoped<IPostEventManagementService, PostEventManagementService>();
builder.Services.AddScoped<IPostEventLabStationService, PostEventLabStationService>();
builder.Services.AddScoped<IPostEventImmunizationStationService, PostEventImmunizationStationService>();
builder.Services.AddScoped<IFileUploadDownloadService, FileUploadDownloadService>();
builder.Services.AddScoped<IVitalStationService, VitalStationService>();
builder.Services.AddScoped<IDentalXRayStationService, DentalXRayStationService>();
builder.Services.AddScoped<IDentalQuestionnaireService, DentalQuestionnaireService>();
builder.Services.AddScoped<IDentalExamService, DentalExamService>();
builder.Services.AddScoped<IDentalTreatmentService, DentalTreatmentService>();
builder.Services.AddScoped<DentalXRayFileSaveCoordinator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHostedService<TemperatureMonitorService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365);
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
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(365); // or any duration you want
    options.SlidingExpiration = true; // refreshes the cookie on activity
});

// Authentication
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RoleAttributePolicy", policy =>
    {
        policy.Requirements.Add(new RoleAttributeRequirement(Array.Empty<(string Role, string Attribute)>()));
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, RoleAttributeHandler>();


builder.Services.AddSignalR();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,       // One file per day
        fileSizeLimitBytes: 5 * 1024 * 1024,         // 5 MB
        rollOnFileSizeLimit: true,                   // Create new file when limit reached
        retainedFileCountLimit: null,                  // Keep last 10 files (optional)
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1)
    )
    .CreateLogger();

builder.Host.UseSerilog();

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

    await SeedData.InitializeAsync(services, builder, env);
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
    endpoints.MapHub<TemperatureHub>("/temperatureHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

app.Run();
