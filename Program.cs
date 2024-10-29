    using ExcelFilesCompiler;
    using ExcelFilesCompiler.Controllers.Services;
    using ExcelFilesCompiler.Interfaces;
    using ExcelFilesCompiler.Repositories.Interfaces;
    using ExcelFilesCompiler.Repositories.Services;
    using Microsoft.EntityFrameworkCore;
    using System.Configuration;

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
    builder.Services.AddControllersWithViews(); // Or AddMvc(), depending on your project
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddScoped(typeof(IFileUploaderRepository<>), typeof(FileUploaderRepository<>));
builder.Services.AddScoped<IFileUploader, FileUploader>();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout period
        options.Cookie.HttpOnly = true; // Prevent JavaScript access
        options.Cookie.IsEssential = true; // Ensure cookie is always stored
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });
    builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);



var app = builder.Build();
        app.UseCors("AllowAllOrigins");
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
        app.UseDeveloperExceptionPage();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();
    //app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
    app.Run();
