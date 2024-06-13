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
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy.WithOrigins("http://localhost:5173")  // Ensure this is the correct URL
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.Authority = "https://YOUR_AUTH0_DOMAIN/";
//    options.Audience = "YOUR_API_IDENTIFIER";
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true
//    };
//});

//builder.Services.AddControllers();

var app = builder.Build();

// HTTP request pipeline configuration
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
//app.UseAuthentication();  // Authentication middleware should come before Authorization
//app.UseAuthorization();   // Only need to call this once after UseAuthentication
app.MapControllers();



//app.MapPost("/security/createToken",
//[AllowAnonymous] (User user) =>
//{
//    if (user.Email == "joydip" && user.Password == "joydip123")
//    {
//        var issuer = builder.Configuration["Jwt:Issuer"];
//        var audience = builder.Configuration["Jwt:Audience"];
//        var key = Encoding.ASCII.GetBytes
//        (builder.Configuration["Jwt:Key"]);
//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject = new ClaimsIdentity(new[]
//            {
//                new Claim("Id", Guid.NewGuid().ToString()),
//                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
//                new Claim(JwtRegisteredClaimNames.Email, user.Email),
//                new Claim(JwtRegisteredClaimNames.Jti,
//                Guid.NewGuid().ToString())
//            }),
//            Expires = DateTime.UtcNow.AddMinutes(5),
//            Issuer = issuer,
//            Audience = audience,
//            SigningCredentials = new SigningCredentials
//            (new SymmetricSecurityKey(key),
//            SecurityAlgorithms.HmacSha512Signature)
//        };
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var token = tokenHandler.CreateToken(tokenDescriptor);
//        var stringToken = tokenHandler.WriteToken(token);
//        return Results.Ok(stringToken);
//    }
//    return Results.Unauthorized();
//});

app.Run();