using System;
using System.Data;
using AutoMapper;
using Logman.Common.Data;
using Logman.Common.DomainObjects;
using Logman.Data.SqlServer.Base;

namespace Logman.Data.SqlServer.Facade
{
    public class SqlDataAccessLayer : IDataAccessLayer
    {
        public void Initialize()
        {
            Mapper.CreateMap<IDataReader, Event>()
                .ForMember(m => m.Id, opt => opt.MapFrom(r => r.GetInt64(r.GetOrdinal("Id"))))
                .ForMember(m => m.ProviderName, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("ProviderName"))))
                .ForMember(m => m.EventLevel,
                    opt => opt.MapFrom(r => (EventLevel) r.GetInt16(r.GetOrdinal("EventLevel"))))
                .ForMember(m => m.Keywords, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("Keywords"))))
                .ForMember(m => m.TimeCreated, opt => opt.MapFrom(r => r.GetDateTime(r.GetOrdinal("TimeCreated"))))
                .ForMember(m => m.ParentId,
                    opt =>
                        opt.MapFrom(
                            r => !r.IsDBNull(r.GetOrdinal("ParentId")) ? r.GetInt64(r.GetOrdinal("ParentId")) : 0))
                .ForMember(m => m.ComputerName, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("ComputerName"))))
                .ForMember(m => m.IpAddress, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("IpAddress"))))
                .ForMember(m => m.UserAgent, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("UserAgent"))))
                .ForMember(m => m.Message, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("Message"))))
                .ForMember(m => m.Description, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("Description"))))
                .ForMember(m => m.ExtendedInformation,
                    opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("ExtendedInformation"))))
                .ForMember(m => m.ApplicationId, opt => opt.MapFrom(r => r.GetInt64(r.GetOrdinal("ApplicationId"))));

            Mapper.CreateMap<IDataReader, Application>()
                .ForMember(m => m.Id, opt => opt.MapFrom(r => (long) r.GetValue(r.GetOrdinal("Id"))))
                .ForMember(m => m.AppName, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("AppName"))))
                .ForMember(m => m.AppKey, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("AppKey"))))
                .ForMember(m => m.Enabled, opt => opt.MapFrom(r => r.GetBoolean(r.GetOrdinal("Enabled"))))
                .ForMember(m => m.DefaultRetainPeriodDays,
                    opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("DefaultRetainPeriodDays"))))
                .ForMember(m => m.MaxFatalErrors, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("MaxFatalErrors"))))
                .ForMember(m => m.MaxErrors, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("MaxErrors"))))
                .ForMember(m => m.MaxWarnings, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("MaxWarnings"))));

            Mapper.CreateMap<IDataReader, ApplicationStatus>()
                .ForMember(m => m.DefaultRetainPeriodDays,
                    opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("DefaultRetainPeriodDays"))))
                .ForMember(m => m.FatalCount, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("FatalCount"))))
                .ForMember(m => m.ErrorCount, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("ErrorCount"))))
                .ForMember(m => m.WarningCount, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("WarningCount"))))
                .ForMember(m => m.MaxFatalErrors, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("MaxFatalErrors"))))
                .ForMember(m => m.MaxErrors, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("MaxErrors"))))
                .ForMember(m => m.MaxWarnings, opt => opt.MapFrom(r => r.GetInt32(r.GetOrdinal("MaxWarnings"))));

            Mapper.CreateMap<IDataReader, User>()
                .ForMember(m => m.Id, opt => opt.MapFrom(r => r.GetInt64(r.GetOrdinal("ID"))))
                .ForMember(m => m.Username, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("USERNAME"))))
                .ForMember(m => m.Password, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("PASSWORD"))))
                .ForMember(m => m.PasswordSalt, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("PASSWORDSALT"))))
                .ForMember(m => m.Enabled, opt => opt.MapFrom(r => r.GetBoolean(r.GetOrdinal("ACTIVE"))))
                .ForMember(m => m.ActivationKey, opt => opt.MapFrom(r => r.GetString(r.GetOrdinal("ACTIVATIONKEY"))));

            Mapper.CreateMap<IDataReader, Alert>()
                .ForMember(m => m.EventLevelValue,
                    opt => opt.MapFrom(reader => reader.GetInt32(reader.GetOrdinal("EventLevelValue"))))
                .ForMember(m => m.PeriodValue,
                    opt => opt.MapFrom(reader => reader.GetInt32(reader.GetOrdinal("PeriodValue"))))
                .ForMember(m => m.TypeOfNotification,
                    opt =>
                        opt.MapFrom(reader => (NotificationType) reader.GetInt16(reader.GetOrdinal("NotificationType"))))
                .ForMember(m => m.Target, opt => opt.MapFrom(reader => reader.GetString(reader.GetOrdinal("Target"))))
                .ForMember(m => m.Id, opt => opt.MapFrom(reader => reader.GetInt32(reader.GetOrdinal("Id"))))
                .ForMember(m => m.Value, opt => opt.MapFrom(reader => reader.GetInt32(reader.GetOrdinal("Value"))))
                .ForMember(m => m.LastExecutionTime,
                    opt =>
                        opt.MapFrom(
                            reader =>
                                reader.IsDBNull(reader.GetOrdinal("LastExecutionTime"))
                                    ? DateTime.MinValue
                                    : reader.GetDateTime(reader.GetOrdinal("LastExecutionTime"))
                            ));
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return new SqlUnitOfWork();
        }

        public IEventRepository GetEventRepository(IUnitOfWork unitOfWork)
        {
            var repository = new EventRepository {UnitOfWork = unitOfWork};
            return repository;
        }

        public IApplicationRepository GetApplicationRepository(IUnitOfWork unitOfWork)
        {
            var repository = new ApplicationRepository {UnitOfWork = unitOfWork};
            return repository;
        }

        public IAccountRepository GetAccountRepository(IUnitOfWork unitOfWork)
        {
            var repository = new AccountRepository {UnitOfWork = unitOfWork};
            return repository;
        }
    }
}