using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// WebApplicationBuilder is the factory for our web application
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container (Dependency Injection)
// We use AddControllersWithViews because you want to display HTML pages (.cshtml)
builder.Services.AddControllersWithViews();

// Build the application
WebApplication app = builder.Build();

// 2. Configure the HTTP request pipeline (Middleware)
if (!app.Environment.IsDevelopment())
{
    // Production error handling
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Required to serve CSS, JS, and Images from wwwroot
app.UseRouting();
app.UseAuthorization();

// 3. Map your controllers using Attribute Routing ([HttpGet("/about")])
app.MapControllers();

// Start the application
app.Run();