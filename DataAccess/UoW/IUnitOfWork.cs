using DataAccess.Repositories;

namespace DataAccess.UoW
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        IGroupRepository Groups { get; }
        IExpenseRepository Expenses { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
        Task Save();
    }
}
