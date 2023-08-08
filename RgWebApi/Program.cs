using Microsoft.OpenApi.Models;
using RbModels.Configuration;
using RpDataHelper.Extensions;
using RpDataHelper.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.Configure<JwtConfig>(configuration.GetSection("JWT"));
builder.Services.InjectDbContext(configuration.GetConnectionString("ConnString")
                                 ?? throw new Exception("Empty Connection String"));
builder.Services.InjectIdentity();
builder.Services.InjectAuth(configuration);
builder.Services.InjectServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RpExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();