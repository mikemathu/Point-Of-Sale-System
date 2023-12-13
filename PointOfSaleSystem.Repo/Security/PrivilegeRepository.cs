using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Security;
using PointOfSaleSystem.Service.Interfaces.Security;

namespace PointOfSaleSystem.Repo.Security
{
    public class PrivilegeRepository : IPrivilegeRepository
    {
        private IConfiguration _configuration;
        public PrivilegeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Privilege?> CreatePrivilegeAsync(Privilege privilege)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                INSERT INTO 
                                    ""Security.Privileges""
                                    (""privilegeName"" )
                                VALUES 
                                    (@privilegeName )
                                RETURNING
                                    ""privilegeName"", ""privilegeID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@privilegeName", privilege.PrivilegeName);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Privilege
                {
                    PrivilegeName = reader["privilegeName"] is DBNull ? string.Empty : (string)reader["privilegeName"],
                    PrivilegeID = reader["privilegeID"] is DBNull ? 0 : (int)reader["privilegeID"]
                };
            }
            return null;
        }
        public async Task<IEnumerable<Privilege>> GetAllPrivilegesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""privilegeID"", ""privilegeName""
                    FROM 
                        ""Security.Privileges"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<Privilege> privileges = new List<Privilege>();

            while (await reader.ReadAsync())
            {
                privileges.Add(new Privilege
                {
                    PrivilegeName = reader["privilegeName"] is DBNull ? string.Empty : (string)reader["privilegeName"],
                    PrivilegeID = reader["privilegeID"] is DBNull ? 0 : (int)reader["privilegeID"]
                });
            }
            return privileges;
        }
        public async Task<IEnumerable<Privilege>> GetRolePrivilegesAsync(int roleID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                    SELECT 
                                        P.""privilegeID"", P.""privilegeName""
                                    FROM 
                                        ""Security.RolesPrivileges"" RP
                                    INNER JOIN 
                                        ""Security.Privileges"" P 
                                    ON 
                                        RP.""privilegeID"" = P.""privilegeID""
                                    WHERE 
                                        RP.""roleID"" = @roleID ";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@roleID", roleID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<Privilege> privileges = new List<Privilege>();

            while (await reader.ReadAsync())
            {
                privileges.Add(new Privilege
                {
                    PrivilegeName = reader["privilegeName"] is DBNull ? string.Empty : (string)reader["privilegeName"],
                    PrivilegeID = reader["privilegeID"] is DBNull ? 0 : (int)reader["privilegeID"]
                });
            }
            return privileges;
        }
        public async Task<IEnumerable<Privilege>> AddPrivilegesToRoleAsync(RolePrivilege rolePrivilege)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"
                                        INSERT INTO 
                                            ""Security.RolesPrivileges""
                                            (""roleID"", ""privilegeID"")
                                        VALUES";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            for (int i = 0; i < rolePrivilege.SelectedPrivilegesIDs.Length; i++)
            {
                commandText += $@"(@roleID, @privilegeID{i}), ";
                command.Parameters.AddWithValue($"privilegeID{i}", rolePrivilege.SelectedPrivilegesIDs[i]);
            }

            // Remove the trailing comma and space
            commandText = commandText.TrimEnd(',', ' ');
            commandText += @"
                                RETURNING
                                (SELECT 
                                    P.""privilegeID""
                                 FROM ""Security.Privileges"" P
                                 WHERE P.""privilegeID"" = ""Security.RolesPrivileges"".""privilegeID""),
                                (SELECT
                                    P.""privilegeName""FROM ""Security.Privileges"" P
                                 WHERE P.""privilegeID"" = ""Security.RolesPrivileges"".""privilegeID"")";

            command.CommandText = commandText;
            command.Parameters.AddWithValue("@roleID", rolePrivilege.RoleID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<Privilege> privileges = new List<Privilege>();

            while (reader.Read())
            {
                privileges.Add(new Privilege
                {
                    PrivilegeID = reader.GetInt32(0), // privilegeID
                    PrivilegeName = reader.GetString(1) // privilegeName
                });
            }
            return privileges;
        }

        public async Task<IEnumerable<Privilege>> DeletePrivilegesFromRoleAsync(RolePrivilege rolePrivilege)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string commandText = @"
                                    DELETE FROM 
                                        ""Security.RolesPrivileges""
                                    WHERE 
                                        ""roleID"" = @roleID
                                    AND 
                                        ""privilegeID"" IN (";

                using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

                // Add privilege IDs as parameters for the IN clause
                for (int i = 0; i < rolePrivilege.SelectedPrivilegesIDs.Length; i++)
                {
                    commandText += $"@privilegeID{i}, ";
                    command.Parameters.AddWithValue($"privilegeID{i}", rolePrivilege.SelectedPrivilegesIDs[i]);
                }
                // Remove the trailing comma and space
                commandText = commandText.TrimEnd(',', ' ');

                // Complete the command text
                commandText += @")
                RETURNING
                   (
                       SELECT P.""privilegeID""
                       FROM ""Security.Privileges"" P
                       WHERE P.""privilegeID"" = ""Security.RolesPrivileges"".""privilegeID""
                   ),
                   (
                       SELECT P.""privilegeName""
                       FROM ""Security.Privileges"" P
                       WHERE P.""privilegeID"" = ""Security.RolesPrivileges"".""privilegeID""
                   )";

                command.CommandText = commandText;
                command.Parameters.AddWithValue("@roleID", rolePrivilege.RoleID);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var removedPrivileges = new List<Privilege>();

                    while (reader.Read())
                    {
                        removedPrivileges.Add(new Privilege
                        {
                            PrivilegeID = reader.GetInt32(0), // privilegeID
                            PrivilegeName = reader.GetString(1) // privilegeName
                        });
                    }
                    return removedPrivileges;
                }
            }
        }
        public async Task<Privilege?> UpdatePrivilegeAsync(Privilege privilege)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                    UPDATE
                                        ""Security.Privileges""
                                    SET
                                        ""privilegeName"" = @privilegeName
                                    WHERE
                                        ""privilegeID"" = @privilegeID 
                                    RETURNING
                                    ""privilegeName"", ""privilegeID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@privilegeName", privilege.PrivilegeName);
            command.Parameters.AddWithValue("@privilegeID", privilege.PrivilegeID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            Privilege? updatedPrivilege = null;
            if (await reader.ReadAsync())
            {
                updatedPrivilege = new Privilege
                {
                    PrivilegeName = reader["privilegeName"] is DBNull ? string.Empty : (string)reader["privilegeName"],
                    PrivilegeID = reader["privilegeID"] is DBNull ? 0 : (int)reader["privilegeID"]
                };
            }
            return updatedPrivilege;
        }
        public async Task<bool> DeleteRolePrivilegeAsync(int privilegeID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Security.Privileges"" 
                                WHERE 
                                    ""privilegeID""=@privilegeID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@privilegeID", privilegeID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }
        public async Task<bool> DoesPrivilegeExist(int privilegeID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Security.Privileges""
                                    WHERE 
                                        ""privilegeID"" = @privilegeID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@privilegeID", privilegeID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}