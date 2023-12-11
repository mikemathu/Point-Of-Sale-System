using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Interfaces.Inventory;

namespace PointOfSaleSystem.Repo.Inventory
{
    public class UnitofMeasureRepository : IUnitofMeasureRepository
    {
        private IConfiguration _configuration;
        public UnitofMeasureRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<UnitOfMeasure?> CreateUnitOfMeasureAsync(UnitOfMeasure unitOfMeasureDto)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    INSERT INTO 
                       ""Inventory.Inventory.UnitsOfMeasure""
                            (""unitOfMeasureName"",
                            ""isSmallestUnit"" )
                       VALUES 
                           (@unitOfMeasureName,
                            @isSmallestUnit )
                        RETURNING
                            ""unitOfMeasureID"", ""unitOfMeasureName"", ""isSmallestUnit"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@unitOfMeasureName", unitOfMeasureDto.UnitOfMeasureName);
            command.Parameters.AddWithValue("@isSmallestUnit", unitOfMeasureDto.IsSmallestUnit);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UnitOfMeasure
                {
                    UnitOfMeasureID = reader["unitOfMeasureID"] is DBNull ? 0 : (int)reader["unitOfMeasureID"],
                    UnitOfMeasureName = reader["unitOfMeasureName"] is DBNull ? string.Empty : (string)reader["unitOfMeasureName"],
                    IsSmallestUnit = reader["isSmallestUnit"] is DBNull ? 0 : (int)reader["isSmallestUnit"]
                };
            }
            return null;
        }

        public async Task<UnitOfMeasure?> UpdateUnitOfMeasureAsync(UnitOfMeasure unitOfMeasureDto)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    UPDATE
                        ""Inventory.Inventory.UnitsOfMeasure""
                    SET
                        ""unitOfMeasureName"" = @unitOfMeasureName,
                        ""isSmallestUnit""  =@isSmallestUnit
                    WHERE
                        ""unitOfMeasureID"" = @unitOfMeasureID 
                    RETURNING
                        ""unitOfMeasureID"", ""unitOfMeasureName"", ""isSmallestUnit""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@unitOfMeasureName", unitOfMeasureDto.UnitOfMeasureName);
            command.Parameters.AddWithValue("@isSmallestUnit", unitOfMeasureDto.IsSmallestUnit);
            command.Parameters.AddWithValue("@unitOfMeasureID", unitOfMeasureDto.UnitOfMeasureID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UnitOfMeasure
                {
                    UnitOfMeasureID = reader["unitOfMeasureID"] is DBNull ? 0 : (int)reader["unitOfMeasureID"],
                    UnitOfMeasureName = reader["unitOfMeasureName"] is DBNull ? string.Empty : (string)reader["unitOfMeasureName"],
                    IsSmallestUnit = reader["isSmallestUnit"] is DBNull ? 0 : (int)reader["isSmallestUnit"]
                };
            }
            return null;
        }

        public async Task<UnitOfMeasure?> GetUnitOfMeasureDetailsAsync(int unitOfMeasureID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""unitOfMeasureID"", ""unitOfMeasureName"", ""isSmallestUnit""
                    FROM 
                       ""Inventory.Inventory.UnitsOfMeasure""
                    WHERE 
                        ""unitOfMeasureID"" = @unitOfMeasureID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@unitOfMeasureID", unitOfMeasureID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UnitOfMeasure
                {
                    UnitOfMeasureID = reader["unitOfMeasureID"] is DBNull ? 0 : (int)reader["unitOfMeasureID"],
                    UnitOfMeasureName = reader["unitOfMeasureName"] is DBNull ? string.Empty : (string)reader["unitOfMeasureName"],
                    IsSmallestUnit = reader["isSmallestUnit"] is DBNull ? 0 : (int)reader["isSmallestUnit"]
                };
            }
            return null;
        }

        public async Task<bool> DeleteUnitOfMeasureAsync(int unitOfMeasureID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Inventory.Inventory.UnitsOfMeasure""
                                WHERE 
                                    ""unitOfMeasureID"" = @unitOfMeasureID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@unitOfMeasureID", unitOfMeasureID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> DoesUnitOfMeasureExist(int unitOfMeasureID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                       ""Inventory.Inventory.UnitsOfMeasure""
                                    WHERE 
                                        ""unitOfMeasureID"" = @unitOfMeasureID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@unitOfMeasureID", unitOfMeasureID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}