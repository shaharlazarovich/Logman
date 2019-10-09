using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using Logman.Common.Code;
using Logman.Common.Data;
using Logman.Common.DomainObjects;
using Microsoft.Practices.ObjectBuilder2;

namespace Logman.Data.SqlServer.Base
{
    internal class ApplicationRepository : IApplicationRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public async Task<long> Add(Application record)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new Application();
            const string getAppByIdSprocName = "CreateApplication";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = getAppByIdSprocName;
                    command.Parameters.AddWithValue("@name", record.AppName);
                    command.Parameters.AddWithValue("@appKey", record.AppKey);
                    command.Parameters.AddWithValue("@retentionDays", record.DefaultRetainPeriodDays);
                    command.Parameters.AddWithValue("@fatals", record.MaxFatalErrors);
                    command.Parameters.AddWithValue("@errors", record.MaxErrors);
                    command.Parameters.AddWithValue("@warnings", record.MaxWarnings);
                    command.Parameters.AddWithValue("@active", record.Enabled);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                    {
                        if (reader.HasRows && await reader.ReadAsync())
                        {
                            result = Mapper.Map<Application>(reader);
                        }
                    }
                }
            }
            return result.Id;
        }


        public async Task<Application> GetByIdAsync(long id)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new Application();
            const string getAppByIdSprocName = "GetApplicationById";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = getAppByIdSprocName;
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows && await reader.ReadAsync())
                        {
                            result = Mapper.Map<Application>(reader);
                            if (result != null)
                            {
                                await PopulateUsers(result);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public async Task<Application> GetByIdAsync(string id)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new Application();
            const string getAppByIdSprocName = "GetApplication";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = getAppByIdSprocName;
                    command.Parameters.AddWithValue("@AppKey", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows && await reader.ReadAsync())
                        {
                            result = Mapper.Map(reader, result);
                            if (result != null)
                            {
                                await PopulateUsers(result);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<Application>> FindAllAsync()
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new List<Application>();
            const string spName = "GetAllApps";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(Mapper.Map<Application>(reader));
                        }
                    }
                }
            }
            return result;
        }

        public async Task<ApplicationTrends> GetApplicationTrendAsync(long id)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new ApplicationTrends();
            const string getAppByIdSprocName = "GetEventTrendOfApp";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = getAppByIdSprocName;
                    command.Parameters.AddWithValue("@AppId", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        //Fatals
                        while (await reader.ReadAsync())
                        {
                            DateTime key = reader.GetDateTime(0).Date;
                            int value = reader.GetInt32(1);
                            if (!result.Fatals.ContainsKey(key))
                            {
                                result.Fatals.TryAdd(key, value);
                            }
                            else
                            {
                                result.Fatals[key] = result.Fatals[key] + value;
                            }
                        }

                        await reader.NextResultAsync();

                        // Errors
                        while (await reader.ReadAsync())
                        {
                            DateTime key = reader.GetDateTime(0).Date;
                            int value = reader.GetInt32(1);
                            if (!result.Fatals.ContainsKey(key))
                            {
                                result.Errors.TryAdd(key, value);
                            }
                            else
                            {
                                result.Errors[key] = result.Fatals[key] + value;
                            }
                        }


                        await reader.NextResultAsync();

                        // Warnings
                        while (await reader.ReadAsync())
                        {
                            DateTime key = reader.GetDateTime(0).Date;
                            int value = reader.GetInt32(1);
                            if (!result.Fatals.ContainsKey(key))
                            {
                                result.Warnings.TryAdd(key, value);
                            }
                            else
                            {
                                result.Warnings[key] = result.Fatals[key] + value;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public async Task<ApplicationStatus> GetAppStatusAsync(long id)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new ApplicationStatus();
            const string getAppByIdSprocName = "GetAppStatus";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = getAppByIdSprocName;
                    command.Parameters.AddWithValue("@AppId", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows && await reader.ReadAsync())
                        {
                            result = Mapper.Map(reader, result);
                        }
                    }
                }
            }
            return result;
        }


        public async Task<Application> UpdateApplicationAsync(Application record)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new Application();
            const string getAppByIdSprocName = "UpdateApplication";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = getAppByIdSprocName;
                    command.Parameters.AddWithValue("@name", record.AppName);
                    command.Parameters.AddWithValue("@appKey", record.AppKey);
                    command.Parameters.AddWithValue("@retentionDays", record.DefaultRetainPeriodDays);
                    command.Parameters.AddWithValue("@fatals", record.MaxFatalErrors);
                    command.Parameters.AddWithValue("@errors", record.MaxErrors);
                    command.Parameters.AddWithValue("@warnings", record.MaxWarnings);
                    command.Parameters.AddWithValue("@active", record.Enabled);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                    {
                        if (reader.HasRows && await reader.ReadAsync())
                        {
                            result = Mapper.Map<Application>(reader);
                        }
                    }
                }
            }
            return result;
        }

        public async Task AddAppUser(long appId, long userId, Roles role)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            const string sprocName = "AddAppUser";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = sprocName;
                    command.Parameters.AddWithValue("@appId", appId);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@roleId", (int) role);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<ApplicationEvents> GetApplicationEventsAsync(long appId, int? pageNumber = null,
            int? pageSize = null, string keywords = null, DateTime? fromDate = null, DateTime? toDate = null,
            EventLevel? eventLevel = null)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new ApplicationEvents();
            const string getAppByIdSprocName = "GETAPPEVENTS";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = getAppByIdSprocName;
                    command.Parameters.AddWithValue("@AppId", appId);
                    command.Parameters.AddWithValue("@PAGENUMBER", pageNumber.HasValue ? pageNumber.Value : 0);
                    command.Parameters.AddWithValue("@PAGESIZE",
                        pageSize.HasValue ? pageSize.Value : Constants.SpecialValues.MaxRowCount);
                    command.Parameters.AddWithValue("@KEYWORD", keywords);
                    command.Parameters.AddWithValue("@FDATE", fromDate);
                    command.Parameters.AddWithValue("@TODATE", toDate);
                    command.Parameters.AddWithValue("@EVENTLEVEL", (int) eventLevel);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.TopEvents.Add(reader.GetString(reader.GetOrdinal("MESSAGE")),
                                reader.GetInt32(reader.GetOrdinal("CNT")));
                        }

                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            result.Events.Add(Mapper.Map<Event>(reader));
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<Alert>> GetAlertsOfApp(long appId)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new List<Alert>();
            const string sprocName = "GetAlerts";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = sprocName;
                    command.Parameters.AddWithValue("@appId", appId);
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var alert = Mapper.Map<Alert>(reader);
                        alert.AppId = appId;
                        result.Add(alert);
                    }
                }
            }
            return result;
        }

        public async Task UpdateAlertExecTimes(Dictionary<long, DateTime> listOfAlerts)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            const string sprocName = "UpdateAlertExecTimes";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = sprocName;
                    var param = new SqlParameter("@updateTimes", SqlDbType.Structured)
                    {
                        Value = ConvertToDataTable(listOfAlerts)
                    };
                    command.Parameters.Add(param);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Application>> GetAppsOfUserAsync(long userId)
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }

            var result = new List<Application>();
            const string spName = "GetUserApps";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(Mapper.Map<Application>(reader));
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<User>> GetApplicationUsersAsync(long appId)
        {
            var result = new List<User>();
            const string spName = "GetAppUser";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                connection.Close();
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@appId", appId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new User
                            {
                                Id = reader.GetInt64(0),
                                Username = reader.GetString(2),
                                CurrentRole = (Roles) reader.GetInt32(1)
                            });
                        }
                    }
                }
            }
            return result;
        }

        private async Task<Application> PopulateUsers(Application instance)
        {
            var result = new Application();
            const string spName = "GetAppUser";
            using (DbConnection connection = UnitOfWork.Connection)
            {
                connection.Close();
                await connection.OpenAsync();
                var command = connection.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@appId", instance.Id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            instance.Users.TryAdd(reader.GetInt64(0), (Roles) reader.GetInt32(1));
                        }
                    }
                }
            }
            return result;
        }

        private static DataTable ConvertToDataTable(Dictionary<long, DateTime> dict)
        {
            var dt = new DataTable();
            dt.Columns.Add("Key", typeof (long));
            dt.Columns.Add("Value", typeof (DateTime));
            dict.ForEach(pair =>
            {
                DataRow row = dt.NewRow();
                row["Key"] = pair.Key;
                row["Value"] = pair.Value;
                dt.Rows.Add(row);
            });
            return dt;
        }
    }
}