using car_booking_service.Application.Services.Implementations;
using car_booking_service.Application.Services.Interfaces;
using car_booking_service.Domain.Entities;
using car_booking_service.Domain.Interfaces;
using car_booking_service.Infrastructure.Data.Context;
using car_booking_service.Infrastructure.Repositories;
using car_booking_service.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using System.Text.Json.Serialization;
using car_booking_service.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerOpt =>
{
    swaggerOpt.OperationFilter<SnakeCaseOperationFilter>();
}).AddControllers(cOpt =>
{
    cOpt.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;

    options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
});

//Configure Connection to Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("CarTestDriveBookingDb")
           .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))); //Ignore transaction

//Setup Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log.txt",
                  rollingInterval: RollingInterval.Day,
                  fileSizeLimitBytes: 1_048_576)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

//Register Repository
builder.Services.AddScoped<IGenericRepository<CarModel>, CarModelRepository>();
builder.Services.AddScoped<ICarModelRepository, CarModelRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Register Service
builder.Services.AddScoped<ICarModelService, CarModelService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});


builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
