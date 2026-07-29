using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

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

// Compress streamed binary glTF payloads (their JSON chunk with object properties compresses very well).
webApplicationBuilder.Services.AddResponseCompression(responseCompressionOptions =>
{
    responseCompressionOptions.EnableForHttps = true;
    responseCompressionOptions.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["model/gltf-binary"]);
});

// We use AddControllersWithViews because you want to display HTML pages (.cshtml)
// DiGi.Communication.WebAPI is referenced for its GIS agnostic propagation calculation result types
// only. It is an extension assembly hosted by api.digiproject.uk, so MVC discovering it as an
// application part would publish its controllers here too (verified against the placeholder endpoint
// it used to carry). It holds no controller today, which makes the removal below a no-op - it is kept
// deliberately: any endpoint that assembly gains belongs to the deployed Web API, not to this UI, and
// without the removal it would silently start answering here as well.
webApplicationBuilder.Services.AddControllersWithViews().ConfigureApplicationPartManager(applicationPartManager =>
{
    ApplicationPart? applicationPart = applicationPartManager.ApplicationParts.FirstOrDefault(x => x.Name == "DiGi.Communication.WebAPI");
    if (applicationPart is not null)
    {
        applicationPartManager.ApplicationParts.Remove(applicationPart);
    }
});

// Register all IGLTFNodeConverter implementations with the generic DiGi.GLTF engine (plugin
// pattern): the shared DiGi.GLTF.Analytical assembly owns the DiGi.Analytical converters
// (BuildingModel, UrbanModel, IComponent). Adding support for a new domain type only requires
// adding a new converter class - no other code changes.
DiGi.GLTF.Analytical.Modify.Register();
DiGi.GLTF.Modify.Register(typeof(Program).Assembly);

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
webApplication.UseResponseCompression();
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