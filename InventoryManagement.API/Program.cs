using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Application.Interfaces.UnitOfWork;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Application.Services.Implementations;
using InventoryManagement.Persistence.Context;
using InventoryManagement.Persistence.Repositories;
using InventoryManagement.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
