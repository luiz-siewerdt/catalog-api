using Microsoft.EntityFrameworkCore;
using CatalogApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var CONNECTION_STRING = builder.Configuration.GetConnectionString("DefaultConnection")
  ?? throw new Exception("dbconnection not founded");

var JWT_SECRET = builder.Configuration["ProjectSettings:JWT_Secret"]
  ?? throw new Exception("jwt_secret not founded");

builder.Services.AddSettings()
  .AddDbContextHandling(CONNECTION_STRING)
  .AddAuthenticationHandling(JWT_SECRET)
  .AddProblemDetailsHandling()
  .AddRepositories()
  .AddServices()
  .AddHelpers();

var app = builder.Build();

app.AddSettings().AddExceptionHandler();

app.Run();

