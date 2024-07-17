using DataAccess.Entities;
using Shared.DTOs;

namespace DataAccess.Repositories
{
    public interface IGroupRepository
    {
        Task<string> CreateGroup(GroupDTO group);
        bool DeleteGroup(int id);
        Group FindById(int Id);
    }
}
