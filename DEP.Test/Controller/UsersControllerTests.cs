using DEP.Controllers;
using DEP.Repository.Models;
using DEP.Repository.ViewModels;
using DEP.Service.Interfaces;
using DEP.Service.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DEP.Test.Controller
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _controller = new UsersController(_mockUserService.Object);
        }

        private void SetUserRole(UserRole role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, role.ToString()) // Convert the enum to string
            };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);

            _mockHttpContextAccessor.Setup(_ => _.HttpContext.User).Returns(principal);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Name = "Alice", UserRole = UserRole.Administrator },
                new User { UserId = 2, Name = "Bob", UserRole = UserRole.Uddannelsesleder }
            };

            // Mock the service to return a list of users
            _mockUserService.Setup(x => x.GetUsers()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Ensure it's OkObjectResult
            var model = Assert.IsAssignableFrom<List<User>>(okResult.Value); // Expect List<User>

            // Check the count and values match
            Assert.Equal(2, model.Count);
            Assert.Equal("Alice", model[0].Name);
            Assert.Equal("Bob", model[1].Name);
        }

        [Fact]
        public async Task GetEducationBosses_ReturnsOkWithBosses()
        {
            var bosses = new List<EducationBossViewModel>
            {
                new EducationBossViewModel { UserId = 1, Name = "Boss 1", UserRole = UserRole.Uddannelseschef }
            };

            // Adjust the mock setup to match the expected return type
            _mockUserService.Setup(x => x.GetEducationBosses()).ReturnsAsync(bosses);

            var result = await _controller.GetEducationBosses();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<List<EducationBossViewModel>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsOk()
        {
            var user = new User
            {
                UserId = 1,
                UserName = "TestUser"
            };
            _mockUserService.Setup(x => x.GetUserById(1)).ReturnsAsync(user);

            var result = await _controller.GetUserById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<ApiResponse<User>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("TestUser", response.Data.UserName);
        }

        [Fact]
        public async Task GetUserById_UserNotFound_ReturnsNotFound()
        {
            _mockUserService.Setup(x => x.GetUserById(99)).ReturnsAsync((User)null);

            var result = await _controller.GetUserById(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsAssignableFrom<ApiResponse<object>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task AddUser_UsernameExists_ReturnsBadRequest()
        {
            var viewModel = new AddUserViewModel { Username = "existing" };
            _mockUserService.Setup(x => x.GetUserByUsername(viewModel.Username)).ReturnsAsync(new User());

            var result = await _controller.AddUser(viewModel);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsAssignableFrom<ApiResponse<object>>(badRequest.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task AddUser_ValidUser_ReturnsOk()
        {
            var viewModel = new AddUserViewModel { Username = "newuser" };
            var expectedUser = new UserViewModel { UserId = 1, Name = "newuser" };

            _mockUserService.Setup(x => x.GetUserByUsername(viewModel.Username)).ReturnsAsync((User)null);
            _mockUserService.Setup(x => x.AddUser(viewModel)).ReturnsAsync(expectedUser);

            var result = await _controller.AddUser(viewModel);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<ApiResponse<UserViewModel>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("newuser", response.Data.Name);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenDeleted()
        {
            _mockUserService.Setup(x => x.DeleteUser(1)).ReturnsAsync(true);

            var result = await _controller.DeleteUser(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ReturnsBadRequest_WhenNotDeleted()
        {
            _mockUserService.Setup(x => x.DeleteUser(99)).ReturnsAsync(false);

            var result = await _controller.DeleteUser(99);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
