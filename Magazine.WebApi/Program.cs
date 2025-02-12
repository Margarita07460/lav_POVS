using Magazine.Core.Services;
using Magazine.WebApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������� Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� ��������� ������������
builder.Services.AddControllers();

// ��������� ������ ProductService
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// ���������� Swagger � ������ ����������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// �������� �������������
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // �������� ��������� API-������������

app.Run();
