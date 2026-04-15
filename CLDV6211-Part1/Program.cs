using CLDV6211_POE_PART1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CLDV6211_POE_PART1.Data;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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


app.Run();

/*References
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8. 
 * Available at: https://visualstudio.microsoft.com (Accessed: 22 March 2026).
*/