using ElevatorCaseFlow.Application.Interfaces;
using ElevatorCaseFlow.Application.Services;
using ElevatorCaseFlow.Infrastructure.Data;
using ElevatorCaseFlow.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── 1. DATABASE ──
// Register AppDbContext with SQL Server
// Connection string is read from appsettings.json — not hardcoded!
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── 2. DEPENDENCY INJECTION ──
// Register our Repository and Service
// "Scoped" means one instance per HTTP request — standard for DB operations
builder.Services.AddScoped<ICaseRepository, CaseRepository>();
builder.Services.AddScoped<ICaseService, CaseService>();

// ── 3. JWT AUTHENTICATION ──
// Read JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    // Set JWT as the default authentication scheme
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate the server that created the token
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        // Validate the client receiving the token
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        // Validate the token expiry
        ValidateLifetime = true,

        // Validate the secret key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// ── 4. CONTROLLERS ──
builder.Services.AddControllers();

// ── 5. SWAGGER ──
// Swagger gives us a browser UI to test our API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ElevatorCaseFlow API",
        Version = "v1",
        Description = "REST API for managing elevator design case requests and workflow"
    });

    // Add JWT support to Swagger UI
    // This lets us test protected endpoints directly in the browser
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token here}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── BUILD THE APP ──
var app = builder.Build();

// ── 6. MIDDLEWARE PIPELINE ──
// Order matters here! Each request passes through these in sequence

// Show Swagger UI in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ElevatorCaseFlow API v1");
        options.RoutePrefix = string.Empty; // Opens Swagger at root URL
    });
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// ── 7. AUTO-CREATE DATABASE ──
// This runs migrations automatically when the app starts
// No manual database setup needed on any machine!
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

