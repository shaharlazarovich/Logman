using System;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Logman.Business.Applications;
using Logman.Business.Events;
using Logman.Common.Code;
using Logman.Common.Contracts;
using Logman.Common.DomainObjects;
using Logman.Common.Logging;
using Logman.Web.Code.Classes;
using Logman.Web.Code.Models;
using Microsoft.Practices.Unity;

namespace Logman.Web.Controllers
{
    [RoutePrefix(Constants.RouteEventVersion1)]
    public class EventController : ApiController
    {
        public EventController()
        {
            Mapper.CreateMap<EventModel, Event>();
            Mapper.CreateMap<Event, EventResponseModel>();
        }


        [Dependency]
        public IEventBusiness EventBusiness { get; set; }


        [Dependency]
        public IApplicationBusiness ApplicationBusiness { get; set; }

        [Dependency]
        public ILogger LogBuddy { get; set; }

        [Route("")]
        public async Task<IHttpActionResult> Post(EventModel log)
        {
            if (log == null)
            {
                return BadRequest(Constants.HttpMessages.NoEntryWasPassed);
            }

            var response = new ApiResponse<long>();
            try
            {
                var eventDto = Mapper.Map<Event>(log);

                Application app = await ApplicationBusiness.GetByAppKeyAsync(log.AppKey);
                eventDto.ApplicationId = app.Id;

                long registerEventResult = await EventBusiness.RegisterEventAsync(eventDto);
                eventDto.Id = registerEventResult;

                await RegisterChildEvents(log, eventDto.Id, eventDto.ApplicationId);
                response.Message = string.Empty;
                response.Payload = registerEventResult;

                string uri = Url.Link("GetEventRoute", new {id = registerEventResult, appKey = log.AppKey});
                return Created(uri, response);
            }
            catch (Exception ex)
            {
                LogBuddy.LogError(ex);
                return InternalServerError(ex);
            }
        }

        private async Task RegisterChildEvents(EventModel log, long id, long applicationId)
        {
            while (log.InnerEvent != null)
            {
                var eventDto = new Event();
                Mapper.Map(log.InnerEvent, eventDto);

                eventDto.ApplicationId = applicationId;
                eventDto.ParentId = id;
                long newId = await EventBusiness.RegisterEventAsync(eventDto);

                log = log.InnerEvent;
                await RegisterChildEvents(log, newId, applicationId);
            }
        }

        [Route("{id}/{appKey}", Name = "GetEventRoute")]
        public async Task<IHttpActionResult> Get(long id, string appKey)
        {
            if (id == 0 || string.IsNullOrEmpty(appKey))
            {
                return BadRequest(Constants.HttpMessages.NoEntryWasPassed);
            }

            try
            {
                Event eventEntry = await EventBusiness.GetByIdAsync(id, appKey);

                var eventResponse = Mapper.Map<EventResponseModel>(eventEntry);
                var response = new ApiResponse<EventResponseModel>
                {
                    Message = string.Empty,
                    Payload = eventResponse,
                    StatusCode = Constants.ApiResponseCodes.OK
                };
                return Ok(response);
            }
            catch (ArgumentException ax)
            {
                LogBuddy.LogError(ax);
                return NotFound();
            }
            catch (Exception ex)
            {
                LogBuddy.LogError(ex);
                return InternalServerError(ex);
            }
        }
    }
}