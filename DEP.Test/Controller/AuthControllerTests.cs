using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DEP.Controllers;
using DEP.Service.Interfaces;
using DEP.Service.ViewModels;
using DEP.Repository.Models;
using static DEP.Service.Services.AuthService;
using System;

namespace DEP.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> mockAuthService = new();
        private readonly Mock<IUserService> mockUserService = new();
        private readonly AuthController controller;

        public AuthControllerTests()
        {
            controller = new AuthController(mockAuthService.Object, mockUserService.Object);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalid()
        {
            var login = new LoginViewModel();
            mockAuthService.Setup(s => s.Login(login)).ReturnsAsync((AuthenticatedResponse)null);

            var result = await controller.Login(login);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenValid()
        {
            var login = new LoginViewModel();
            var expected = new AuthenticatedResponse();
            mockAuthService.Setup(s => s.Login(login)).ReturnsAsync(expected);

            var result = await controller.Login(login);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsNotFound_WhenUserNotFound()
        {
            mockAuthService.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordViewModel>()))
                .ReturnsAsync(ChangePasswordResult.UserNotFound);

            var result = await controller.ChangePassword(new ChangePasswordViewModel());

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.False(((ApiResponse<object>)notFound.Value).Success);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenOldPasswordIsWrong()
        {
            mockAuthService.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordViewModel>()))
                .ReturnsAsync(ChangePasswordResult.WrongOldPassword);

            var result = await controller.ChangePassword(new ChangePasswordViewModel());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(((ApiResponse<object>)badRequest.Value).Success);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenSuccess()
        {
            mockAuthService.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordViewModel>()))
                .ReturnsAsync(ChangePasswordResult.Success);

            var result = await controller.ChangePassword(new ChangePasswordViewModel());

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(((ApiResponse<object>)okResult.Value).Success);
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenFailed()
        {
            mockAuthService.Setup(x => x.ResetPassword(1)).ReturnsAsync(false);

            var result = await controller.ResetPassword(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenSuccess()
        {
            mockAuthService.Setup(x => x.ResetPassword(1)).ReturnsAsync(true);

            var result = await controller.ResetPassword(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task RefreshToken_ReturnsBadRequest_WhenNull()
        {
            var result = await controller.RefreshToken(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RefreshToken_ReturnsUnauthorized_WhenUserNotFoundOrExpired()
        {
            var response = new AuthenticatedResponse { UserId = 1 };
            mockUserService.Setup(x => x.GetUserById(1)).ReturnsAsync((User)null);

            var result = await controller.RefreshToken(response);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task RefreshToken_ReturnsUnauthorized_WhenTokenMismatch()
        {
            var response = new AuthenticatedResponse
            {
                UserId = 1,
                RefreshToken = "token"
            };

            var user = new User
            {
                RefreshToken = "wrong-token",
                RefreshTokenExpiryDate = DateTime.Now.AddHours(1)
            };

            mockUserService.Setup(x => x.GetUserById(1)).ReturnsAsync(user);

            var result = await controller.RefreshToken(response);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task RefreshToken_ReturnsOk_WhenValid()
        {
            var response = new AuthenticatedResponse
            {
                UserId = 1,
                RefreshToken = "token"
            };

            var user = new User
            {
                UserId = 1,
                RefreshToken = "token",
                RefreshTokenExpiryDate = DateTime.Now.AddHours(1)
            };

            mockUserService.Setup(x => x.GetUserById(1)).ReturnsAsync(user);
            mockAuthService.Setup(x => x.CreateJwtToken(user)).Returns("new-token");
            mockAuthService.Setup(x => x.CreateRefreshToken()).ReturnsAsync("new-refresh");

            mockUserService.Setup(x => x.UpdateUser(It.IsAny<User>())).ReturnsAsync(true);


            var result = await controller.RefreshToken(response);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<AuthenticatedResponse>(ok.Value);

            Assert.Equal("new-token", returned.AccessToken);
            Assert.Equal("new-refresh", returned.RefreshToken);
        }
    }
}
