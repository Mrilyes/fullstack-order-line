using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using order_orderline.Application.Services;
using order_orderline.Infrastructure.Data;
using order_orderline.Infrastructure.Mappings;
using order_orderline.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Add CORS Policy to allow frontend to make requests to backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins("http://localhost:5173") // your frontend URL
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


// 1. Load Configuration for Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Register DbContext with DI container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// will let the controller to consume from the Repository
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderLineRepository, OrderLineRepository>();
builder.Services.AddScoped<IOrderLineService, OrderLineService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MappingProfile));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//Avoid Table Recreation with EnsureCreated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated(); // Ensures database exists but does NOT run migrations
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowFrontend");  // Enable the CORS policy you just added

app.UseAuthorization();

app.MapControllers();

app.Run();
