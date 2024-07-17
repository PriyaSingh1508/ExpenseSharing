using DataAccess.Entities;
using Shared.DTOs;

namespace DataAccess.Repositories
{
    public interface IExpenseRepository
    {
        Task<bool> CreateExpense(ExpenseDTO expenseDTO);
        Task<Expense> GetExpenseById(int id);
        Task<string> UpdateExpense(ExpenseDTO expense,int id);
        bool DeleteExpense(int id);
        IEnumerable<Expense> GetAllExpensesOfAGroup(int groupId);
    }
}
