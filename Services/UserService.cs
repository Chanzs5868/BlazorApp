using BlazorApp.Data;
using BlazorApp.Models;

namespace BlazorApp.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public User? GetByUsername(string username) =>
        _db.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

    public User Register(string username, string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Username = username, PasswordHash = hash };
        _db.Users.Add(user);
        _db.SaveChanges();
        return user;
    }

    public bool VerifyPassword(User user, string password) =>
        BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    
    public bool ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null) return false;

        if (!VerifyPassword(user, oldPassword)) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _db.SaveChanges();
        return true;
    }

    public User? GetById(int userId) => _db.Users.FirstOrDefault(u => u.Id == userId);
    
    public bool SetAdmin(string username, bool isAdmin)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
        if (user is null) return false;

        user.IsAdmin = isAdmin;
        _db.SaveChanges();
        return true;
    }
}