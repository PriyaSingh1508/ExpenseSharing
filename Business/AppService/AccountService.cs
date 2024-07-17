using DataAccess.UoW;
using Shared.DTOs;

namespace Business.AppService
{
    public class AccountService : IAccountService
    {

        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<ResponseDTO<LoginResponseDTO>> Login(LoginDTO loginDTO)
        {
            //return await _unitOfWork.Accounts.Login(loginDTO);
            var res = await _unitOfWork.Accounts.Login(loginDTO);
            return new ResponseDTO<LoginResponseDTO> { IsSuccess = true,Data=res,Message="" };
        }
        public ResponseDTO<UserDTO> FindByEmail(string email)
        {
            var user = _unitOfWork.Accounts.FindByEmail(email);
            if (user == null)
            {
                return new ResponseDTO<UserDTO> { IsSuccess = false, Message = "User not found", Data = null, };
            }
            var userInfo = new UserDTO { 
                Email = user.Email,
                Name= user.Name,
               Role = user.Roles,
               Id= user.Id
            };
           // return userInfo;
           return new ResponseDTO<UserDTO> {IsSuccess=true,Message="User Information", Data = userInfo,  };
        }

        public ResponseDTO<IEnumerable<GroupInfoDto>> GetAllGroupsByUserEmail(string email)
        {
            var user = _unitOfWork.Accounts.FindByEmail(email);
            if (user == null)
            {
                return new ResponseDTO<IEnumerable<GroupInfoDto>> { IsSuccess = false, Message = "User does not exist", Data = null, };
            };
            var groupId = user.GroupsName.Split(',')
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => int.Parse(id))
                .ToList();

            List<GroupInfoDto> groupInfoDtos = new List<GroupInfoDto>();

            foreach (var id in groupId)
            {
                var grp = _unitOfWork.Groups.FindById(id);
                GroupInfoDto groupInfo = new GroupInfoDto
                {
                    Id = grp.Id,
                    GroupName = grp.GroupName,
                    GroupDescription = grp.GroupDescription,
                    GroupCreatedDate = grp.GroupCreatedDate,
                    TeamMembers = grp.TeamMembers,
                    TotalMembers = grp.TotalMembers,
                };
                groupInfoDtos.Add(groupInfo);
            }

            //return groupInfoDtos;
            return new ResponseDTO<IEnumerable<GroupInfoDto>> { IsSuccess=true,Message="",Data= groupInfoDtos, };
        }

    }
}
