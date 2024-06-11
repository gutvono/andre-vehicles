using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AndreVehicles.Data;
using AndreVehicles.Utils;
using Microsoft.Extensions.Options;
using AndreVehicles.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AndreVehiclesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AndreVehiclesContext") ?? throw new InvalidOperationException("Connection string 'AndreVehiclesContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoConfig>(builder.Configuration.GetSection(nameof(MongoConfig)));
builder.Services.AddSingleton<IMongoConfig>(sp => sp.GetRequiredService<IOptions<MongoConfig>>().Value);
builder.Services.AddSingleton<AddressService>();

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
