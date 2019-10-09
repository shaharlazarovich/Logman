using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logman.Common.DomainObjects;

namespace Logman.Common.Data
{
    public interface IApplicationRepository : IRepository<Application>
    {
        Task<ApplicationTrends> GetApplicationTrendAsync(long id);
        Task<ApplicationStatus> GetAppStatusAsync(long id);
        Task<Application> UpdateApplicationAsync(Application record);
        Task AddAppUser(long appId, long userId, Roles role);
        Task<ApplicationEvents> GetApplicationEventsAsync(long appId, int? pageNumber = null, int? pageSize = null, string keywords = null, DateTime? fromDate = null, DateTime? toDate = null, EventLevel? eventLevel = null);
        Task<List<Alert>> GetAlertsOfApp(long appId);
        Task UpdateAlertExecTimes(Dictionary<long, DateTime> listOfAlerts);
        Task<List<Application>> GetAppsOfUserAsync(long userId);
        Task<List<User>> GetApplicationUsersAsync(long appId);
    }
}