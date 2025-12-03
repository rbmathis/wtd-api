using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WillTheyDie.Api.Data;
using WillTheyDie.Api.Services;
using WillTheyDie.Api.Endpoints;
using WillTheyDie.Api.Configuration;
using Microsoft.FeatureManagement;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure Azure App Configuration
var azureAppConfigSettings = builder.Configuration.GetSection(AzureAppConfigSettings.SectionName).Get<AzureAppConfigSettings>();
if (azureAppConfigSettings?.Enabled == true && !string.IsNullOrEmpty(azureAppConfigSettings.ConnectionString))
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(azureAppConfigSettings.ConnectionString)
            .ConfigureRefresh(refresh =>
            {
                foreach (var key in azureAppConfigSettings.WatchedKeys)
                {
                    refresh.Register(key, refreshAll: true)
                        .SetRefreshInterval(TimeSpan.FromSeconds(azureAppConfigSettings.RefreshIntervalSeconds));
                }
            })
            .UseFeatureFlags(featureFlagOptions =>
            {
                featureFlagOptions.SetRefreshInterval(TimeSpan.FromSeconds(azureAppConfigSettings.RefreshIntervalSeconds));
            });
    });
}

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Redis Settings
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(RedisSettings.SectionName));
var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();

// Add Redis Distributed Cache
if (redisSettings?.Enabled == true)
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisSettings.ConnectionString;
        options.InstanceName = redisSettings.InstanceName;
    });
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IBetService, BetService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();

// Add Feature Management
builder.Services.AddFeatureManagement();

// Add Azure App Configuration if enabled
if (azureAppConfigSettings?.Enabled == true)
{
    builder.Services.AddAzureAppConfiguration();
}

// Add JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] 
    ?? throw new InvalidOperationException("JWT Secret Key not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:5173" };
        
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add OpenAPI
builder.Services.AddOpenApi();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

if (redisSettings?.Enabled == true)
{
    builder.Services.AddHealthChecks()
        .AddRedis(redisSettings.ConnectionString, name: "redis", tags: new[] { "cache" });
}

var app = builder.Build();

// Seed database in development environment
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Apply migrations (stub - will fail without actual database)
        // await context.Database.MigrateAsync();
        
        // Seed data
        await DbSeeder.SeedAsync(context);
        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database seeding skipped - no database connection available (stubbed)");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapAuthEndpoints();
app.MapShowEndpoints();
app.MapBetEndpoints();
app.MapUserEndpoints();
app.MapEpisodeEndpoints();
app.MapFeatureEndpoints();

// Health check endpoint
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

// Detailed health check endpoint
app.MapHealthChecks("/api/health/ready")
    .WithName("HealthCheckReady")
    .WithTags("Health");

app.Run();
