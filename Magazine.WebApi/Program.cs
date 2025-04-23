//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerUI;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Magazine.Core.Services;
//using Magazine.WebApi.Services;
//using Magazine.Core.Models;

//var builder = WebApplication.CreateBuilder(args);
//builder.Logging.AddConsole();

//// Добавление CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllOrigins",
//        builder =>
//        {
//            builder.AllowAnyOrigin() // Разрешить запросы с любого origin
//                   .AllowAnyMethod() // Разрешить все HTTP-методы (GET, POST, PUT и т.д.)
//                   .AllowAnyHeader(); // Разрешить все заголовки
//        });
//});


//// Регистрация сервисов
//builder.Services.AddScoped<IProductService, ProductService>();

//// Добавление контроллеров
//builder.Services.AddControllers();


//// Add services to the container.
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
//builder.Services.AddEndpointsApiExplorer(); // Добавляет поддержку API Explorer
//builder.Services.AddSwaggerGen(); // Генерирует Swagger-документацию

//var app = builder.Build();

//app.UseCors("AllowAllOrigins");


//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//    app.UseSwagger(); // Включает генерацию Swagger-документации
//    app.UseSwaggerUI(); // Включает Swagger UI
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Magazine.Core.Services;
using Magazine.WebApi.Services;
using Magazine.Core.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Добавление CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() // Разрешить запросы с любого origin
                   .AllowAnyMethod() // Разрешить все HTTP-методы (GET, POST, PUT и т.д.)
                   .AllowAnyHeader(); // Разрешить все заголовки
        });
});


// Регистрация сервисов
//builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductService, DataBaseProductService>();

// Добавление контроллеров
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(); // Только для разработки!
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Добавляет поддержку API Explorer
builder.Services.AddSwaggerGen(); // Генерирует Swagger-документацию

var app = builder.Build();

app.UseCors("AllowAllOrigins");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); // Включает генерацию Swagger-документации
    app.UseSwaggerUI(); // Включает Swagger UI
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
