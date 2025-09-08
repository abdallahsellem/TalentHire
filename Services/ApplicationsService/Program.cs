using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TalentHire.Services.ApplicationsService.Data;
using TalentHire.Services.ApplicationsService.Repositories;
using TalentHire.Services.ApplicationsService.Mapper;
using TalentHire.Services.ApplicationsService.Services;
using TalentHire.Shared.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ApplicationMapperProfile));

// Add Repository
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<KafkaApplicationProducerService>();


// Add JWT Authentication
var secretKey = builder.Configuration.GetSection("JwtSettings")["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IJobServiceClient, JobServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://job-service/api"); // Use service name or gateway route
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

