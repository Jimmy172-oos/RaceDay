using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers (the classes that hold our endpoints)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Stops JSON errors when objects point at each other (e.g. Event <-> Category)
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 2. Database (SQL Server through Entity Framework Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Session (remembers who is logged in and their role)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Swagger (a web page where we can see and test every endpoint)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Shows the /// comments we write on our endpoints inside Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // Tells the Swagger page where to find the list of endpoints
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay.Api v1");

    // Opens Swagger straight away when the app starts (no need to type /swagger)
    options.RoutePrefix = string.Empty;
});

app.UseSession();
app.MapControllers();

app.Run();

