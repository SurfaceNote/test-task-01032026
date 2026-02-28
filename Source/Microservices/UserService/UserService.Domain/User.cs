namespace UserService.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    
}