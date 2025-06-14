using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TalentHire.Services.ApplicationsService.Data;
using TalentHire.Services.JobService.Mapper;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));
var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });
// Create the mapper and register it as a singleton

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


var secretKey = builder.Configuration.GetSection("JwtSettings")["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"], // Add this line
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();               // ✅ 1. Routing first
app.UseAuthentication();        // ✅ 2. Auth middleware
app.UseAuthorization();         // ✅ 3. Authorization AFTER Authentication

app.MapControllers();           // ✅ 4. Map endpoints

app.Run();

