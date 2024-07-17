using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.JwtHandler;
using Shared.DTOs;

namespace DataAccess.Repositories
{
    public class AccountRepository:IAccountRepository
    {
        private readonly ExpenseDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AccountRepository(ExpenseDbContext context, IJwtTokenGenerator jwtTokenGenerator) { 
            _context = context;
            _jwtTokenGenerator=jwtTokenGenerator;
        }
        public ApplicationUser FindByEmail(string email)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            return user;
        }
        public async Task<LoginResponseDTO> Login(LoginDTO loginDTO)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == loginDTO.Email.ToLower()); //User Information

            bool isValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password,user.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDTO() { Id = -1,Email="", Token = "" };
            }
            //if user was found , Generate JWT Token
            var roles = user.Roles.ToString();
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Token = token,
                Role=user.Roles
            };

            return loginResponseDTO;
        }
        public string PasswordGenerator(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return passwordHash;
        }

    }
}
