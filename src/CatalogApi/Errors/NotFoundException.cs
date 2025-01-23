
namespace CatalogApi.Errors;

public class NotFoundException(string msg)
  : ServiceException(StatusCodes.Status404NotFound, msg) { };
