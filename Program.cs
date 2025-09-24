using CosmosDbDemo.Interface;
using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using CosmosDbDemo.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton(s =>
//{
//    var client = new CosmosClient(builder.Configuration["CosmosDb:ConnectionString"]);
//    return new CosmosRepository(client, builder.Configuration["CosmosDb:DatabaseName"]);
//});

builder.Services.Configure<CosmosDbSettings>(
    builder.Configuration.GetSection("CosmosDb"));

// Register CosmosClient
builder.Services.AddSingleton(s =>
{
    var settings = s.GetRequiredService<IOptions<CosmosDbSettings>>().Value;
    return new CosmosClient(settings.Account, settings.Key);
});

// Register CosmosRepository and UserRepository
//builder.Services.AddSingleton<ICosmosRepository, CosmosRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(CosmosRepository<>));
//builder.Services.AddSingleton<ICosmosRepository>(sp => sp.GetRequiredService<CosmosRepository>());
//builder.Services.AddScoped<IRepository<GavUser>, UserRepository>();

//builder.Services.AddSingleton<ICosmosRepository, CosmosRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IComponentRepository, ComponentRepository>();
builder.Services.AddScoped<ICommunicationRepository, CommunicationRepository>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAngularApp");
app.MapControllers();

app.Run();
