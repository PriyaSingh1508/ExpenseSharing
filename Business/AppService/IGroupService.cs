using Shared.DTOs;

namespace Business.AppService
{
    public interface IGroupService
    {
        Task<ResponseDTO<string>> CreateGroup(GroupDTO group);
        Task<ResponseDTO<bool>> DeleteGroup(int id);
    }
}