using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Security;
using PointOfSaleSystem.Service.Interfaces.Security;

namespace PointOfSaleSystem.Repo.Security
{
    public class RoleRepository : IRoleRepository
    {
        private IConfiguration _configuration;
        public RoleRepository(IConfiguration config)
        {
            _configuration = config;
        }
        public async Task<Role?> CreateRoleAsync(Role role)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                INSERT INTO 
                                        ""Security.Roles""
                                    (""roleName"",""description"")
                                VALUES 
                                    (@roleName,@description)
                                RETURNING
                                    ""roleName"", ""description"", ""roleID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@roleName", role.RoleName);
            command.Parameters.AddWithValue("@description", role.Description);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Role
                {
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    RoleName = reader["roleName"] is DBNull ? string.Empty : (string)reader["roleName"],
                    RoleID = (int)reader["roleID"] is DBNull ? 0 : (int)reader["roleID"]
                };
            }
            return null;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string commandText = $@"
                    SELECT 
                       ""roleID"",  ""roleName"",""description""
                    FROM 
                        ""Security.Roles""";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<Role> roles = new List<Role>();

            while (await reader.ReadAsync())
            {
                roles.Add(new Role
                {
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    RoleName = reader["roleName"] is DBNull ? string.Empty : (string)reader["roleName"],
                    RoleID = reader["roleID"] is DBNull ? 0 : (int)reader["roleID"]
                });
            }
            return roles;
        }
        public async Task<Role?> GetRoleDetailsAsync(int roleID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""roleID"",""roleName"", ""description""
                    FROM 
                        ""Security.Roles""
                    WHERE 
                        ""roleID"" = @roleID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@roleID", roleID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Role
                {
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    RoleName = reader["roleName"] is DBNull ? string.Empty : (string)reader["roleName"],
                    RoleID = reader["roleID"] is DBNull ? 0 : (int)reader["roleID"]
                };
            }
            return null;
        }

        public async Task<bool> DeleteRoleAsync(int roleID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                        ""Security.Roles""
                                    WHERE 
                                        ""roleID"" = @roleID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@roleID", roleID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }

        public async Task<bool> DoesRoleExist(int roleID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Security.Roles"" 
                                    WHERE 
                                        ""roleID"" = @roleID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@roleID", roleID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }

        public async Task<Role?> UpdateRoleAsync(Role role)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                UPDATE
                                    ""Security.Roles""
                                SET
                                    ""description"" = @description, ""roleName""  = @roleName
                                WHERE
                                    ""roleID"" = @roleID 
                                RETURNING
                                    ""description"", ""roleName"", ""roleID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@description", role.Description);
            command.Parameters.AddWithValue("@roleName", role.RoleName);
            command.Parameters.AddWithValue("@roleID", role.RoleID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            Role updatedRole = null;
            if (await reader.ReadAsync())
            {
                updatedRole = new Role
                {
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    RoleName = reader["roleName"] is DBNull ? string.Empty : (string)reader["roleName"],
                    RoleID = reader["roleID"] is DBNull ? 0 : (int)reader["roleID"]
                };
            }
            return updatedRole;
        }

        public IEnumerable<string> GetUserPrivilege(int userID)
        {
            List<string> privilegeNames = new List<string>();

            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            connection.Open();
            string commandText = @"
                SELECT DISTINCT 
                    P.""privilegeName""
                FROM 
                    ""Security.Privileges"" P
                INNER JOIN 
                    ""Security.RolesPrivileges"" RP ON P.""privilegeID"" = RP.""privilegeID""
                INNER JOIN 
                    ""Security.Roles"" R ON RP.""roleID"" = R.""roleID""
                INNER JOIN 
                    ""Security.SystemUsersRoles"" UR ON R.""roleID"" = UR.""roleID""
                WHERE 
                    UR.""sysUserID"" = @sysUserID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@sysUserID", userID);

            using NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string privilegeName = reader["privilegeName"] is DBNull ? string.Empty : (string)reader["privilegeName"];
                privilegeNames.Add(privilegeName);
            }
            return privilegeNames;
        }
    }
}
