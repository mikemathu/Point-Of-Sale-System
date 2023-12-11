using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Security;
using PointOfSaleSystem.Service.Interfaces.Security;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Repo.Security
{
    public class SystemUserRepository : ISystemUserRepository
    {
        private readonly IConfiguration _configuration;
        public SystemUserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<SystemUser?> AuthenticateUserAsync(SystemUser systemUser)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string commandText = @"
                                        SELECT 
                                            ""userName"", ""sysUserID""
                                        FROM 
                                            ""Security.SystemUsers""                                                        
                                        WHERE 
                                            ""userName"" = @userName
                                        AND 
                                            ""password"" = @password ";
                using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@userName", systemUser.UserName);
                command.Parameters.AddWithValue("@password", systemUser.Password);

                await connection.OpenAsync();

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new SystemUser
                        {
                            SysUserID = reader["sysUserID"] is DBNull ? 0 : (int)reader["sysUserID"],
                            UserName = reader["userName"] is DBNull ? string.Empty : (string)reader["userName"]
                        };
                    }
                }
                return null;
            }
        }

        public async Task<SystemUser?> RegisterUserAsync(SystemUser systemUser)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                INSERT INTO 
                        ""Security.SystemUsers"" 
                            (""userName"", ""password"", ""dateTimeCreated"")
                VALUES 
                        ( @userName, @password, @dateTimeCreated)
                RETURNING  
                        ""sysUserID"", ""userName""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@userName", systemUser.UserName);
            command.Parameters.AddWithValue("@password", systemUser.Password);
            command.Parameters.AddWithValue("@dateTimeCreated", DateTime.Now);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new SystemUser
                {
                    SysUserID = reader["sysUserID"] is DBNull ? 0 : (int)reader["sysUserID"],
                    UserName = reader["userName"] is DBNull ? string.Empty : (string)reader["userName"]
                };
            }
            return null;
        }

        public async Task<bool> CreateSystemUserAsync(SystemUser user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string userInsertCommand = @"
                    INSERT INTO  ""Security.SystemUsers"" 
                        (
                            ""otherNames"",
                            ""password"",
                            ""surName"",
                            ""userName"",
                            ""dateTimeCreated""
                        )
                    VALUES
                        (
                            @Othernames,
                            @Password,
                            @Surname,
                            @Username,
                            @DateTimeCreated
                        )
                    RETURNING ""sysUserID""";
                        int sysUserID = 0;
                        using (NpgsqlCommand userCommand = new NpgsqlCommand(userInsertCommand, connection))
                        {

                            userCommand.Transaction = transaction;

                            userCommand.Parameters.AddWithValue("@Othernames", user.OtherNames);
                            userCommand.Parameters.AddWithValue("@Password", user.Password);
                            userCommand.Parameters.AddWithValue("@Surname", user.SurName);
                            userCommand.Parameters.AddWithValue("@Username", user.UserName);
                            userCommand.Parameters.AddWithValue("@DateTimeCreated", DateTime.Now);

                            sysUserID = (int)await userCommand.ExecuteScalarAsync();
                        }

                        string usersRolesInsertCommand = @"
                                INSERT INTO 
                                     ""Security.SystemUsersRoles"" (""sysUserID"", ""roleID"") 
                                VALUES 
                                     (@sysUserID, @RoleID)";

                        using (NpgsqlCommand usersRolesInsertCmd = new NpgsqlCommand(usersRolesInsertCommand, connection))
                        {
                            usersRolesInsertCmd.Transaction = transaction;
                            int rowsAffected = 0;

                            foreach (int roleID in user.UserRolesIDs)
                            {
                                usersRolesInsertCmd.Parameters.Clear();
                                usersRolesInsertCmd.Parameters.AddWithValue("@sysUserID", sysUserID);
                                usersRolesInsertCmd.Parameters.AddWithValue("@RoleID", roleID);
                                rowsAffected += await usersRolesInsertCmd.ExecuteNonQueryAsync();
                            }
                            if (rowsAffected < user.UserRolesIDs.Length)
                            {
                                transaction.Commit();
                                throw new FalseException("User was inserted, but some roles were not updated. Try adding and/or removing the roles again and update.");
                            }
                        }
                        transaction.Commit();
                        return sysUserID > 0;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> UpdateSystemUserAsync(SystemUser user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update the systemUser information in the "Security.Users" table
                        string userUpdateCommand = @"
                            UPDATE  ""Security.SystemUsers"" 
                            SET
                                ""otherNames"" = @OtherNames,
                                ""password"" = @Password,
                                ""surName"" = @SurName,
                                ""userName"" = @UserName
                            WHERE
                                ""sysUserID"" = @sysUserID";
                        int userUpdateResult = 0;
                        using (NpgsqlCommand userCommand = new NpgsqlCommand(userUpdateCommand, connection))
                        {

                            userCommand.Transaction = transaction;

                            userCommand.Parameters.AddWithValue("@OtherNames", user.OtherNames);
                            userCommand.Parameters.AddWithValue("@Password", user.Password);
                            userCommand.Parameters.AddWithValue("@SurName", user.SurName);
                            userCommand.Parameters.AddWithValue("@UserName", user.UserName);
                            userCommand.Parameters.AddWithValue("@sysUserID", user.SysUserID);

                            userUpdateResult = await userCommand.ExecuteNonQueryAsync();
                        }
                        // Delete existing roles for the systemUser from "Security.UsersRoles" table
                        string deleteRolesCommand = @"
                            DELETE FROM ""Security.SystemUsersRoles""
                            WHERE ""sysUserID"" = @sysUserID";

                        using (NpgsqlCommand deleteRolesCmd = new NpgsqlCommand(deleteRolesCommand, connection))
                        {
                            deleteRolesCmd.Transaction = transaction;

                            deleteRolesCmd.Parameters.AddWithValue("@sysUserID", user.SysUserID);

                            await deleteRolesCmd.ExecuteNonQueryAsync();
                        }

                        // Insert new roles for the systemUser into "Security.UsersRoles" table
                        string insertRolesCommand = @"
                            INSERT INTO ""Security.SystemUsersRoles"" (""sysUserID"", ""roleID"")
                            VALUES (@sysUserID, @RoleID)";

                        using (NpgsqlCommand insertRolesCmd = new NpgsqlCommand(insertRolesCommand, connection))
                        {
                            insertRolesCmd.Transaction = transaction;
                            int rowsAffected = 0;

                            foreach (int roleID in user.UserRolesIDs)
                            {
                                insertRolesCmd.Parameters.Clear();
                                insertRolesCmd.Parameters.AddWithValue("@sysUserID", user.SysUserID);
                                insertRolesCmd.Parameters.AddWithValue("@RoleID", roleID);
                                rowsAffected += await insertRolesCmd.ExecuteNonQueryAsync();
                            }

                            if (rowsAffected < user.UserRolesIDs.Length)
                            {
                                transaction.Commit();
                                throw new FalseException("User was inserted, but some roles were not updated. Try adding and/or removing the roles again and update.");
                            }
                        }
                        // Commit the transaction if everything was successful
                        transaction.Commit();
                        return userUpdateResult > 0;
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions or rethrow with more specific information
                        transaction.Rollback();
                        throw new Exception("Failed to update the systemUser and roles: " + ex.Message, ex);
                    }
                }
            }
        }

        public async Task<IEnumerable<SystemUser>> GetAllUsersAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""dateTimeCreated"",""otherNames"",""sysUserID"",""userName"",""surName""
                    FROM 
                         ""Security.SystemUsers"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<SystemUser> users = new List<SystemUser>();

            while (await reader.ReadAsync())
            {
                users.Add(new SystemUser
                {
                    DateTimeCreated = reader["dateTimeCreated"] is DBNull ? DateTime.MinValue : (DateTime)reader["dateTimeCreated"],
                    OtherNames = reader["otherNames"] is DBNull ? string.Empty : (string)reader["otherNames"],
                    SysUserID = reader["sysUserID"] is DBNull ? 0 : (int)reader["sysUserID"],
                    UserName = reader["userName"] is DBNull ? string.Empty : (string)reader["userName"],
                    SurName = reader["surName"] is DBNull ? string.Empty : (string)reader["surName"]
                });
            }
            return users;
        }

        public async Task<SystemUser?> GetUserDetailsAsync(int sysUserID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"
                                   SELECT 
                                       u.""surName"", u.""otherNames"", u.""userName"", u.""dateTimeCreated"", u.""password"", u.""sysUserID"",
                                           array_agg(ur.""roleID"") as ""UserRolesIDs""
                                   FROM  
                                       ""Security.SystemUsers""  u
                                   LEFT JOIN 
                                       ""Security.SystemUsersRoles"" ur ON u.""sysUserID"" = ur.""sysUserID""
                                   WHERE 
                                       u.""sysUserID"" = @sysUserID
                                   GROUP BY 
                                       u.""surName"", u.""otherNames"", u.""userName"", u.""dateTimeCreated"", u.""password"", u.""sysUserID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@sysUserID", sysUserID);

            await connection.OpenAsync();


            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                try
                {
                    return new SystemUser
                    {
                        SurName = reader["surName"] is DBNull ? string.Empty : (string)reader["surName"],
                        OtherNames = reader["otherNames"] is DBNull ? string.Empty : (string)reader["otherNames"],
                        UserName = reader["userName"] is DBNull ? string.Empty : (string)reader["userName"],
                        DateTimeCreated = reader["dateTimeCreated"] is DBNull ? DateTime.MinValue : (DateTime)reader["dateTimeCreated"],
                        Password = reader["password"] is DBNull ? string.Empty : (string)reader["password"],
                        SysUserID = reader["sysUserID"] is DBNull ? 0 : (int)reader["sysUserID"],
                        UserRolesIDs = reader["UserRolesIDs"] is DBNull ? Array.Empty<int>() : reader["UserRolesIDs"] as int[]
                    };
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Cannot read a non-nullable collection of elements because the returned array contains nulls"))
                    {
                        return new SystemUser
                        {
                            SurName = reader["surName"] is DBNull ? string.Empty : (string)reader["surName"],
                            OtherNames = reader["otherNames"] is DBNull ? string.Empty : (string)reader["otherNames"],
                            UserName = reader["userName"] is DBNull ? string.Empty : (string)reader["userName"],
                            DateTimeCreated = reader["dateTimeCreated"] is DBNull ? DateTime.MinValue : (DateTime)reader["dateTimeCreated"],
                            Password = reader["password"] is DBNull ? string.Empty : (string)reader["password"],
                            SysUserID = reader["sysUserID"] is DBNull ? 0 : (int)reader["sysUserID"],
                        };
                    }
                }
            }
            return null;
        }

        public async Task<bool> DoesUserExist(int systemUserID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Security.SystemUsers""
                                    WHERE 
                                        ""sysUserID"" = @sysUserID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@sysUserID", systemUserID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
        public async Task<bool> AuthenticateAccessPasswordAsync(int userID, string password)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Security.SystemUsers""
                                    WHERE 
                                        ""sysUserID"" = @sysUserID
                                    AND 
                                        ""password"" = @password ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@sysUserID", userID);
            command.Parameters.AddWithValue("@password", password);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}
