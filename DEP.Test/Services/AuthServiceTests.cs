using Xunit;
using Moq;
using System.Threading.Tasks;
using DEP.Repository.Interfaces;
using DEP.Service.Services;
using Microsoft.Extensions.Configuration;
using DEP.Repository.Models;
using DEP.Service.ViewModels;
using System.Collections.Generic;
using DEP.Service.Interfaces;

namespace DEP.Test.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> userRepoMock;
        private readonly Mock<IConfiguration> configMock;
        private readonly Mock<IEncryptionService> encryptionMock;
        private readonly AuthService authService;

        public AuthServiceTests()
        {
            userRepoMock = new Mock<IUserRepository>();
            configMock = new Mock<IConfiguration>();
            encryptionMock = new Mock<IEncryptionService>();
            authService = new AuthService(userRepoMock.Object, configMock.Object, encryptionMock.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsAuthenticatedResponse()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var configMock = new Mock<IConfiguration>();
            var encryptionServiceMock = new Mock<IEncryptionService>();

            var testUser = new User
            {
                UserId = 1,
                UserName = "EncryptedUsername",
                Name = "EncryptedName",
                PasswordExpiryDate = DateTime.Now.AddMonths(1),
                PasswordHash = new byte[64],
                PasswordSalt = new byte[64],
                UserRole = UserRole.Driftskoordinator
            };

            var loginModel = new LoginViewModel
            {
                Username = "testuser",
                Password = "password"
            };

            // Simuler dekryptering og match af brugernavn og navn
            encryptionServiceMock.Setup(e => e.Decrypt("EncryptedUsername"))
                                 .Returns("testuser");

            encryptionServiceMock.Setup(e => e.Decrypt("EncryptedName"))
                                 .Returns("Test Person");

            encryptionServiceMock.Setup(e => e.Encrypt("Test Person"))
                                 .Returns("EncryptedName");

            userRepoMock.Setup(r => r.GetUsers())
                        .ReturnsAsync(new List<User> { testUser });

            configMock.Setup(c => c.GetSection("AppSettings:Token").Value)
                      .Returns("supersecretkey1234567890");

            userRepoMock.Setup(r => r.UpdateUser(It.IsAny<User>()))
                        .ReturnsAsync(true);

            var authService = new AuthService(userRepoMock.Object, configMock.Object, encryptionServiceMock.Object);

            // Generér en hash/salt, der matcher password
            authService.CreatePasswordHash(loginModel.Password, out byte[] hash, out byte[] salt);
            testUser.PasswordHash = hash;
            testUser.PasswordSalt = salt;

            // Act
            var result = await authService.Login(loginModel);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessToken);
            Assert.NotEmpty(result.RefreshToken);
            Assert.True(result.PasswordExpiryDate > DateTime.Now);
        }

        [Fact]
        public async Task ChangePassword_WithCorrectOldPassword_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "oldPassword";
            var newPassword = "newPassword";

            var authService = new AuthService(userRepoMock.Object, configMock.Object, encryptionMock.Object);

            authService.CreatePasswordHash(oldPassword, out byte[] oldHash, out byte[] oldSalt);

            var user = new User
            {
                UserId = userId,
                PasswordHash = oldHash,
                PasswordSalt = oldSalt
            };

            var viewModel = new ChangePasswordViewModel
            {
                UserId = userId,
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            userRepoMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
            userRepoMock.Setup(r => r.UpdateUser(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await authService.ChangePassword(viewModel);

            // Assert
            Assert.Equal(AuthService.ChangePasswordResult.Success, result);
        }

        [Fact]
        public async Task ChangePassword_WithIncorrectOldPassword_ReturnsWrongOldPassword()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "oldPassword";
            var newPassword = "newPassword";

            var authService = new AuthService(userRepoMock.Object, configMock.Object, encryptionMock.Object);

            authService.CreatePasswordHash(oldPassword, out byte[] oldHash, out byte[] oldSalt);

            var user = new User
            {
                UserId = userId,
                PasswordHash = oldHash,
                PasswordSalt = oldSalt
            };

            var viewModel = new ChangePasswordViewModel
            {
                UserId = userId,
                OldPassword = "incorrectOldPassword", // Simulerer en forkert gammel adgangskode
                NewPassword = newPassword
            };

            userRepoMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);

            // Act
            var result = await authService.ChangePassword(viewModel);

            // Assert
            Assert.Equal(AuthService.ChangePasswordResult.WrongOldPassword, result);
        }


        [Fact]
        public async Task ResetPassword_WithValidUser_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var defaultPassword = "default123";

            var user = new User { UserId = userId };

            userRepoMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
            userRepoMock.Setup(r => r.UpdateUser(It.IsAny<User>())).ReturnsAsync(true);
            configMock.Setup(c => c.GetSection("AppSettings:Password").Value).Returns(defaultPassword);

            var authService = new AuthService(userRepoMock.Object, configMock.Object, encryptionMock.Object);

            // Act
            var result = await authService.ResetPassword(userId);

            // Assert
            Assert.True(result);
        }
    }
}
