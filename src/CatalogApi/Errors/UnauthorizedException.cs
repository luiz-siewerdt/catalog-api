
namespace CatalogApi.Errors;

public class UnauthorizedException(string msg = "") : ServiceException(401, msg) { }

