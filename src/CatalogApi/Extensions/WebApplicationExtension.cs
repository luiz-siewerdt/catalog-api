using CatalogApi.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace CatalogApi.Extensions;

public static class WebApplicationExtension {
  public static WebApplication AddSettings(this WebApplication app) {
    if (app.Environment.IsDevelopment()) {
      app.UseSwagger();
      app.UseSwaggerUI();
    }
    app.MapControllers();
    app.UseHttpsRedirection();

    return app;
  }

  public static WebApplication AddExceptionHandler(this WebApplication app) {
    app.UseExceptionHandler("/error");
    app.Map("/error", static (HttpContext ctx) => {
      var exception = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;

      return exception is null ? Results.Problem()
        : exception switch {
          UnauthorizedException ex => Results.Problem(statusCode: 401),
          ServiceException ex => Results.Problem(statusCode: ex.StatusCode, detail: ex.StatusMessage),
          _ => Results.Problem(statusCode: 500, detail: "Ops, algo deu errado, tente novamente mais tarde")
        };
    });

    return app;
  }
}
