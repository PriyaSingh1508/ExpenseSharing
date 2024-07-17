using Shared.DTOs;

namespace Business.AppService
{
    public interface IExpenseService
    {
        Task<ResponseDTO<string>> AmountContributedByUser(UserAmtSettlementDTO amtSettlementDTO);
        Task<ResponseDTO<string>> CreateExpense(ExpenseDTO expense);
        Task<ResponseDTO<string>> ExpenseSettlement(int expenseId);
        Task<ResponseDTO<string>> UpdateExpense(ExpenseDTO expense, int id);
        ResponseDTO<IEnumerable<GetExpensesDTO>> GetAllExpensesOfAGroup(int id);
    }
}