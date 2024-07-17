using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;


namespace ExpenseSharing.Test.RepositoryTesting
{
    public class GroupRepositoryTest
    {
        private readonly DbContextOptions<ExpenseDbContext> _options;

        public GroupRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<ExpenseDbContext>()
                .UseInMemoryDatabase(databaseName: "ExpenseTestDb") 
                .Options;
        }

        [Fact]
        public async Task CreateGroup_ShouldCreateAGroup()
        {
            // Arrange
            using var context = new ExpenseDbContext(_options);
            var repository = new GroupRepository(context);

            var groupDto = new GroupDTO
            {
                GroupName = "DPCODE",
                GroupDescription = "edu-hub",
                TeamMembers = "dpcode1@gmail.com, dpcode2@gmail.com",
                TotalMembers = 2
            };

            // Act
            var result = await repository.CreateGroup(groupDto);

            // Assert
            Assert.NotNull(result);
            var createdGroup =  repository.FindById(int.Parse(result));
            Assert.NotNull(createdGroup);
            Assert.Equal("DPCODE", createdGroup.GroupName);
            Assert.Equal("edu-hub", createdGroup.GroupDescription);
            Assert.Equal("dpcode1@gmail.com, dpcode2@gmail.com", createdGroup.TeamMembers);
            Assert.Equal(2, createdGroup.TotalMembers);
        }

        [Fact]
        public async Task Can_FetchGroupById()
        {
            // To
            using var context = new ExpenseDbContext(_options);
            var repository = new GroupRepository(context);
            var groupDto = new GroupDTO
            {
                GroupName = "MYDPCODE",
                GroupDescription = "g&m",
                TeamMembers = "dpcode1@gmail.com, dpcode2@gmail.com, dpcode@gmail.com",
                TotalMembers = 3
            };
            var result = await repository.CreateGroup(groupDto);
            Assert.NotNull(result);
            //Do
            var createdGroup = repository.FindById(int.Parse(result));
            //Done
            Assert.NotNull(createdGroup);
            Assert.Equal("MYDPCODE", createdGroup.GroupName);
            Assert.Equal("g&m", createdGroup.GroupDescription);
            Assert.Equal("dpcode1@gmail.com, dpcode2@gmail.com, dpcode@gmail.com", createdGroup.TeamMembers);
            Assert.Equal(3, createdGroup.TotalMembers);
        }

        [Fact]
        public async Task DeleteGroup_GroupExists_ReturnsTrue()
        {
            // Arrange
            using var context = new ExpenseDbContext(_options);
            var repository = new GroupRepository(context);
            var groupDto = new GroupDTO
            {
                GroupName = "MYDPCODE",
                GroupDescription = "g&m",
                TeamMembers = "dpcode1@gmail.com, dpcode2@gmail.com, dpcode@gmail.com",
                TotalMembers = 3
            };
            var result = await repository.CreateGroup(groupDto);
            Assert.NotNull(result);
             // Act
            var response = repository.DeleteGroup(1);

            // Assert
            Assert.True(response);
        }

        [Fact]
        public void DeleteGroup_GroupDoesNotExist_ReturnsFalse()
        {
            using var context = new ExpenseDbContext(_options);
            // Arrange
            var repository = new GroupRepository(context);

            // Act
            var response = repository.DeleteGroup(12);

            // Assert
            Assert.False(response);
        }
    }
}
