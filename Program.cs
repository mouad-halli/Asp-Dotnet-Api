using FirstAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using FirstAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using FirstAPI.interfaces;
using FirstAPI.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("connection string not found");

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString)
);

//IdentityFramework
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

//Jwt configuration
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>() ?? throw new InvalidOperationException("jwt issuer not found");
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>() ?? throw new InvalidOperationException("jwt key not found");

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "access_token";
        options.Cookie.HttpOnly = true; // XSS protection by preventing browser javascript from accessing the cookie
        // options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Always use HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // CSRF protection
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.Events = new JwtBearerEvents
        {
        /*
            Since we are storing the JWT token in a cookie rather than as a Bearer token in the Authorization header
            (where the JwtBearer middleware typically expects it), we need to implement custom extraction logic.
            This logic will retrieve the token from the cookie in the HTTP request when a user accesses a route that requires authorization.
        */
            OnMessageReceived = context =>
            {
                // if cookie named access_token was found it is stored inside a variable named accessToken
                context.Request.Cookies.TryGetValue("access_token", out var accessToken);

                if (!string.IsNullOrEmpty(accessToken))
                    // setting the token for authorization
                    context.Token = accessToken;
                
                return Task.CompletedTask;
            },
            // OnAuthenticationFailed = context =>
            // {
            //     return Task.CompletedTask;
            // }
        };
    }
);

// registering service for dependency injection with AddScoped method
builder.Services.AddScoped<ITokenService, TokenService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
