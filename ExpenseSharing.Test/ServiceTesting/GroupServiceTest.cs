using Business.AppService;
using DataAccess.Entities;
using DataAccess.UoW;
using Moq;
using Shared.DTOs;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseSharing.Test.ServiceTesting
{
    public class GroupServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GroupService _groupService;

        public GroupServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _groupService = new GroupService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateGroup_ShouldReturnSuccessMessage()
        {
            // Arrange
            var groupDto = new GroupDTO
            {
                GroupName = "DPCODE",
                GroupDescription = "edu-hub",
                TeamMembers = "test@gmail.com,test1@gmail.com",
                TotalMembers = 0
            };
            var user1 = new ApplicationUser
            {
                Name = "test",
                Email = "test@gmail.com",
                CurrentBalance = 0,
                Roles = "User",
                GroupsName = ""
            };
            var user2 = new ApplicationUser
            {
                Name = "test1",
                Email = "test1@gmail.com",
                CurrentBalance = 0,
                Roles = "User",
                GroupsName = ""
            };

            _unitOfWorkMock.Setup(uow => uow.Accounts.FindByEmail("test@gmail.com")).Returns(user1);
            _unitOfWorkMock.Setup(uow => uow.Accounts.FindByEmail("test1@gmail.com")).Returns(user2);
            _unitOfWorkMock.Setup(uow => uow.Groups.CreateGroup(It.IsAny<GroupDTO>())).ReturnsAsync("1");
            _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _groupService.CreateGroup(groupDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("1", result.Message);
            Assert.Equal("test@gmail.com,test1@gmail.com", groupDto.TeamMembers);
            Assert.Equal(2, groupDto.TotalMembers);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task CreateGroup_InvalidMember_ReturnsErrorMessage()
        {
            // Arrange
            var groupDto = new GroupDTO
            {
                GroupName = "DPCODE",
                GroupDescription = "edu-hub",
                TeamMembers = "test3@gmail.com,test4@gmail.com",
                TotalMembers = 0
            };

            var user1 = new ApplicationUser
            {
                Name = "test3",
                Email = "test3@gmail.com",
                CurrentBalance = 0,
                Roles = "User",
                GroupsName = ""
            };

            _unitOfWorkMock.Setup(uow => uow.Accounts.FindByEmail("test3@gmail.com")).Returns(user1);
            _unitOfWorkMock.Setup(uow => uow.Accounts.FindByEmail("test4@gmail.com")).Returns((ApplicationUser)null);

            // Act
            var result = await _groupService.CreateGroup(groupDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Only registered users can be added to the team", result.Message);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }

        [Fact]
        public async Task DeleteGroup_ExistingGroup_ReturnsSuccessMessage()
        {
            // Arrange
            var groupId = 551;
            var group = new Group
            {
                Id = groupId,
                GroupName = "DPCODE",
                GroupDescription = "edu-hub",
                TeamMembers = "test6@gmail.com,test7@gmail.com",
                TotalMembers = 2
            };
            var expense1 = new Expense
            {
                Id = 501,
                GroupId = groupId,
                IsSettled = true,
                Amount = 0,
                PaidBy = "test6@gmail.com",
                SplitAmong = "test6@gmail.com,test7@gmail.com",
                ContributedBy = "test6@gmail.com,test7@gmail.com",
                IndividualAmount = 50,
            };
            var expense2 = new Expense
            {
                Id = 502,
                GroupId = groupId,
                IsSettled = false,
                Amount = 50,
                PaidBy = "test6@gmail.com",
                SplitAmong = "test6@gmail.com,test7@gmail.com",
                ContributedBy = "test6@gmail.com",
                IndividualAmount = 50
            };
            var user1 = new ApplicationUser
            {
                Name = "test6",
                Email = "test6@gmail.com",
                CurrentBalance = 50,
                Roles = "User",
                GroupsName = "551,"
            };
            var user2 = new ApplicationUser
            {
                Name = "test7",
                Email = "test7@gmail.com",
                CurrentBalance = -50,
                Roles = "User",
                GroupsName = "551,"
            };

            _unitOfWorkMock.Setup(uow => uow.Groups.FindById(groupId)).Returns(group);
            _unitOfWorkMock.Setup(uow => uow.Expenses.GetAllExpensesOfAGroup(groupId)).Returns(new List<Expense> { expense1, expense2 }.AsQueryable());
            _unitOfWorkMock.Setup(uow => uow.Accounts.FindByEmail("test6@gmail.com")).Returns(user1);
            _unitOfWorkMock.Setup(uow => uow.Accounts.FindByEmail("test7@gmail.com")).Returns(user2);
            _unitOfWorkMock.Setup(uow => uow.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _groupService.DeleteGroup(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Deleted successfully", result.Message);
            _unitOfWorkMock.Verify(uow => uow.Expenses.DeleteExpense(expense1.Id), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Expenses.DeleteExpense(expense2.Id), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Groups.DeleteGroup(groupId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task DeleteGroup_NonExistingGroup_ReturnsErrorMessage()
        {
            // Arrange
            var groupId = 1;

            _unitOfWorkMock.Setup(uow => uow.Groups.FindById(groupId)).Returns((Group)null);

            // Act
            var result = await _groupService.DeleteGroup(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("No such group exists", result.Message);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
    }
}
