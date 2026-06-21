using System.Text.Json.Serialization;

namespace ElectroCorp.Platform.Iam.Domain.Model.Aggregates;

public partial class User
{
    public User()
    {
        Username = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(string username, string email, string passwordHash)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsDeleted = false;
    }

    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    
    [JsonIgnore] 
    public string PasswordHash { get; private set; }
    public bool IsDeleted { get; private set; }

    public User UpdateProfile(string username, string email)
    {
        Username = username;
        Email = email;
        return this;
    }

    public User UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        return this;
    }

    public User MarkAsDeleted()
    {
        IsDeleted = true;
        return this;
    }
}
