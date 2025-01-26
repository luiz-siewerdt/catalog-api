namespace CatalogApi.Errors;

public abstract class ProductServiceErrors {
  public const string NotFound = "Produto não encontrado!";
  public const string InvalidName = "Nome inválido!";
  public const string InvalidPrice = "Preço inválido!";
  public const string InvalidDiscount = "Desconto Inválido!";
}

public abstract class AuthServiceErrors {
  public const string IncorrectLogin = "Email/Senha Inválidos!";
  public const string InvalidToken = "Token Inválido!";
}

public abstract class UserServiceErrors {
  public const string NotFound = "Usuário não encontrado!";
  public const string InvalidName = "Nome inválido!";
  public const string InvalidEmail = "Email inválido!";
  public const string EmailAlreadyInUse = "Email já em uso!";
  public const string InvalidPassword = "Senha inválida!";
  public const string NotIqualPassword = "Senhas não conferem!";
}

public abstract class CategoryServiceErrors {
  public const string NotFound = "Categoria não encontrada!";
  public const string NameAlreadyExists = "Categoria já existe!";
  public const string InvalidName = "Nome inválido!";
}
