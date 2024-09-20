using Capstone___Api.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string issuer = builder.Configuration["Jwt:Issuer"]!;
string audience = builder.Configuration["Jwt:Audience"]!;
string key = builder.Configuration["Jwt:Key"]!;

builder.Services
    .AddAuthentication(opt => {
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
        };
    })
    ;
builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(options =>
       { 
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
           options.JsonSerializerOptions.WriteIndented = true;
    });
  

var conn = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(conn));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  ;
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowAngularClient");

app.MapControllers();

app.Run();
