namespace CatalogApi.Errors;

public class ProductServiceErrors {
  public string Value { get; private set; }

  private ProductServiceErrors(string value) { Value = value; }

  public static ProductServiceErrors NotFounded { get { return new ProductServiceErrors("Produto não encontrado!"); } }
  public static ProductServiceErrors InvalidName { get { return new ProductServiceErrors("Nome inválido!"); } }
  public static ProductServiceErrors InvalidPrice { get { return new ProductServiceErrors("Preço inválido!"); } }
  public static ProductServiceErrors InvalidDiscount { get { return new ProductServiceErrors("Desconto Inválido!"); } }
}

public class AuthServiceErrors {
  public string Value { get; private set; }

  private AuthServiceErrors(string value) { Value = value; }

  public static AuthServiceErrors IncorrectLogin { get { return new AuthServiceErrors("Email/Senha Inválidos!"); } }
  public static AuthServiceErrors InvalidToken { get { return new AuthServiceErrors("Token Inválido!"); } }
}

public class UserServiceErrors {
  public string Value { get; private set; }

  private UserServiceErrors(string value) { Value = value; }

  public static UserServiceErrors NotFounded { get { return new UserServiceErrors("Usuário não encontrado!"); } }
  public static UserServiceErrors InvalidName { get { return new UserServiceErrors("Nome inválido!"); } }
  public static UserServiceErrors InvalidEmail { get { return new UserServiceErrors("Email inválido!"); } }
  public static UserServiceErrors EmailAlreadyInUse { get { return new UserServiceErrors("Email já em uso!"); } }
  public static UserServiceErrors InvalidPassword { get { return new UserServiceErrors("Senha inválida!"); } }
  public static UserServiceErrors NotIqualPassword { get { return new UserServiceErrors("Senhas não conferem!"); } }
}

public class CategoryServiceErrors {
  public string Value { get; private set; }

  private CategoryServiceErrors(string value) { Value = value; }

  public static CategoryServiceErrors NotFounded { get { return new CategoryServiceErrors("Categoria não encontrada!"); } }
  public static CategoryServiceErrors InvalidName { get { return new CategoryServiceErrors("Nome inválido!"); } }
  public static CategoryServiceErrors NameAlreadyExists { get { return new CategoryServiceErrors("Categoria já existe"); } }
}
