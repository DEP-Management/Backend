using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Repository.ViewModels;
using DEP.Service.Interfaces;
using DEP.Service.Services;
using DEP.Service.ViewModels;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace DEP.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly Mock<IPersonRepository> _personRepositoryMock = new();
        private readonly Mock<IEncryptionService> _encryptionServiceMock = new();

        private UserService CreateService()
        {
            return new UserService(
                _userRepositoryMock.Object,
                _authServiceMock.Object,
                _configurationMock.Object,
                _personRepositoryMock.Object,
                _encryptionServiceMock.Object
            );
        }

        [Fact]
        public async Task AddUser_Should_CreateUserWithHashedPassword()
        {
            // Arrange
            var viewModel = new AddUserViewModel
            {
                Username = "newuser",
                Name = "Test Bruger",
                LocationId = 1,
                DepartmentId = 2,
                EducationBossId = 3,
                UserRole = UserRole.Uddannelsesleder
            };

            _configurationMock.Setup(c => c.GetSection("UserSettings:DefaultPassword").Value)
                              .Returns("default123");

            var passwordHash = new byte[32];
            var passwordSalt = new byte[32];

            _authServiceMock
                .Setup(x => x.CreatePasswordHash("default123", out passwordHash, out passwordSalt));

            _encryptionServiceMock
                .Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns((string s) => s);
            _encryptionServiceMock
                .Setup(x => x.Decrypt(It.IsAny<string>()))
                .Returns((string s) => s);

            _userRepositoryMock
                .Setup(x => x.AddUser(It.IsAny<User>()))
                .ReturnsAsync((User u) =>
                {
                    // Simuler dekrypteret bruger
                    u.Name = "Test Bruger";
                    u.Location = new Location { Name = "Test Lokation" };
                    u.Department = new Department { Name = "Test Afdeling" };
                    u.EducationBoss = new User { Name = "Bossman" };
                    return u;
                });

            var service = CreateService();

            // Act
            var result = await service.AddUser(viewModel);

            // Assert
            Assert.Equal("Test Bruger", result.Name);
            _userRepositoryMock.Verify(r => r.AddUser(It.IsAny<User>()), Times.Once);
        }



        [Fact]
        public async Task UpdateUserFromViewModel_Should_ClearPersonRole_WhenUserRoleChanged()
        {
            // Arrange
            var existingUser = new User
            {
                UserId = 1,
                Name = "Old Name",
                UserRole = UserRole.Uddannelsesleder
            };

            var persons = new List<Person>
            {
                new Person { EducationalLeaderId = 1 },
                new Person { EducationalLeaderId = 1 }
            };

            _userRepositoryMock.Setup(r => r.GetUserById(1)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(r => r.UpdateUser(It.IsAny<User>())).ReturnsAsync(true);
            _personRepositoryMock.Setup(p => p.GetPersonsByUserId(1)).ReturnsAsync(persons);
            _personRepositoryMock.Setup(p => p.UpdatePersons(It.IsAny<List<Person>>())).Returns(Task.CompletedTask);

            var service = CreateService();

            var viewModel = new UserViewModel
            {
                UserId = 1,
                Name = "New Name",
                UserRole = UserRole.Controller
            };

            // Act
            var result = await service.UpdateUserFromViewModel(viewModel);

            // Assert
            Assert.True(result);
            Assert.All(persons, p => Assert.Null(p.EducationalLeaderId));
            _personRepositoryMock.Verify(p => p.UpdatePersons(It.IsAny<List<Person>>()), Times.Once);
        }

        [Fact]
        public async Task GetEducationBosses_Should_ReturnMappedViewModels()
        {
            // Arrange
            var bosses = new List<User>
            {
                new User { UserId = 1, Name = "Boss 1", UserRole = UserRole.Uddannelseschef },
                new User { UserId = 2, Name = "Boss 2", UserRole = UserRole.Uddannelseschef }
            };

            _userRepositoryMock.Setup(r => r.GetEducationBossesExcel()).ReturnsAsync(bosses);

            var service = CreateService();

            // Act
            var result = await service.GetEducationBosses();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Name == "Boss 1");
        }
    }
}
