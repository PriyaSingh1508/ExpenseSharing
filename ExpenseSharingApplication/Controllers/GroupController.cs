using Business.AppService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace ExpenseSharingApplication.Controllers
{
    [Route("group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService=groupService;
        }

        [HttpPost("add-group")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateGroup(GroupDTO group)
        {
            var res = await _groupService.CreateGroup(group);
            return Ok(res);
        }

        [HttpDelete("delete-group/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
           var res= await _groupService.DeleteGroup(id);
            return Ok(res);
        }
    }
}
