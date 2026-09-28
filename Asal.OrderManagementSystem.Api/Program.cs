using Asal.OrderManagementSystem.Api.Data;
using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IProductRepository,ProductRepository>();
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();

app.Run();
