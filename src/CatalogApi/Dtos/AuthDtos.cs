namespace CatalogApi.Dtos;


public record AuthSignInRequestDto(string Email, string Password);

public record AuthSignInResponseDto(string Token);
