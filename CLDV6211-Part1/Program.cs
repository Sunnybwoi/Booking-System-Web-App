using System;
using System.Threading.Tasks;
using CLDV6211_POE_PART1;
using CLDV6211_Part1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CLDV6211_POE_PART1.Data;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

/* Connecting to SQL Server with retry logic for transient faults
 * Code completion assisted by Visual Studio IntelliSense
 * (Microsoft Corporation, 2022). Version 17.8. */

builder.Services.AddDbContext<CLDV6211_DbContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: (null)

    )));

// Add services to the container.
builder.Services.AddControllersWithViews();

//Use scoped instead of Singleton for services that interact with the database to ensure a new instance per request
builder.Services.AddScoped<IBlobService, BlobService>();

var app = builder.Build();

// Ensure developer exception page is enabled in Development so exceptions are visible in-browser
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Register global exception handlers to log unobserved exceptions (helps diagnose crashes while debugging)
try
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
    {
        if (eventArgs.ExceptionObject is Exception ex)
            logger.LogCritical(ex, "Unhandled exception (AppDomain)");
        else
            logger.LogCritical("Unhandled exception (AppDomain): {Obj}", eventArgs.ExceptionObject);
    };

    TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
    {
        logger.LogError(eventArgs.Exception, "Unobserved task exception");
        eventArgs.SetObserved();
    };

    app.Run();
}
catch (Exception ex)
{
    // Log to console as a fallback so you can see the error when running without the debugger
    Console.Error.WriteLine(ex.ToString());
    throw;
}

/*References
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8. 
 * Available at: https://visualstudio.microsoft.com (Accessed: 24 April 2026).
*/