using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Logman.Common.Contracts;
using Logman.Common.Data;
using Logman.Common.DomainObjects;
using Microsoft.Practices.Unity;

namespace Logman.Business.Applications
{
    public class ApplicationBusiness : IApplicationBusiness
    {
        [Dependency]
        public IDataAccessLayer DataAccessLayer { get; set; }

        [Dependency]
        public IAccountBusiness AccountBusiness { get; set; }

        public async Task<ApplicationStatus> GetAppStatusAsync(long id)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetAppStatusAsync(id);
            }
        }

        public async Task<ApplicationTrends> GetApplicationTrendAsync(long id)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetApplicationTrendAsync(id);
            }
        }

        public async Task<Application> GetByAppKeyAsync(string appKey)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetByIdAsync(appKey);
            }
        }

        public async Task<Application> GetByIdAsync(long id)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetByIdAsync(id);
            }
        }

        public async Task<Application> CreateApplicationAsync(Application record)
        {
            var bytes = new byte[16];
            new RNGCryptoServiceProvider().GetNonZeroBytes(bytes);
            record.AppKey = string.Join("", bytes.Select(c => c.ToString("x2")));
            var user = AccountBusiness.GetCurrentUser();
            if (user != null)
            {
                using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
                {
                    IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                    var newAppId = await repos.Add(record);
                    await repos.AddAppUser(newAppId, user.Id, Roles.Admin);
                    record.Id = newAppId;
                }
            }

            return record;
        }

        public async Task<Application> UpdateApplicationAsync(Application record)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.UpdateApplicationAsync(record);
            }
        }

        public async Task<ApplicationEvents> GetApplicationEventsAsync(long appId, int? pageNumber = null, int? pageSize = null, string keywords = null, DateTime? fromDate = null, DateTime? toDate = null, EventLevel? eventLevel = null)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetApplicationEventsAsync(appId, pageNumber, pageSize, keywords, fromDate, toDate, eventLevel);
            }
        }

        public async Task<List<Alert>> GetAlertsOfApp(long appId)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetAlertsOfApp(appId);
            }
        }

        public async Task<List<Application>> FindAllAsync()
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.FindAllAsync();
            }
        }

        public async Task UpdateAlertExecTimes(Dictionary<long, DateTime> listOfAlerts)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                await repos.UpdateAlertExecTimes(listOfAlerts);
            }
        }

        public async Task<List<Application>> GetAppsOfUserAsync(long userId)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IApplicationRepository repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetAppsOfUserAsync(userId);
            }
        }

        public async Task<List<User>> GetApplicationUsersAsync(long appId)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                var repos = DataAccessLayer.GetApplicationRepository(uow);
                return await repos.GetApplicationUsersAsync(appId);
            }
        }
    }
}