using Magazine.Core.Services;
using Magazine.WebApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем поддержку контроллеров
builder.Services.AddControllers();

// Добавляем сервис ProductService
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Подключаем Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Настроим маршрутизацию
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Включаем поддержку API-контроллеров

app.Run();
