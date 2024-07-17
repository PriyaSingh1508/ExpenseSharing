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
    public class ExpenseControllerTest
    {
        private readonly Mock<IExpenseService> _mockExpenseService;
        private readonly ExpenseController _controller;
        private readonly ClaimsPrincipal _user;

        public ExpenseControllerTest()
        {
            _mockExpenseService = new Mock<IExpenseService>();
            _controller = new ExpenseController(_mockExpenseService.Object);

            var userClaims = new List<Claim> { new Claim(ClaimTypes.Role, "User") };
            var userIdentity = new ClaimsIdentity(userClaims);
            _user = new ClaimsPrincipal(userIdentity);
        }

        [Fact]
        public async Task CreateExpense_User_ReturnsOkResult()
        {
            // Arrange
            var expense = new ExpenseDTO
            {
                GroupId = 1,
                Description = "Controller Expense",
                Amount = 1600,
                PaidBy = "test@gmail.com",
                Date = System.DateTime.UtcNow,
                SplitAmong = "test@gmail.com,test1@gmail.com",
                IndividualAmount = 800,
            };

            var response = new ResponseDTO<string> { IsSuccess = true, Message = "Success", Data = null };

            _mockExpenseService.Setup(service => service.CreateExpense(expense)).ReturnsAsync(response);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _user }
            };

            // Act
            var result = await _controller.CreateExpense(expense);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var value = okResult.Value as ResponseDTO<string>;
            Assert.NotNull(value);
            Assert.True(value.IsSuccess);
            Assert.Equal("Success", value.Message);
        }

        [Fact]
        public async Task UpdateExpense_ShouldReturnSuccessMessage()
        {
            // Arrange
            var expenseDto = new ExpenseDTO
            {
                Description = "Updated Expense",
                Amount = 150,
                SplitAmong = "test@gmail.com,test1@gmail.com",
            };
            var id = 1;

            var response = new ResponseDTO<string> { IsSuccess = true, Message = "Expense updated successfully", Data = null };

            _mockExpenseService.Setup(service => service.UpdateExpense(expenseDto, id)).ReturnsAsync(response);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _user }
            };

            // Act
            var result = await _controller.UpdateExpense(expenseDto, id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var value = result.Value as ResponseDTO<string>;
            Assert.NotNull(value);
            Assert.True(value.IsSuccess);
            Assert.Equal("Expense updated successfully", value.Message);
        }

        [Fact]
        public async Task AmountContributedByUser_ShouldReturnSuccessMessage()
        {
            // Arrange
            var amtSettlementDto = new UserAmtSettlementDTO
            {
                Email = "test@gmail.com",
                ExpenseId = 1,
            };

            var response = new ResponseDTO<string> { IsSuccess = true, Message = "Amount contributed successfully", Data = null };

            _mockExpenseService.Setup(service => service.AmountContributedByUser(amtSettlementDto)).ReturnsAsync(response);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _user }
            };

            // Act
            var result = await _controller.AmountContributedByUser(amtSettlementDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var value = result.Value as ResponseDTO<string>;
            Assert.NotNull(value);
            Assert.True(value.IsSuccess);
            Assert.Equal("Amount contributed successfully", value.Message);
        }

        [Fact]
        public async Task ExpenseSettlement_ShouldReturnSuccessMessage()
        {
            // Arrange
            var expenseId = 1;

            var response = new ResponseDTO<string> { IsSuccess = true, Message = "Expense settled successfully", Data = null };

            _mockExpenseService.Setup(service => service.ExpenseSettlement(expenseId)).ReturnsAsync(response);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _user }
            };

            // Act
            var result = await _controller.ExpenseSettlement(expenseId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var value = result.Value as ResponseDTO<string>;
            Assert.NotNull(value);
            Assert.True(value.IsSuccess);
            Assert.Equal("Expense settled successfully", value.Message);
        }
    }
}
