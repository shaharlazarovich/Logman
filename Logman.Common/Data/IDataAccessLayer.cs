namespace Logman.Common.Data
{
    public interface IDataAccessLayer
    {
        void Initialize();
        IUnitOfWork GetUnitOfWork();
        IEventRepository GetEventRepository(IUnitOfWork unitOfWork);
        IApplicationRepository GetApplicationRepository(IUnitOfWork unitOfWork);
        IAccountRepository GetAccountRepository(IUnitOfWork unitOfWork);
    }
}