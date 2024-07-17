using Business.AppService;
using ExpenseSharingApplication.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Test.ControllerTesting
{
    public class GroupControllerTest
    {
        private readonly Mock<IGroupService> _mockGroupService;
        private readonly GroupController _controller;
        private readonly ClaimsPrincipal _user;
        private readonly ClaimsPrincipal _admin;

        public GroupControllerTest()
        {
            _mockGroupService = new Mock<IGroupService>();
            _controller = new GroupController(_mockGroupService.Object);

            var userClaims = new List<Claim> { new Claim(ClaimTypes.Role, "User") };
            var userIdentity = new ClaimsIdentity(userClaims);
            _user = new ClaimsPrincipal(userIdentity);

            var adminClaims = new List<Claim> { new Claim(ClaimTypes.Role, "Admin") };
            var adminIdentity = new ClaimsIdentity(adminClaims);
            _admin = new ClaimsPrincipal(adminIdentity);
        }

        [Fact]
        public async Task CreateGroup_User_ReturnsSuccess()
        {
            // Arrange
            var group = new GroupDTO
            {
                GroupName = "DPCODE",
                GroupDescription = "edu-hub",
                TeamMembers = "test@gmail.com, test1@gmail.com",
                TotalMembers = 2
            };

            var response = new ResponseDTO<string> { IsSuccess = true, Message = "GroupCreated", Data = null };

            _mockGroupService.Setup(service => service.CreateGroup(group)).ReturnsAsync(response);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _user }
            };

            // Act
            var result = await _controller.CreateGroup(group) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var value = result.Value as ResponseDTO<string>;
            Assert.NotNull(value);
            Assert.True(value.IsSuccess);
            Assert.Equal("GroupCreated", value.Message);
        }

        [Fact]
        public async Task DeleteGroup_User_ReturnsTrue()
        {
            // Arrange
            int groupId = 1;
            var response = new ResponseDTO<bool> { IsSuccess = true, Message = "GroupDeleted", Data = true };

            _mockGroupService.Setup(service => service.DeleteGroup(groupId)).ReturnsAsync(response);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _user }
            };

            // Act
            var result = await _controller.DeleteGroup(groupId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var value = result.Value as ResponseDTO<bool>;
            Assert.NotNull(value);
            Assert.True(value.IsSuccess);
            Assert.True(value.Data);
            Assert.Equal("GroupDeleted", value.Message);
        }

        [Fact]
        public async Task DeleteGroup_User_ReturnsFalse()
        {
            // Arrange
            int groupId = 1;
            var response = new ResponseDTO<bool> { IsSuccess = false, Message = "GroupNotDeleted", Data = false };

            _mockGroupService.Setup(service => service.DeleteGroup(groupId)).ReturnsAsync(response);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _user }
            };

            // Act
            var result = await _controller.DeleteGroup(groupId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var value = result.Value as ResponseDTO<bool>;
            Assert.NotNull(value);
            Assert.False(value.IsSuccess);
            Assert.False(value.Data);
            Assert.Equal("GroupNotDeleted", value.Message);
        }
    }
}
