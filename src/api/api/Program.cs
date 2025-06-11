using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using AspNetCoreRateLimit;
using System.Text;
using FinSync.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Serilog Logging
builder.Host.UseSerilog((ctx, config) =>
{
    config.ReadFrom.Configuration(ctx.Configuration);
});

// ✅ Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var apiKeyHeader = builder.Configuration["ApiKeySettings:HeaderName"];
var validApiKeys = builder.Configuration.GetSection("ApiKeySettings:ValidKeys").Get<string[]>();

// ✅ Database Context
builder.Services.AddDbContext<FinSyncContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });

// ✅ Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ✅ Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "FinSync API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer {token}'"
    };

    opt.AddSecurityDefinition("Bearer", jwtScheme);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, new[] { "Bearer" } }
    });
});

// ✅ API Key middleware support
builder.Services.AddSingleton(validApiKeys ?? Array.Empty<string>());
builder.Services.AddSingleton(apiKeyHeader ?? "x-api-key");

var app = builder.Build();

// ✅ Middleware
app.UseSerilogRequestLogging();
app.UseIpRateLimiting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ✅ API Key verification middleware
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/api"))
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue(apiKeyHeader, out var extractedKey) ||
        !(validApiKeys ?? Array.Empty<string>()).Contains(extractedKey.ToString()))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("API Key is missing or invalid.");
        return;
    }

    await next();
});

app.MapControllers();

app.Run();
