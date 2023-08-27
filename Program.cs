using Microsoft.EntityFrameworkCore;
using NLog.Web;
using FluentValidation;
using SpotifyAPI.Configuration;
using SpotifyAPI.Entities;
using SpotifyAPI.Helpers;
using SpotifyAPI.Models;
using SpotifyAPI.Middleware;
using SpotifyAPI.Services;
using SpotifyAPI.Validations;
using SpotifyAPI.Variables;
using SpotifyAPI.Requests;

var builder = WebApplication.CreateBuilder(args);

// DB context
EnvironmentHelper.EnsureConnectionStringVariableExists(EnvironmentVariables.ConnectionString);
var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.ConnectionString);

builder.Services.AddDbContext<SpotifyDbContext>(options => options.UseNpgsql(connectionString));

// Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var passwordResetSettings = builder.Configuration.GetSection("PasswordResetSettings");
builder.Services.Configure<PasswordResetSettings>(passwordResetSettings);

builder.Host.UseNLog();
new CorsConfiguration(builder.Services);

// Additional Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IRequestValidationService, RequestValidationService>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IAccessTokenService, AccessTokenService>();
builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddTransient<IPasswordResetService, PasswordResetService>();
builder.Services.AddTransient<IEmailService, EmailService>();

// Validators
builder.Services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
builder.Services.AddScoped<IValidator<LoginUserRequest>, LoginUserRequestValidator>();
builder.Services.AddScoped<IValidator<PasswordResetRequest>, PasswordResetRequestValidator>();
builder.Services.AddScoped<IValidator<PasswordResetCompleteRequest>, PasswordResetCompleteRequestValidator>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(Cors.CorsPolicy);

// Configure the HTTP request pipeline.

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
