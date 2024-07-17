using DataAccess.Entities;
using Shared.DTOs;

namespace DataAccess.Repositories
{
    public interface IAccountRepository
    {
        Task<LoginResponseDTO> Login(LoginDTO loginDTO);
        ApplicationUser FindByEmail(string email);
        string PasswordGenerator(string password);
    }
}
