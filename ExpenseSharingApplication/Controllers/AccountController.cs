using Business.AppService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace ExpenseSharingApplication.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService=accountService;
        }
        [HttpPost("Login")]
        public async Task<ResponseDTO<LoginResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            return await _accountService.Login(loginDTO);
        }
        [HttpGet("find-by-email/{email}")]
        public IActionResult FindByEmail(string email)
        {
            return Ok(_accountService.FindByEmail(email));
        }

        [Authorize]
        [HttpGet("get-all-group-by-user-email/{email}")]
        public IActionResult GetAllGroupsByUserEmail(string email)
        {
            return Ok(_accountService.GetAllGroupsByUserEmail(email));
        }
    }
}
