using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logman.Common.DomainObjects;

namespace Logman.Common.Contracts
{
    public interface IApplicationBusiness
    {
        Task<ApplicationStatus> GetAppStatusAsync(long id);
        Task<ApplicationTrends> GetApplicationTrendAsync(long id);
        Task<Application> GetByAppKeyAsync(string appKey);
        Task<Application> GetByIdAsync(long id);
        Task<Application> CreateApplicationAsync(Application record);
        Task<Application> UpdateApplicationAsync(Application record);
        Task<ApplicationEvents> GetApplicationEventsAsync(long appId, int? pageNumber = null, int? pageSize = null, string keywords = null, DateTime? fromDate = null, DateTime? toDate = null, EventLevel? eventLevel = null);
        Task<List<Alert>> GetAlertsOfApp(long appId);
        Task<List<Application>> FindAllAsync();
        Task UpdateAlertExecTimes(Dictionary<long, DateTime> listOfAlerts);
        Task<List<Application>> GetAppsOfUserAsync(long userId);
        Task<List<User>> GetApplicationUsersAsync(long appId);
    }
}