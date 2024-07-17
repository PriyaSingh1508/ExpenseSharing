using DataAccess.Data;
using DataAccess.Entities;
using Shared.DTOs;


namespace DataAccess.Repositories
{
    public class ExpenseRepository:IExpenseRepository
    {
        private readonly ExpenseDbContext _context;

        public ExpenseRepository(ExpenseDbContext context) {
        _context=context;
        }

        public async Task<bool> CreateExpense(ExpenseDTO expenseDTO)
        {
            try
            {
                Expense createExpense = new Expense
                {
                    GroupId = expenseDTO.GroupId,
                    Description = expenseDTO.Description,
                    Amount = expenseDTO.Amount-expenseDTO.IndividualAmount,
                    PaidBy = expenseDTO.PaidBy,
                    Date = expenseDTO.Date,
                    SplitAmong = expenseDTO.SplitAmong,
                    IndividualAmount = expenseDTO.IndividualAmount,
                    ContributedBy = expenseDTO.PaidBy
                };

                await _context.Expenses.AddAsync(createExpense);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error creating expense: {e}");
                return false;
            }
        }

        public async Task<string> UpdateExpense(ExpenseDTO expense, int id)
        {
            var existingExpense = _context.Expenses.FirstOrDefault(x => x.Id == expense.ExpenseId);
            
            if(existingExpense!= null)
            {
                existingExpense.GroupId = expense.GroupId;
                existingExpense.Description = expense.Description;
                existingExpense.Date = expense.Date;
                existingExpense.IndividualAmount = expense.IndividualAmount;
                _context.Expenses.Update(existingExpense);
                _context.SaveChanges();
                return "Updated successfully";
                
            }
            return "Update failed";

        }

        public bool DeleteExpense(int id)
        {
            var existingExpense = _context.Expenses.FirstOrDefault(e=>e.Id == id);
            if(existingExpense!= null)
            {
                _context.Expenses.Remove(existingExpense);
                return true;
            }
            return false;
        }
        public IEnumerable<Expense> GetAllExpensesOfAGroup(int groupId)
        {
            IEnumerable<Expense> expenses = _context.Expenses.Where(x => x.GroupId == groupId);
            return expenses;
        }
        public async Task<Expense> GetExpenseById(int id)
        {
            return _context.Expenses.FirstOrDefault(x => x.Id == id);
        
        }
    }
}
