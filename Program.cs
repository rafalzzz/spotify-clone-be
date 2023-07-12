using Microsoft.EntityFrameworkCore;
using FluentValidation;
using SpotifyAPI.Entities;
using SpotifyAPI.Helpers;
using SpotifyAPI.Services;
using SpotifyAPI.Validations;
using SpotifyAPI.Variables;
using SpotifyAPI.Requests;

var builder = WebApplication.CreateBuilder(args);

// DB context
EnvironmentHelper.EnsureConnectionStringVariableExists(EnvironmentVariables.ConnectionString);
var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.ConnectionString);

builder.Services.AddDbContext<SpotifyDbContext>(options => options.UseNpgsql(connectionString));

// Additional Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IRequestValidationService, RequestValidationService>();

// Validators
builder.Services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
