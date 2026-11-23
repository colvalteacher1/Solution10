using Microsoft.EntityFrameworkCore;
using MyApp.ProductsApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ProductsDatabase")
    ?? throw new InvalidOperationException("La chaîne de connexion ProductsDatabase est introuvable.");

builder.Services.AddControllers();
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
