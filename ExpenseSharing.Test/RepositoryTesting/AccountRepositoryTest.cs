using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.JwtHandler;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.DTOs;

namespace ExpenseSharing.Test.RepositoryTesting
{
    public class AccountRepositoryTest
    {
            private readonly DbContextOptions<ExpenseDbContext> _options;
            private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
            private readonly ExpenseDbContext _context;

            public AccountRepositoryTest()
            {
                _options = new DbContextOptionsBuilder<ExpenseDbContext>()
                    .UseInMemoryDatabase(databaseName: "ExpenseTestDb")
                    .Options;

                _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
                _context = new ExpenseDbContext(_options);
            }

         [Fact]
        public async Task Login_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "Test@123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new ApplicationUser { 
                Email = email,
                Name = "Test User",
                CurrentBalance=0,
                Password = hashedPassword, 
                Roles = "User",
                GroupsName="1"
            };
            _context.ApplicationUsers.Add(user);
            _context.SaveChanges();

            _mockJwtTokenGenerator.Setup(j => j.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns("testToken");

            var repository = new AccountRepository(_context, _mockJwtTokenGenerator.Object);

            var loginDTO = new LoginDTO { Email = email, Password = password };

            // Act
            var result = await repository.Login(loginDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(1, result.Id);
            Assert.Equal(user.Name,result.Name);
            Assert.Equal("testToken", result.Token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsInvalidLoginResponse()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "Test@123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new ApplicationUser
            {
                Email = email,
                Name = "Test User",
                CurrentBalance = 0,
                Password = hashedPassword,
                Roles = "User",
                GroupsName = "1"
            };
            _context.ApplicationUsers.Add(user);
            _context.SaveChanges();

            var repository = new AccountRepository(_context, _mockJwtTokenGenerator.Object);

            var loginDTO = new LoginDTO { Email = email, Password = "Test@12345" };

            // Act
            var result = await repository.Login(loginDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(-1, result.Id);
            Assert.Equal("", result.Email);
            Assert.Equal("", result.Token);
        }

        [Fact]
        public void FindByEmail_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "test@gmail.com";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Test@123");
            var user = new ApplicationUser
            {
                Email = email,
                Name = "Test User",
                CurrentBalance = 0,
                Password = hashedPassword,
                Roles = "User",
                GroupsName = "1"
            };
            _context.ApplicationUsers.Add(user);
            _context.SaveChanges();

            var repository = new AccountRepository(_context, _mockJwtTokenGenerator.Object);

            // Act
            var result = repository.FindByEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal(user.Roles, result.Roles);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.CurrentBalance, result.CurrentBalance);

        }

        [Fact]
        public void FindByEmail_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var repository = new AccountRepository(_context, _mockJwtTokenGenerator.Object);

            // Act
            var result = repository.FindByEmail("test@gmail.com");

            // Assert
            Assert.Null(result);
        }
    }
}
