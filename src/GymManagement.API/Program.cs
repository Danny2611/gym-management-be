using GymManagement.API.Extensions;
using GymManagement.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// =========================
// Controllers
// =========================
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
// =========================
// Swagger
// =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("Database:Mongo"));

// Add MoMo Configuration
builder.Services.Configure<MoMoSettings>(
    builder.Configuration.GetSection("MoMo"));

// =========================
// CORS (SPA)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =========================
// AUTHENTICATION (JWT + External Cookie)
// =========================
// =========================
// AUTHENTICATION
// =========================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
        )
    };
})
.AddCookie("External", options =>
{
    options.Cookie.Name = "Gym.External";
    options.Cookie.HttpOnly = true;

    // ⭐ DEVELOPMENT: Dùng Lax cho localhost
    options.Cookie.SameSite = SameSiteMode.Lax;  // ✅ Thay đổi từ None
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // localhost không HTTPS

    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

    options.CallbackPath = "/signin-google"; // ✅ Khớp với Google Console

    options.SignInScheme = "External"; // Cookie scheme

    options.SaveTokens = true;

    options.Scope.Add("email");
    options.Scope.Add("profile");

    options.ClaimActions.MapJsonKey("picture", "picture");

    // ⭐ QUAN TRỌNG: Correlation Cookie Settings
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;  // ✅ Thay đổi
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None; // localhost
    options.CorrelationCookie.HttpOnly = true;
});



builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Accept cả camelCase và PascalCase khi nhận data
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        // Return data theo camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });


var app = builder.Build();

// =========================
// MIDDLEWARE
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
