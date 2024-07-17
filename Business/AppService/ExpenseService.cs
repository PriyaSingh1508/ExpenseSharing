using DataAccess.UoW;
using Shared.DTOs;


namespace Business.AppService
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseDTO<IEnumerable<GetExpensesDTO>> GetAllExpensesOfAGroup(int id)
        {
            var expenses = _unitOfWork.Expenses.GetAllExpensesOfAGroup(id);

            var expensesDTO = expenses.Select(expense => new GetExpensesDTO
            {
                Id = expense.Id,
                GroupId = expense.GroupId,
                Description = expense.Description,
                Amount = expense.Amount,
                PaidBy = expense.PaidBy,
                Date = expense.Date,
                SplitAmong = expense.SplitAmong,
                IndividualAmount = expense.IndividualAmount,
                ContributedBy = expense.ContributedBy,
                IsSettled = expense.IsSettled
            });

            //return expensesDTO.ToList();
            return new ResponseDTO<IEnumerable<GetExpensesDTO>> { IsSuccess = true,Data= expensesDTO.ToList(),Message ="All expenses returned successfully!" };
        }

        public async Task<ResponseDTO<string>> CreateExpense(ExpenseDTO expense)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var existingGroup = _unitOfWork.Groups.FindById(expense.GroupId);
                if (existingGroup == null)
                {
                    return new ResponseDTO<string> { IsSuccess=false,Message= "The Group Id does not exist",Data=null};
                }

                var groupMembers = existingGroup.TeamMembers.Split(',').ToList();
                if (!ValidateTeamMember(groupMembers, expense.PaidBy))
                {
                    //return "The paidBy user is not a team member";
                    return new ResponseDTO<string> { IsSuccess=false,Message= "The paidBy user is not a team member", Data=null};
                }

                var splitAmongList = expense.SplitAmong.Split(',').ToList();
                if (!ValidateSplitAmong(groupMembers, splitAmongList))
                {
                    //return "One or more users in splitAmong are not team members";
                    return new ResponseDTO<string> { IsSuccess = false, Message = "One or more users in splitAmong are not team members", Data=null };
                }
                if (splitAmongList.Count == 0)
                {
                    //return "Cannot split among zero users";
                    return new ResponseDTO<string> { IsSuccess = false, Message = "Cannot split among zero users", Data=null };
                }

                var singleUserAmt = expense.Amount / splitAmongList.Count;
                expense.IndividualAmount = singleUserAmt;

                var res = await _unitOfWork.Expenses.CreateExpense(expense);
                if (res)
                {
                    foreach (var item in splitAmongList)
                    {
                        var user = _unitOfWork.Accounts.FindByEmail(item);
                        if (user != null && user.Email != expense.PaidBy)
                        {
                            //user.CurrentBalance += singleUserAmt;
                            user.CurrentBalance -= singleUserAmt;
                            await _unitOfWork.Save();
                        }
                    }
                }
                _unitOfWork.Commit();
                return new ResponseDTO<string> { Data = null, Message = "Expense created successfully", IsSuccess = true };
                //return "Expense created successfully";
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                //return $"An error occurred while creating the expense: {ex.Message}";
                return new ResponseDTO<string> { IsSuccess = false, Message = $"An error occurred while creating the expense: {ex.Message}", Data = null };
            }
        }

        public async Task<ResponseDTO<string>> UpdateExpense(ExpenseDTO expense, int id)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var existingExpense = await _unitOfWork.Expenses.GetExpenseById(id);
                if (existingExpense == null)
                {
                    //return "This expense does not exist";
                    return new ResponseDTO<string> { Data=null,Message= "This expense does not exist", IsSuccess=false };
                }

                if (expense.Amount != existingExpense.Amount)
                {
                    expense.IndividualAmount = expense.Amount / existingExpense.SplitAmong.Split(',').Length;
                    foreach (var item in existingExpense.SplitAmong.Split(',').ToList())
                    {
                        var user = _unitOfWork.Accounts.FindByEmail(item);
                        if (user != null)
                        {
                            user.CurrentBalance += existingExpense.IndividualAmount;
                            user.CurrentBalance -= expense.IndividualAmount;
                            await _unitOfWork.Save();
                        }
                    }
                }

                var res = await _unitOfWork.Expenses.UpdateExpense(expense, id);
                _unitOfWork.Commit();
                //return res;
                if (res== "Updated successfully")
                    return new ResponseDTO<string> { Data = null, Message = res, IsSuccess=true };
                 else
                   return new ResponseDTO<string> { Data=null,Message=res,IsSuccess=false};
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                //return $"An error occurred while updating the expense: {ex.Message}";
                return new ResponseDTO<string> { Data = null, Message = $"An error occurred while updating the expense: {ex.Message}", IsSuccess = false };
            }
        }
        public async Task<ResponseDTO<string>> ExpenseSettlement(int expenseId)
        {
            var existingExpense = await _unitOfWork.Expenses.GetExpenseById(expenseId);
            if (existingExpense == null)
            {
                //return "This expense does not exist";
                return new ResponseDTO<string> { IsSuccess = false,Data=null,Message= "This expense does not exist" };
            }
            if (existingExpense.Amount <= 0)
            {
                existingExpense.IsSettled = true;
                await _unitOfWork.Save();
                //return "Expense Settled";
                return new ResponseDTO<string> { IsSuccess = true, Message = "Expense Settled", Data = null };
            }
            //return "Expense yet to be Settled";
            return new ResponseDTO<string> { IsSuccess = false, Data = null, Message = "Expense yet to be Settled" };
        }
        public async Task<ResponseDTO<string>> AmountContributedByUser(UserAmtSettlementDTO amtSettlementDTO)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var userInfo = _unitOfWork.Accounts.FindByEmail(amtSettlementDTO.Email);
                if (userInfo == null) {
                    //return "No record found corresponding to user"; 
                    return new ResponseDTO<string> { Message= "No record found corresponding to user", IsSuccess=false,Data = null};
                }
                if (userInfo != null)
                {
                    var expInfo = await _unitOfWork.Expenses.GetExpenseById(amtSettlementDTO.ExpenseId); ///paid by user
                    var members = expInfo.SplitAmong.Split(',').ToList();
                    if (!ValidateTeamMember(members, amtSettlementDTO.Email))
                    {
                        //return "The user is not a team member";
                        return new ResponseDTO<string> { IsSuccess = false, Message = "The user is not a team member", Data = null };
                    }
                    if (expInfo != null && amtSettlementDTO.Email != expInfo.PaidBy)
                    {
                        //User call by its email
                        var expenseCreatedbyUserInfo = _unitOfWork.Accounts.FindByEmail(expInfo.PaidBy);
                        expInfo.Amount -= expInfo.IndividualAmount;
                        userInfo.CurrentBalance += expInfo.IndividualAmount;
                        expInfo.ContributedBy = expInfo.ContributedBy + "," + amtSettlementDTO.Email;
                        expenseCreatedbyUserInfo.CurrentBalance += expInfo.IndividualAmount;
                        await _unitOfWork.Save();
                    }
                }
                _unitOfWork.Commit();
                //return "Individual Expense Settled";
                return new ResponseDTO<string> { IsSuccess=true, Message = "Individual Expense Settled", Data=null};
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return new ResponseDTO<string> { IsSuccess = false, Message = $"An error occurred while updating the expense: {ex.Message}", Data = null };
                //return $"An error occurred while updating the expense: {ex.Message}";
            }


        }
        private static bool ValidateTeamMember(List<string> groupMembers, string user)
        {
            return groupMembers.Contains(user);
        }

        private static bool ValidateSplitAmong(List<string> groupMembers, List<string> splitAmong)
        {
            return splitAmong.All(user => groupMembers.Contains(user));
        }
    }
}
