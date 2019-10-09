using System;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Logman.Business.Caching;
using Logman.Common.Contracts;
using Logman.Common.Data;
using Logman.Common.DomainObjects;
using Microsoft.Practices.Unity;

namespace Logman.Business.Events
{
    public class EventBusiness : IEventBusiness
    {
        [Dependency]
        public IDataAccessLayer DataAccessLayer { get; set; }

        [Dependency]
        public ICacheProvider CacheProvider { get; set; }

        [Dependency]
        public IApplicationBusiness ApplicationBusiness { get; set; }

        public async Task<long> RegisterEventAsync(Event log)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                if (HttpContext.Current != null)
                {
                    Util.PopulateClientInfo(log, HttpContext.Current.Request);
                }

                IRepository<Event> repos = DataAccessLayer.GetEventRepository(uow);
                long newId = await repos.Add(log);
                return newId;
            }
        }

        public async Task<Event> GetByIdAsync(long id, string appKey)
        {
            var app = ApplicationBusiness.GetByAppKeyAsync(appKey);
            if (app == null)
            {
                throw new ArgumentException("The given application key is invalid.");
            }

            string cacheKey = Util.GetEventCacheKey(id);
            var foundEvent = CacheProvider.Get<Event>(cacheKey);

            if (foundEvent == null)
            {
                using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
                {
                    IRepository<Event> repos = DataAccessLayer.GetEventRepository(uow);
                    foundEvent = await repos.GetByIdAsync(id);
                    if (foundEvent.ApplicationId != app.Id)
                    {
                        throw new ArgumentException("The given event ID and application key do not match!");
                    }

                    if (foundEvent != null)
                    {
                        await GetChildrenAsync(foundEvent);
                        CacheProvider.Add(cacheKey, foundEvent, DateTimeOffset.Now.AddHours(Util.GetLogLifespanHours()));
                    }
                }
            }

            return foundEvent;
        }


        public async Task<Event> GetChildAsync(long id)
        {
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IEventRepository repos = DataAccessLayer.GetEventRepository(uow);
                return await repos.GetChildAsync(id);
            }
        }

        public async Task<int> AddAlertAsync(Alert alert)
        {
            if (alert == null)
            {
                return -1;
            }
            using (IUnitOfWork uow = DataAccessLayer.GetUnitOfWork())
            {
                IEventRepository repos = DataAccessLayer.GetEventRepository(uow);
                return await repos.AddAlert(alert);
            }
        }

        private async Task GetChildrenAsync(Event model)
        {
            if (model == null)
            {
                return;
            }

            while (true)
            {
                Event childEvent = await GetChildAsync(model.Id);

                if (childEvent == null)
                {
                    return;
                }
                var childResponse = Mapper.Map<Event>(childEvent);
                model.InnerEvent = childResponse;
                model = childResponse;
            }
        }
    }
}