using Xunit;
using Moq;
using bingGooAPI.Controllers;
using bingGooAPI.Interfaces;
using bingGooAPI.Models;
using bingGooAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UnitTest
{
    public class AuthTest
    {
        private readonly Mock<IAuthService> _mockAuth;
        private readonly AuthController _controller;

        public AuthTest()
        {
            _mockAuth = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuth.Object);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "admin",
                Password = "123"
            };

            var user = new User
            {
                Id = 1,
                Username = "admin",
                FullName = "Administrator",
                RoleName = "Admin",
                outLetId = 10
            };

            _mockAuth.Setup(x => x.LoginAsync(request.Username, request.Password))
                .ReturnsAsync((true, "", "fake-jwt-token", user));


            var result = await _controller.Login(request);

         
            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;

            var accessTokenProperty = value.GetType().GetProperty("access_token");
            var accessToken = accessTokenProperty.GetValue(value)?.ToString();

            var userProperty = value.GetType().GetProperty("user");
            var userObject = userProperty.GetValue(value);

            var usernameProperty = userObject.GetType().GetProperty("username");
            var username = usernameProperty.GetValue(userObject)?.ToString();

            var roleProperty = userObject.GetType().GetProperty("roleName");
            var role = roleProperty.GetValue(userObject)?.ToString();

        
            Assert.Equal("fake-jwt-token", accessToken);
            Assert.Equal("admin", username);
            Assert.Equal("Admin", role);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenFailed()
        {
          
            var request = new LoginRequest
            {
                Username = "wrong",
                Password = "wrong"
            };

            _mockAuth.Setup(x => x.LoginAsync(request.Username, request.Password))
                .ReturnsAsync((false, "Invalid username or password", "", null));

   
            var result = await _controller.Login(request);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

            var message = unauthorizedResult.Value?.ToString();

            Assert.Equal("Invalid username or password", message);
        }
    }
}