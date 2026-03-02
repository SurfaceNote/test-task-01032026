namespace UserService.Domain.Entities;

public class User(string name, string passwordHash)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
    public string PasswordHash { get; private set; } = passwordHash;
}