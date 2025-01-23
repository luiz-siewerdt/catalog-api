namespace CatalogApi.Errors;

public class BadRequestException(string msg) : ServiceException(400, msg) { }
