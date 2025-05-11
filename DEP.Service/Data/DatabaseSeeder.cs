using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DEP.Repository.Models;
using DEP.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using DEP.Service.Interfaces;
using Microsoft.Extensions.Configuration;

public static class DatabaseSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var encryption = scope.ServiceProvider.GetRequiredService<IEncryptionService>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Get the default password from appsettings
        var defaultPass = configuration.GetSection("UserSettings:DefaultPassword").Value;

        // Only seed if no users exist
        if (await db.Users.AnyAsync()) return;

        var encryptedUserName = encryption.Encrypt("admin");
        var encryptedName = encryption.Encrypt("Administrator");

        CreatePasswordHash(defaultPass, out var hash, out var salt);

        var adminUser = new User
        {
            Name = encryptedName,
            UserName = encryptedUserName,
            PasswordHash = hash,
            PasswordSalt = salt,
            UserRole = 0,
            PasswordExpiryDate = DateTime.Now.AddDays(-1),
            RefreshTokenExpiryDate = DateTime.Now.AddDays(1),
        };

        db.Users.Add(adminUser);
        await db.SaveChangesAsync();
    }


    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}
