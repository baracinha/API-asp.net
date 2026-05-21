using Microsoft.Extensions.Options;
using hotel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using hotel.Jwt;
using hotel.Services;
using hotel.Controllers;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<AutenthicationServices>();

builder.Services.AddScoped<ChatServices>();

builder.Services.AddScoped<TokenProvider>();

builder.Services.AddScoped<SalesServices>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Porta padrão do Redis
    options.InstanceName = "HotelProject_";
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Conn"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Conn"))
    ));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
    policy.WithOrigins("http://baracinha.ddns.net:9600", "http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    }); 

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(Options =>
    {
        Options.SwaggerEndpoint("/openapi/v1.json", "api");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
