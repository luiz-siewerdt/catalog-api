using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CatalogApi.Helpers;
using CatalogApi.Services;
using CatalogApi.Persistence.Data;
using CatalogApi.Persistence.Repositories;

namespace CatalogApi.Extensions;

public static class ServiceCollectionExtension {

  public static IServiceCollection AddRepositories(this IServiceCollection service) {
    service.AddScoped<IUserRepository, UserRepository>();
    service.AddScoped<IProductRepository, ProductRepository>();
    service.AddScoped<ICategoryRepository, CategoryRepository>();
    service.AddScoped<IAuthenticationHelper, AuthenticationHelper>();

    return service;
  }

  public static IServiceCollection AddServices(this IServiceCollection service) {
    service.AddScoped<IAuthService, AuthService>();
    service.AddScoped<IUserService, UserService>();
    service.AddScoped<IProductService, ProductService>();
    service.AddScoped<ICategoryService, CategoryService>();

    return service;
  }

  public static IServiceCollection AddHelpers(this IServiceCollection service) {
    service.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
    return service;
  }

  public static IServiceCollection AddAuthenticationHandling(this IServiceCollection service, string jwtSecret) {
    service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(opts => {
        opts.SaveToken = false;
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters {
          ValidateAudience = false,
          ValidateIssuer = false,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
          ClockSkew = TimeSpan.Zero,
        };
      });

    return service;
  }

  public static IServiceCollection AddProblemDetailsHandling(this IServiceCollection service) {
    service.AddProblemDetails(static opts => {
      opts.CustomizeProblemDetails = static ctx => {
        ctx.ProblemDetails.Extensions["ClientId"] = "clientId";
        ctx.ProblemDetails.Extensions["ServerVersion"] = "1.0.0";
        ctx.ProblemDetails.Extensions["TraceId"] = ctx.HttpContext.TraceIdentifier;
      };
    });

    return service;
  }

  public static IServiceCollection AddDbContextHandling(this IServiceCollection service, string connectionString) {
    service.AddDbContext<CatalogApiContext>(opts => opts.UseNpgsql(connectionString));
    return service;
  }

  public static IServiceCollection AddSettings(this IServiceCollection service) {
    service.AddControllers();
    service.AddEndpointsApiExplorer();
    service.AddSwaggerGen();

    return service;
  }
}
