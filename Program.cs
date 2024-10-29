using WordCloud.Hubs;
using WordCloud.Services.Implementations;
using WordCloud.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add SignalR services
builder.Services.AddSignalR();

// Optional: Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Register application services
builder.Services.AddScoped<IWordCloudService, WordCloudService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Ensure UseRouting is called before UseEndpoints
app.UseRouting();

// Configure CORS (optional, if using CORS)
app.UseCors();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

// Map routes and SignalR hub after UseRouting
app.UseEndpoints(endpoints =>
{
    // Map the SignalR hub
    endpoints.MapHub<WordCloudHub>("/wordcloudHub");

    // Default controller route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=WordView}/{action=WordCloud}/{id?}");

});

app.Run();