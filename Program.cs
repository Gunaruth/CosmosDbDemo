using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using CosmosDbDemo.Repository;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(s =>
{
    var client = new CosmosClient(builder.Configuration["CosmosDb:ConnectionString"]);
    return new CosmosRepository(client, builder.Configuration["CosmosDb:DatabaseName"]);
});

builder.Services.AddSingleton<ICosmosRepository>(sp => sp.GetRequiredService<CosmosRepository>());
builder.Services.AddScoped<IRepository<GavUser>, UserRepository>();
builder.Services.AddScoped<IRepository<AuditEngagement>, GroupRepository>();
builder.Services.AddScoped<IRepository<ComponentEngagement>, ComponentRepository>();
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
