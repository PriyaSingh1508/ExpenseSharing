using Business.AppService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace ExpenseSharingApplication.Controllers
{
    [Route("expense")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService) { 
        _expenseService = expenseService;
        }
        [Authorize(Roles = "User")]
        [HttpGet("get-all-expenses-of-a-group/{id}")]
        public IActionResult GetAllExpensesOfAGroup(int id)
        {
            return Ok(_expenseService.GetAllExpensesOfAGroup(id));   
        }

        [Authorize(Roles = "User")]
        [HttpPost("create-expense")]
        public async Task<IActionResult> CreateExpense(ExpenseDTO expense)
        {
            var res = await _expenseService.CreateExpense(expense);

            return Ok(res);
           
        }


        [HttpPut("update-expense/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateExpense(ExpenseDTO expenseDTO,int id)
        {
          var res = await _expenseService.UpdateExpense(expenseDTO,id);
            return Ok(res);
        }

        [HttpPost("amount-contributed-by-user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AmountContributedByUser(UserAmtSettlementDTO amtSettlementDTO)
        {
            var res = await _expenseService.AmountContributedByUser(amtSettlementDTO);
            return Ok(res); ;
        }

        [HttpGet("expense-settlement/{expenseId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ExpenseSettlement(int expenseId)
        {
            var res = await _expenseService.ExpenseSettlement(expenseId);
            return Ok(res);
        }
    }
}
