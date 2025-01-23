
namespace CatalogApi.Errors;


public class ServiceException(int statusCode, string msg)
: Exception {
  public int StatusCode { get; set; } = statusCode;
  public string StatusMessage { get; set; } = msg;
}
