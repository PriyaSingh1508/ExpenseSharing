using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Repositories;
using DataAccess.JwtHandler;

namespace DataAccess.UoW
{
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        public ExpenseDbContext Context { get; private set; }
        private IDbContextTransaction? _objTran = null;
        public IAccountRepository Accounts{ get; private set; }
        public IJwtTokenGenerator TokenGenerator { get; private set; }
        public IExpenseRepository Expenses { get; private set; }
        public IGroupRepository Groups { get; private set; }
        public UnitOfWork(ExpenseDbContext context,IJwtTokenGenerator tokenGenerator)
        {
            Context = context;
            Accounts = new AccountRepository(context,tokenGenerator);
            Groups = new GroupRepository(context);
            Expenses = new ExpenseRepository(context);
        }
        public void BeginTransaction()
        {
            _objTran = Context.Database.BeginTransaction();
        }
        public void Commit()
        {
            _objTran?.Commit();
        }
        public void Rollback()
        {
            _objTran?.Rollback();
            _objTran?.Dispose();
        }
        public async Task Save()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
