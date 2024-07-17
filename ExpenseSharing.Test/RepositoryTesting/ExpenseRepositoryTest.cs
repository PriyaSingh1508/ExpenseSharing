using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace ExpenseSharing.Test.RepositoryTesting
{
    public class ExpenseRepositoryTest
    {
        private readonly DbContextOptions<ExpenseDbContext> _options;

        public ExpenseRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ExpenseDbContext>()
                .UseInMemoryDatabase(databaseName: "ExpenseTestDb")
                .Options;
        }

        [Fact]
        public async Task CreateExpense_ValidExpense_ReturnsTrue()
        {
            using var context = new ExpenseDbContext(_options);
            // Arrange
            var repository = new ExpenseRepository(context);
            var expenseDTO = new ExpenseDTO
            {
                GroupId = 1,
                Description = "New Test Expense",
                Amount = 1600,
                PaidBy = "test@gmail.com",
                Date = DateTime.UtcNow,
                SplitAmong = "test@gmail.com,test1@gmail.com",
                IndividualAmount = 800,
            };

            // Act
            var result = await repository.CreateExpense(expenseDTO);
            var response = await repository.GetExpenseById(1);
            // Assert
            Assert.True(result);
            Assert.Equal(1, response.Id);
            Assert.Equal(1, response.GroupId);
            Assert.Equal("New Test Expense", response.Description);
            Assert.Equal(1600, response.Amount*2);
            Assert.Equal(expenseDTO.Date,response.Date);
            Assert.Equal(expenseDTO.PaidBy,response.PaidBy);
            Assert.Equal(expenseDTO.SplitAmong,response.SplitAmong);
            Assert.Equal(expenseDTO.IndividualAmount,response.IndividualAmount);
        }

        [Fact]
        public async Task CreateExpense_InvalidExpense_ReturnsFalse()
        {
            using var context = new ExpenseDbContext(_options);
            // Arrange
            var repository = new ExpenseRepository(context);
            ExpenseDTO expenseDTO = null;

            // Act
            var result = await repository.CreateExpense(expenseDTO);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetExpenseById_GroupIdExists_ReturnsExpense()
        { // Arrange
            using var context = new ExpenseDbContext(_options);         
            var repository = new ExpenseRepository(context);
            var expenseDTO = new ExpenseDTO
            {
                GroupId = 1,
                Description = "New Test Expense",
                Amount = 1600,
                PaidBy = "test@gmail.com",
                Date = DateTime.UtcNow,
                SplitAmong = "test@gmail.com,test1@gmail.com",
                IndividualAmount = 800,
            };

            // Act
            var result = await repository.CreateExpense(expenseDTO);
            Assert.True(result);
            // Act
            var response = await repository.GetExpenseById(1);

            // Assert
           
            Assert.Equal(1, response.Id);
            Assert.Equal(1, response.GroupId);
            Assert.Equal("New Test Expense", response.Description);
            Assert.Equal(1600, response.Amount * 2);
            Assert.Equal(expenseDTO.PaidBy, response.PaidBy);
            Assert.Equal(expenseDTO.SplitAmong, response.SplitAmong);
            Assert.Equal(expenseDTO.IndividualAmount, response.IndividualAmount);
        }

        [Fact]
        public async Task GetExpenseById_NonExistingGroupId_ReturnsNull()
        {
            using var context = new ExpenseDbContext(_options);
            // Arrange
            var repository = new ExpenseRepository(context);

            // Act
            var result = await repository.GetExpenseById(9);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteExpense_ExistingExpense_ReturnsTrue()
        {
            // Arrange
            using var context = new ExpenseDbContext(_options);
            var repository = new ExpenseRepository(context);
            var expenseDTO = new ExpenseDTO
            {
                GroupId = 1,
                Description = "New Test Expense",
                Amount = 1600,
                PaidBy = "test@gmail.com",
                Date = DateTime.UtcNow,
                SplitAmong = "test@gmail.com,test1@gmail.com",
                IndividualAmount = 800,
            };
            var response = await repository.CreateExpense(expenseDTO);
            // Act
            var result = repository.DeleteExpense(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteExpense_NonExistingExpense_ReturnsFalse()
        {
            using var context = new ExpenseDbContext(_options);
            // Arrange
            var repository = new ExpenseRepository(context);

            // Act
            var result = repository.DeleteExpense(9);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllExpensesOfAGroup_ExistingGroup_ReturnsExpenses()
        {
            using var context = new ExpenseDbContext(_options);
            // Arrange
            var repository = new ExpenseRepository(context);
            var expenseDTO = new ExpenseDTO
            {
                GroupId = 2,
                Description = "New Test Expense",
                Amount = 1600,
                PaidBy = "test@gmail.com",
                Date = DateTime.UtcNow,
                SplitAmong = "test@gmail.com,test1@gmail.com",
                IndividualAmount = 800,
            };
            var response = await repository.CreateExpense(expenseDTO);
            Assert.True(response);
            var expenseDTO1 = new ExpenseDTO
            {
                GroupId = 2,
                Description = "New Test Expense 2",
                Amount = 2100,
                PaidBy = "test1@gmail.com",
                Date = DateTime.UtcNow,
                SplitAmong = "test1@gmail.com,test2@gmail.com,test3@gmail.com",
                IndividualAmount = 700,
            };
            var response1 = await repository.CreateExpense(expenseDTO);
            Assert.True(response1);

            // Act
            var result = repository.GetAllExpensesOfAGroup(2);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());

        }

        [Fact]
        public void GetAllExpensesOfAGroup_NonExistingGroup_ReturnsEmpty()
        {
            using var context = new ExpenseDbContext(_options);
            // Arrange
            var repository = new ExpenseRepository(context);

            // Act
            var result = repository.GetAllExpensesOfAGroup(999);

            // Assert
            Assert.Empty(result);
        }

    }
}
