using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// WebApplicationBuilder is the factory for our web application
WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

string corsPolicyName = "DiGi_Origins";

webApplicationBuilder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.WithOrigins("https://gis.digiproject.uk", "https://www.gis.digiproject.uk")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 1. Add services to the container (Dependency Injection)

// Register IHttpClientFactory to allow server-side API calls
webApplicationBuilder.Services.AddHttpClient();

// We use AddControllersWithViews because you want to display HTML pages (.cshtml)
webApplicationBuilder.Services.AddControllersWithViews();

// Build the application
WebApplication webApplication = webApplicationBuilder.Build();

// 2. Configure the HTTP request pipeline (Middleware)
if (!webApplication.Environment.IsDevelopment())
{
    // Production error handling
    webApplication.UseExceptionHandler("/Home/Error");
    webApplication.UseHsts();
}

webApplication.UseHttpsRedirection();
webApplication.UseStaticFiles(); // Required to serve CSS, JS, and Images from wwwroot
webApplication.UseRouting();
webApplication.UseCors(corsPolicyName);
webApplication.UseAuthorization();

// 3. Map your controllers using Attribute Routing
webApplication.MapControllers();

webApplication.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Start the application
webApplication.Run();