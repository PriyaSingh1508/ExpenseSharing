using Business.AppService;
using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.UoW;
using Moq;
using Shared.DTOs;

namespace ExpenseSharing.Test.ServiceTesting
{
    public class AccountServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly AccountService _accountService;

        public AccountServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();

            _mockUnitOfWork.Setup(uow => uow.Accounts).Returns(_mockAccountRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.Groups).Returns(_mockGroupRepository.Object);

            _accountService = new AccountService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "test@gmail.com", Password = "password" };
            var loginResponse = new LoginResponseDTO { Id = 1, Name = "Test User", Email = "test@gmail.com", Token = "token", Role = "User" };

            _mockAccountRepository.Setup(repo => repo.Login(loginDTO)).ReturnsAsync(loginResponse);

            // Act
            var result = await _accountService.Login(loginDTO);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(loginResponse, result.Data);
            Assert.Equal(string.Empty, result.Message);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsFailure()
        {
            // Arrange
            var loginDTO = new LoginDTO { Email = "abc@gmil.com", Password = "invalidPassword" };
            var loginResponse = new LoginResponseDTO() { Id = -1, Email = "", Token = "" };
           _mockAccountRepository.Setup(repo => repo.Login(loginDTO)).ReturnsAsync(loginResponse);

            // Act
            var result = await _accountService.Login(loginDTO);

            // Assert
            Assert.Equal(-1,result.Data.Id);
            Assert.Empty(result.Data.Email);
        }

        [Fact]
        public void FindByEmail_ExistingUser_ReturnsUser()
        {
            // Arrange
            var email = "testuser@gmail.com";
            var user = new ApplicationUser { Id = 301, Name = "Test User", Email = "testuser@gmail.com", Roles = "User", GroupsName = "1,2" };

            _mockAccountRepository.Setup(repo => repo.FindByEmail(email)).Returns(user);

            // Act
            var result = _accountService.FindByEmail(email);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("User Information", result.Message);
            Assert.Equal(email, result.Data.Email);
        }

        [Fact]
        public void FindByEmail_NonExistingUser_ReturnsFailure()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockAccountRepository.Setup(repo => repo.FindByEmail(email)).Returns((ApplicationUser)null);

            // Act
            var result = _accountService.FindByEmail(email);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public void GetAllGroupsByUserEmail_ExistingUser_ReturnsGroups()
        {
            // Arrange
            var email = "testuser1@gmail.com";
            var user = new ApplicationUser { Id = 801, Name = "Test User", Email = "testuser1@gmail.com", Roles = "User", GroupsName = "1,2" };
            var group1 = new Group { Id = 1, GroupName = "Group1", GroupDescription = "Description1", GroupCreatedDate = DateTime.Now, TeamMembers = "testuser1@gmail.com", TotalMembers = 1 };
            var group2 = new Group { Id = 2, GroupName = "Group2", GroupDescription = "Description2", GroupCreatedDate = DateTime.Now, TeamMembers = "testuser1@gmail.com", TotalMembers = 1 };

            _mockAccountRepository.Setup(repo => repo.FindByEmail(email)).Returns(user);
            _mockGroupRepository.Setup(repo => repo.FindById(1)).Returns(group1);
            _mockGroupRepository.Setup(repo => repo.FindById(2)).Returns(group2);

            // Act
            var result = _accountService.GetAllGroupsByUserEmail(email);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public void GetAllGroupsByUserEmail_NonExistingUser_ReturnsFailure()
        {
            // Arrange
            var email = "invalid@gmail.com";
            _mockAccountRepository.Setup(repo => repo.FindByEmail(email)).Returns((ApplicationUser)null);

            // Act
            var result = _accountService.GetAllGroupsByUserEmail(email);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }
    }
}
