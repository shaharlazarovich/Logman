using System;
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
    public class AccountRepository : IAccountRepository
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public async Task<User> CreateUserAsync(User newUser)
        {
            CheckUnitOfWork();
            using (DbConnection conn = UnitOfWork.Connection)
            {
                await conn.OpenAsync();
                const string spName = "CreateUser";
                var command = conn.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    string activationKey = Guid.NewGuid().ToString();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@UserName", newUser.Username);
                    command.Parameters.AddWithValue("@Password", newUser.Password);
                    command.Parameters.AddWithValue("@ActivationKey", activationKey);
                    command.Parameters.AddWithValue("@PasswordSalt", newUser.PasswordSalt);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Username = newUser.Username,
                                Password = newUser.Password,
                                Enabled = false,
                                Id = (long) reader.GetDecimal(0),
                                ActivationKey = activationKey
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<User> GetUserAsync(long id)
        {
            CheckUnitOfWork();
            using (var conn = UnitOfWork.Connection)
            {
                await conn.OpenAsync();
                const string spName = "GETUSERBYID";
                var command = conn.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@ID", id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Mapper.Map<User>(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<User> GetUserAsync(string userName)
        {
            CheckUnitOfWork();
            using (DbConnection conn = UnitOfWork.Connection)
            {
                await conn.OpenAsync();
                const string spName = "GETUSERBYNAME";
                var command = conn.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@USERNAME", userName);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Mapper.Map<User>(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<User> ActivateUserAsync(string activationKey)
        {
            CheckUnitOfWork();
            using (DbConnection conn = UnitOfWork.Connection)
            {
                const string spName = "ConfirmUser";
                var command = conn.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@activationKey", activationKey);
                    conn.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                    {
                        if (await reader.ReadAsync())
                        {
                            return Mapper.Map<User>(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task ChangePasswordAsync(string userName, string password, string passwordSalt)
        {
            CheckUnitOfWork();
            using (DbConnection conn = UnitOfWork.Connection)
            {
                const string spName = "ChangePassword";
                var command = conn.CreateCommand() as SqlCommand;
                if (command != null)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    command.Parameters.AddWithValue("@userName", userName);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@passwordSalt", passwordSalt);
                    conn.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        private void CheckUnitOfWork()
        {
            if (UnitOfWork == null)
            {
                throw new InvalidOleVariantTypeException("Unit of work is not provided.");
            }
        }
    }
}