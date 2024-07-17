using Business.AppService;
using DataAccess.Entities;
using DataAccess.UoW;
using Moq;
using Shared.DTOs;
using DataAccess.Repositories;

namespace ExpenseSharing.Test.ServiceTesting
{
    public class ExpenseServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<IGroupRepository> _groupRepositoryMock;
        private readonly ExpenseService _expenseService;

        public ExpenseServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _expenseRepositoryMock = new Mock<IExpenseRepository>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _groupRepositoryMock = new Mock<IGroupRepository>();

            _unitOfWorkMock.SetupGet(uow => uow.Expenses).Returns(_expenseRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(uow => uow.Accounts).Returns(_accountRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(uow => uow.Groups).Returns(_groupRepositoryMock.Object);

            _expenseService = new ExpenseService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateExpense_ValidExpense_ReturnsSuccessMessage()
        {
            // Arrange
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

            var group = new Group
            {
                Id = 1,
                TeamMembers = "test@gmail.com,test1@gmail.com",
                GroupName = "DPCODE",
                GroupDescription = "edu-hub",
                TotalMembers = 2
            };

            _groupRepositoryMock.Setup(gr => gr.FindById(It.IsAny<int>())).Returns(group);
            _expenseRepositoryMock.Setup(er => er.CreateExpense(It.IsAny<ExpenseDTO>())).ReturnsAsync(true);
            _accountRepositoryMock.Setup(ar => ar.FindByEmail(It.IsAny<string>())).Returns(new ApplicationUser
            {
                Email = "test@gmail.com",
                Name = "Test User",
                CurrentBalance = 0,
                Roles = "User",
                GroupsName = "1"
            });
            _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _expenseService.CreateExpense(expenseDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Expense created successfully", result.Message);
            _unitOfWorkMock.Verify(uow => uow.BeginTransaction(), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
        }

        [Fact]
        public async Task CreateExpense_InvalidGroupId_ReturnsErrorMessage()
        {
            // Arrange
            var expenseDTO = new ExpenseDTO
            {
                GroupId = 993,
                Description = "Test Expense",
                Amount = 900,
                Date = DateTime.UtcNow,
                PaidBy = "user1@gmail.com",
                SplitAmong = "user1@gmail.com,user2@gmail.com,user3@gmail.com",
                IndividualAmount = 300,
            };

            _groupRepositoryMock.Setup(gr => gr.FindById(It.IsAny<int>())).Returns((Group)null);

            // Act
            var result = await _expenseService.CreateExpense(expenseDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("The Group Id does not exist", result.Message);
        }

        [Fact]
        public async Task ExpenseSettlement_ExistingExpense_ReturnsSettledMessage()
        {
            // Arrange
            var existingExpense = new Expense
            {
                Id = 2,
                GroupId = 10,
                Description = "Test Expense",
                Amount = 0,
                Date = DateTime.UtcNow,
                PaidBy = "user1@gmail.com",
                SplitAmong = "user1@gmail.com,user2@gmail.com,user3@gmail.com",
                IndividualAmount = 300,
                ContributedBy = "user1@gmail.com,user2@gmail.com,user3@gmail.com",
                IsSettled = false
            };

            // Setup mock methods
            _expenseRepositoryMock.Setup(er => er.GetExpenseById(It.IsAny<int>())).ReturnsAsync(existingExpense);
            _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _expenseService.ExpenseSettlement(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Expense Settled", result.Message);
            Assert.True(existingExpense.IsSettled);
        }

        [Fact]
        public async Task ExpenseSettlement_NonExistingExpense_ReturnsErrorMessage()
        {
            // Arrange
            _expenseRepositoryMock.Setup(er => er.GetExpenseById(It.IsAny<int>())).ReturnsAsync((Expense)null);

            // Act
            var result = await _expenseService.ExpenseSettlement(99);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("This expense does not exist", result.Message);
        }

        [Fact]
        public async Task AmountContributedByUser_ValidUser_ReturnsSuccessMessage()
        {
            // Arrange
            var amtSettlementDTO = new UserAmtSettlementDTO
            {
                Email = "user2@gmail.com",
                ExpenseId = 1
            };

            var user = new ApplicationUser
            {
                Name = "user2",
                Email = "user2@gmail.com",
                CurrentBalance = -50,
                Roles = "User",
                GroupsName = "10"
            };
            var expense = new Expense
            {
                Id = 1,
                GroupId = 10,
                Description = "Test Expense",
                Date = DateTime.UtcNow,
                Amount = 100,
                IndividualAmount = 50,
                SplitAmong = "user1@gmail.com,user2@gmail.com",
                PaidBy = "user1@gmail.com",
                IsSettled = false
            };
            var expenseCreator = new ApplicationUser
            {
                Name = "user1",
                Email = "user1@gmail.com",
                CurrentBalance = 0,
                Roles = "User",
                GroupsName = "10"
            };

            _accountRepositoryMock.Setup(ar => ar.FindByEmail(It.Is<string>(email => email == "user2@gmail.com"))).Returns(user);
            _expenseRepositoryMock.Setup(er => er.GetExpenseById(It.Is<int>(id => id == 1))).ReturnsAsync(expense);
            _accountRepositoryMock.Setup(ar => ar.FindByEmail(It.Is<string>(email => email == "user1@gmail.com"))).Returns(expenseCreator);
            _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _expenseService.AmountContributedByUser(amtSettlementDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Individual Expense Settled", result.Message);
            Assert.Equal(0, user.CurrentBalance);
            Assert.Equal(50, expenseCreator.CurrentBalance);
            Assert.Contains("user2@gmail.com", expense.ContributedBy);
        }

        [Fact]
        public async Task AmountContributedByUser_InvalidUser_ReturnsErrorMessage()
        {
            // Arrange
            var amtSettlementDTO = new UserAmtSettlementDTO
            {
                Email = "user3@gmail.com",
                ExpenseId = 2
            };
            var expense = new Expense
            {
                Id = 2,
                GroupId = 10,
                Description = "Test Expense",
                Date = DateTime.UtcNow,
                Amount = 100,
                IndividualAmount = 50,
                SplitAmong = "user1@gmail.com,user2@gmail.com",
                PaidBy = "user1@gmail.com",
                IsSettled = false
            };

            _accountRepositoryMock.Setup(ar => ar.FindByEmail(It.Is<string>(email => email == "user3@gmail.com"))).Returns((ApplicationUser)null);
            _expenseRepositoryMock.Setup(er => er.GetExpenseById(It.Is<int>(id => id == 2))).ReturnsAsync(expense);

            // Act
            var result = await _expenseService.AmountContributedByUser(amtSettlementDTO);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No record found corresponding to user", result.Message);
        }

        [Fact]
        public async Task UpdateExpense_ExistingExpense_ReturnsSuccessMessage()
        {
            // Arrange
            var expenseDTO = new ExpenseDTO
            {
                GroupId = 5,
                Description = "Updated Expense",
                Amount = 1200,
                Date = DateTime.UtcNow,
                PaidBy = "user1@gmail.com",
                SplitAmong = "user1@gmail.com,user2@gmail.com,user3@gmail.com",
                IndividualAmount = 400,
            };
            var existingExpense = new Expense
            {
                Id = 1,
                GroupId = 5,
                Description = "Expense",
                Amount = 800,
                Date = DateTime.UtcNow,
                SplitAmong = "user1@gmail.com,user2@gmail.com,user3@gmail.com",
                IndividualAmount = 400
            };
            _expenseRepositoryMock.Setup(er => er.GetExpenseById(It.IsAny<int>())).ReturnsAsync(existingExpense);
            _expenseRepositoryMock.Setup(er => er.UpdateExpense(It.IsAny<ExpenseDTO>(), It.IsAny<int>())).ReturnsAsync("Updated successfully");
            _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _expenseService.UpdateExpense(expenseDTO, 1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated successfully", result.Message);
            Assert.Equal("Expense", existingExpense.Description);
            Assert.Equal(800, existingExpense.Amount);
        }

        [Fact]
        public async Task UpdateExpense_NonExistingExpense_ReturnsErrorMessage()
        {
            // Arrange
            var expenseDTO = new ExpenseDTO
            {
                GroupId = 5,
                Description = "Updated Expense",
                Amount = 1200,
                Date = DateTime.UtcNow,
                PaidBy = "user1@gmail.com",
                SplitAmong = "user1@gmail.com,user2@gmail.com,user3@gmail.com",
                IndividualAmount = 400,
            };

            _expenseRepositoryMock.Setup(er => er.GetExpenseById(It.IsAny<int>())).ReturnsAsync((Expense)null);

            // Act
            var result = await _expenseService.UpdateExpense(expenseDTO, 1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("This expense does not exist", result.Message);
        }
    }
}
