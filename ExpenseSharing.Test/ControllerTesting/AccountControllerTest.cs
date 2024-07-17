using Business.AppService;
using ExpenseSharingApplication.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs;
using System.Security.Claims;

namespace ExpenseSharing.Test.ControllerTesting
{
    public class AccountControllerTest
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AccountController _controller;

        public AccountControllerTest()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountController(_mockAccountService.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "test@gmail.com", Password = "Test@123" };
            var loginResponse = new ResponseDTO<LoginResponseDTO>
            {
                IsSuccess = true,
                Data = new LoginResponseDTO { Id = 1, Name = "Test User", Email = "test@gmail.com", Token = "token", Role = "User" }
            };

            _mockAccountService.Setup(service => service.Login(loginDTO)).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(loginResponse.Data, result.Data);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsFailure()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "wronguser@gmail.com", Password = "Wronguser" };
            var loginResponse = new ResponseDTO<LoginResponseDTO> { IsSuccess = false, Message = "Invalid credentials" };

            _mockAccountService.Setup(service => service.Login(loginDTO)).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid credentials", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public void FindByEmail_ExistingUser_ReturnsUser()
        {
            // Arrange
            var email = "test@gmail.com";
            var userResponse = new ResponseDTO<UserDTO>
            {
                IsSuccess = true,
                Message = "User Information",
                Data = new UserDTO { Id = 1, Name = "Test User", Email = "test@gmail.com", Role = "User" }
            };

            _mockAccountService.Setup(service => service.FindByEmail(email)).Returns(userResponse);

            // Act
            var result = _controller.FindByEmail(email) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = result.Value as ResponseDTO<UserDTO>;
            Assert.True(response.IsSuccess);
            Assert.Equal("User Information", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(email, response.Data.Email);
        }

        [Fact]
        public void FindByEmail_NonExistingUser_ReturnsFailure()
        {
            // Arrange
            var email = "wronguser@gmail.com";
            var userResponse = new ResponseDTO<UserDTO> { IsSuccess = false, Message = "User not found" };

            _mockAccountService.Setup(service => service.FindByEmail(email)).Returns(userResponse);

            // Act
            var result = _controller.FindByEmail(email) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = result.Value as ResponseDTO<UserDTO>;
            Assert.False(response.IsSuccess);
            Assert.Equal("User not found", response.Message);
            Assert.Null(response.Data);
        }

        [Fact]
        public void GetAllGroupsByUserEmail_ExistingUser_ReturnsGroups()
        {
            // Arrange
            var email = "test@gmail.com";
            var groupsResponse = new ResponseDTO<IEnumerable<GroupInfoDto>>
            {
                IsSuccess = true,
                Data = new List<GroupInfoDto>
                {
                    new GroupInfoDto { Id = 1, GroupName = "Group1", GroupDescription = "Description1", TeamMembers="test@gmail.com,test1@gmail.com" },
                    new GroupInfoDto { Id = 2, GroupName = "Group2", GroupDescription = "Description2",TeamMembers="test1@gmail.com,test2@gmail.com" }
                }
            };

            _mockAccountService.Setup(service => service.GetAllGroupsByUserEmail(email)).Returns(groupsResponse);

            var userClaims = new List<Claim> { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Role, "User") };
            var userIdentity = new ClaimsIdentity(userClaims, "mock");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = _controller.GetAllGroupsByUserEmail(email) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = result.Value as ResponseDTO<IEnumerable<GroupInfoDto>>;
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count());
        }

        [Fact]
        public void GetAllGroupsByUserEmail_NonExistingUser_ReturnsFailure()
        {
            // Arrange
            var email = "nonexistent@gmail.com";
            var groupsResponse = new ResponseDTO<IEnumerable<GroupInfoDto>> { IsSuccess = false, Message = "User not found" };

            _mockAccountService.Setup(service => service.GetAllGroupsByUserEmail(email)).Returns(groupsResponse);

            var userClaims = new List<Claim> { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Role, "User") };
            var userIdentity = new ClaimsIdentity(userClaims, "mock");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = _controller.GetAllGroupsByUserEmail(email) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = result.Value as ResponseDTO<IEnumerable<GroupInfoDto>>;
            Assert.False(response.IsSuccess);
            Assert.Equal("User not found", response.Message);
            Assert.Null(response.Data);
        }
    }
}
