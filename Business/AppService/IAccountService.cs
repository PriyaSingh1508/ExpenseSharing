using Shared.DTOs;

namespace Business.AppService
{
    public interface IAccountService
    {
        Task<ResponseDTO<LoginResponseDTO>> Login(LoginDTO loginDTO);
        ResponseDTO<UserDTO> FindByEmail(string email);
        ResponseDTO<IEnumerable<GroupInfoDto>> GetAllGroupsByUserEmail(string email);
    }
}