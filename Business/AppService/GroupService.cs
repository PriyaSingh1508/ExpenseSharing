using DataAccess.UoW;
using Shared.DTOs;

namespace Business.AppService
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO<string>> CreateGroup(GroupDTO group)
        {
            IEnumerable<string> members = group.TeamMembers.Split(',');
            List<string> validMembers = new();

            foreach (var member in members)
            {
                var IsUser = _unitOfWork.Accounts.FindByEmail(member);
                if (IsUser == null)
                {
                    //return "Only registered users can be added to the team";
                    return new ResponseDTO<string> { IsSuccess = false,Message= "Only registered users can be added to the team", Data=null };
                }

                validMembers.Add(member);
                group.TotalMembers++;
            }

            string teamMembers = string.Join(",", validMembers);
            group.TeamMembers = teamMembers;
            var res = await _unitOfWork.Groups.CreateGroup(group);
            foreach (var item in group.TeamMembers.Split(','))
            {
                var user = _unitOfWork.Accounts.FindByEmail(item);
                user.GroupsName = res+","+user.GroupsName;
                //var groups = user.GroupsName.Split(",");
                //var newGroupsList = groups.ToList();

                //if (!newGroupsList.Contains(res))
                //{
                //    newGroupsList.Add(res);
                //}
                //user.GroupsName = String.Join(",", newGroupsList);

            }
            await _unitOfWork.Save();
            //return res;
            return new ResponseDTO<string> { IsSuccess=true,Message=res,Data=null };
        }

        public async Task<ResponseDTO<bool>> DeleteGroup(int id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var existingGroup = _unitOfWork.Groups.FindById(id);
                if (existingGroup == null) { /*return false; */
                    return new ResponseDTO<bool> { IsSuccess = false, Message = "No such group exists", Data = false };
                }
                if (existingGroup != null)
                {
                    var expenses = _unitOfWork.Expenses.GetAllExpensesOfAGroup(id).ToList();
                    foreach (var expense in expenses)
                    {
                        if (!expense.IsSettled)
                        {
                            var splitList = expense.SplitAmong.Split(",").ToList();

                            var teamMembersContributed = expense.ContributedBy.Split(",").ToList();
                            List<string> newTeamMembersNotContributed = new List<string>();
                            foreach (var teamMember in splitList)
                            {
                                if (!teamMembersContributed.Contains(teamMember)) newTeamMembersNotContributed.Add(teamMember);
                            }
                            foreach (var member in newTeamMembersNotContributed)
                            {
                                var user = _unitOfWork.Accounts.FindByEmail(member);
                                if (user != null)
                                {
                                    user.CurrentBalance += expense.IndividualAmount;
                                }

                            }
                        }
                        _unitOfWork.Expenses.DeleteExpense(expense.Id);
                       
                    }
                    var userEmails = existingGroup.TeamMembers.Split(",").Where(member => !string.IsNullOrWhiteSpace(member)).ToList();
                    foreach (var email in userEmails)
                    {
                       var user= _unitOfWork.Accounts.FindByEmail(email);
                        var groupIds=user.GroupsName.Split(",").Where(id => !string.IsNullOrWhiteSpace(id)).ToList();
                        groupIds.Remove(existingGroup.Id.ToString());
                        var updatedGroupIds = "";
                        foreach (var grpId in groupIds)
                        {   if (updatedGroupIds != "")
                                updatedGroupIds = grpId + "," + updatedGroupIds;
                            else updatedGroupIds = grpId;
                        }
                        user.GroupsName=updatedGroupIds;
                    }
                   _unitOfWork.Groups.DeleteGroup(id);
                }
                await _unitOfWork.Save();
                _unitOfWork.Commit();
                //return true;
                return new ResponseDTO<bool> { IsSuccess = true, Message = "Deleted successfully", Data = true };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                //return false;
                return new ResponseDTO<bool> { IsSuccess = false, Message = "Delete failed", Data = false };
            }
        }

    }
}
