global using EmployeePortal.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Handle circular references
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignore null values when writing JSON
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Preserving PascalCase in JSON output
});

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database context configuration
builder.Services.AddDbContext<EmpPortalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(Program));

// Logging configuration
builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); // Add console logging
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTrustedOrigins", builder =>
    {
        builder.WithOrigins("https://zeush1bportal.azurewebsites.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});





var app = builder.Build();

// HTTP request pipeline configuration
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
//app.UseCors("AllowTrustedOrigins");
//app.UseAuthentication();  // Authentication middleware should come before Authorization
app.UseAuthorization();   // Only need to call this once after UseAuthentication
app.MapControllers();





app.Run();