using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using Logman.Common.Data;
using Logman.Common.DomainObjects;

namespace Logman.Data.SqlServer.Base
{
    public class EventRepository : IEventRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public async Task<long> Add(Event record)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            const string registerEventStoredProcName = "RegisterEvent";

            DbConnection connection = UnitOfWork.Connection;
            await connection.OpenAsync();
            var command = connection.CreateCommand() as SqlCommand;


            if (command != null)
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = registerEventStoredProcName;

                command.Parameters.AddWithValue("@ParentId", record.ParentId);
                command.Parameters.AddWithValue("@Providername", record.ProviderName);
                command.Parameters.AddWithValue("@EventLevel", (short) record.EventLevel);
                command.Parameters.AddWithValue("@Keywords", record.Keywords);
                command.Parameters.AddWithValue("@ComputerName", record.ComputerName);
                command.Parameters.AddWithValue("@IpAddress", record.IpAddress);
                command.Parameters.AddWithValue("@UserAgent", record.UserAgent);
                command.Parameters.AddWithValue("@Message", record.Message);
                command.Parameters.AddWithValue("@Description", record.Description);
                command.Parameters.AddWithValue("@ExtendedInformation", record.ExtendedInformation);
                command.Parameters.AddWithValue("@ApplicationId", record.ApplicationId);

                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return (long) reader.GetDecimal(0);
                }
            }
            return 1;
        }

        public async Task<Event> GetByIdAsync(long id)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            const string getByIdStoredProcName = "GetEventById";

            var result = new Event();
            DbConnection connection = UnitOfWork.Connection;
            await connection.OpenAsync();
            var command = connection.CreateCommand() as SqlCommand;
            if (command != null)
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = getByIdStoredProcName;
                command.Parameters.AddWithValue("@Id", id);
                DbDataReader reader = await command.ExecuteReaderAsync();
                reader.Read();
                return Mapper.Map(reader, result);
            }
            return null;
        }

        public Task<Event> GetByIdAsync(string id)
        {
            return null;
        }

        public Task<List<Event>> FindAllAsync()
        {
            return null;
        }

        public async Task<Event> GetChildAsync(long id)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            const string getByIdStoredProcName = "GetChildEvents";

            var result = new Event();
            DbConnection connection = UnitOfWork.Connection;
            await connection.OpenAsync();
            var command = connection.CreateCommand() as SqlCommand;
            if (command != null)
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = getByIdStoredProcName;
                command.Parameters.AddWithValue("@Id", id);
                DbDataReader reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                return reader.HasRows ? Mapper.Map(reader, result) : null;
            }
            return null;
        }

        public async Task<int> AddAlert(Alert alert)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            const string spName = "AddAlert";

            DbConnection connection = UnitOfWork.Connection;
            await connection.OpenAsync();
            var command = connection.CreateCommand() as SqlCommand;
            if (command != null)
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;
                command.Parameters.AddWithValue("@EventLevelValue", alert.EventLevelValue);
                command.Parameters.AddWithValue("@PeriodValue", alert.PeriodValue);
                command.Parameters.AddWithValue("@PeriodType", alert.TypeOfPeriod);
                command.Parameters.AddWithValue("@Value", alert.Value);
                command.Parameters.AddWithValue("@NotificationType", alert.TypeOfNotification);
                command.Parameters.AddWithValue("@Target", alert.Target);
                command.Parameters.AddWithValue("@AppId", alert.AppId);
                DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleResult);
                return  await reader.ReadAsync() ? reader.GetInt32(0) : -1;
            }
            return -1;
        }
    }
}