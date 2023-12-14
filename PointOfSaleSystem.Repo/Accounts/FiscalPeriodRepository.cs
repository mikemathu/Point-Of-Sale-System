using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class FiscalPeriodRepository : IFiscalPeriodRepository
    {
        private readonly IConfiguration _configuration;
        public FiscalPeriodRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<FiscalPeriod>> GetAllFiscalPeriodsAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT
                                        ""fiscalPeriodID"", ""fiscalPeriodNo"", ""openDate"", ""closeDate"", ""isActive"", ""isOpen""
                                    FROM  
                                        ""Accounts.Fiscal.FiscalPeriods"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<FiscalPeriod> fiscalPeriods = new List<FiscalPeriod>();

            while (await reader.ReadAsync())
            {
                fiscalPeriods.Add(new FiscalPeriod
                {
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    FiscalPeriodNo = reader["fiscalPeriodNo"] is DBNull ? 0 : (int)reader["fiscalPeriodNo"],
                    OpenDate = reader["openDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["openDate"],
                    CloseDate = reader["closeDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["closeDate"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"]
                });
            }
            return fiscalPeriods;
        }
        public async Task<int?> GetActiveFiscalPeriodID()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT
                                        ""fiscalPeriodID""
                                    FROM 
                                        ""Accounts.Fiscal.FiscalPeriods""
                                    ORDER BY 
                                        ""isActive"" DESC, ""fiscalPeriodID"" DESC
                                    LIMIT 1";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"];
            }
            return null;
        }

        public void AddFiscalPeriodAsync(FiscalPeriod fiscalPeriodID)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsFiscalPeriodActiveAsync(int fiscalPeriodID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""isActive""
                                FROM
                                    ""Accounts.Fiscal.FiscalPeriods""
                                WHERE 
                                    ""fiscalPeriodID"" = @fiscalPeriodID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);

            int isActive = 0;

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                isActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"];
            }
            return isActive > 0;
        }

        public async Task<bool> IsFiscalPeriodOpenAsync(int fiscalPeriodID)
        {

            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""isOpen""
                                FROM
                                    ""Accounts.Fiscal.FiscalPeriods""
                                WHERE 
                                    ""fiscalPeriodID"" = @fiscalPeriodID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);

            int isOpen = 0;

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                isOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"];
            }
            return isOpen > 0;
        }

        public async Task<FiscalPeriod?> CloseFiscalPeriodAsync(int fiscalPeriodID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    WITH PeriodsWithLag AS (
                        SELECT 
                            ""fiscalPeriodID"",
                            ""isOpen"",
                            LAG(""isOpen"") OVER (ORDER BY ""fiscalPeriodID"") AS prev_isOpen
                        FROM 
                            ""Accounts.Fiscal.FiscalPeriods""
                    )
                    UPDATE ""Accounts.Fiscal.FiscalPeriods"" AS current_period
                    SET ""isOpen"" = @isOpen
                    FROM PeriodsWithLag
                    WHERE 
                        current_period.""fiscalPeriodID"" = PeriodsWithLag.""fiscalPeriodID""
                        AND current_period.""fiscalPeriodID"" = @fiscalPeriodID 
                        AND PeriodsWithLag.prev_isOpen = @isOpen 
                    RETURNING
                        current_period.""fiscalPeriodNo"", current_period.""openDate"", 
                        current_period.""closeDate"" , current_period.""isActive"", current_period.""isOpen""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);
            command.Parameters.AddWithValue("@isOpen", 0);

            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            FiscalPeriod? fiscalPeriod = null;

            if (await reader.ReadAsync())
            {
                fiscalPeriod = new FiscalPeriod
                {
                    FiscalPeriodNo = reader["fiscalPeriodNo"] is DBNull ? 0 : (int)reader["fiscalPeriodNo"],
                    OpenDate = reader["openDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["openDate"],
                    CloseDate = reader["closeDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["closeDate"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"]
                };
            }
            return fiscalPeriod;
            /* int isOpen = 1;

             if (await reader.ReadAsync())
             {
                 {
                     isOpen = (int)reader["isOpen"];
                 }
             }*/
        }

        public async Task<FiscalPeriod?> CreateFiscalPeriodAsync(FiscalPeriod fiscalPeriod)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            DateTime closeDate = fiscalPeriod.CloseDate;
            closeDate = closeDate.Date.AddDays(1).AddSeconds(-1);

            string commandText = $@"
                INSERT INTO 
                        ""Accounts.Fiscal.FiscalPeriods"" 
                            (""openDate"", ""closeDate"", ""isActive"", ""isOpen"")
                VALUES 
                        ( @openDate, @closeDate, @isActive, @isOpen)
                RETURNING 
                       ""fiscalPeriodID"", ""fiscalPeriodNo"", ""openDate"", ""closeDate"", ""isActive"", ""isOpen""  ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@openDate", fiscalPeriod.OpenDate);
            command.Parameters.AddWithValue("@closeDate", closeDate);
            command.Parameters.AddWithValue("@isActive", fiscalPeriod.IsActive);

            command.Parameters.AddWithValue("@isOpen", 1); //TODO: implement this using bussiness aspect

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new FiscalPeriod
                {
                    CloseDate = reader["closeDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["closeDate"],
                    FiscalPeriodNo = reader["fiscalPeriodNo"] is DBNull ? 0 : (int)reader["fiscalPeriodNo"],
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"],
                    OpenDate = reader["openDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["openDate"]
                };
            }
            return null;
        }
        public async Task<IEnumerable<FiscalPeriod>> GetAllActiveFiscalPeriodsAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""closeDate"", ""fiscalPeriodID"", ""fiscalPeriodNo"", ""isActive"", ""isOpen"", ""openDate""
                    FROM 
                        ""Accounts.Fiscal.FiscalPeriods""
                    WHERE 
                        ""isActive"" = @isActive";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@isActive", 1);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<FiscalPeriod> fiscalPeriods = new List<FiscalPeriod>();

            while (await reader.ReadAsync())
            {
                fiscalPeriods.Add(new FiscalPeriod
                {
                    CloseDate = reader["closeDate"] is DBNull ? DateTime.MaxValue : (DateTime)reader["closeDate"],
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    FiscalPeriodNo = reader["fiscalPeriodNo"] is DBNull ? 0 : (int)reader["fiscalPeriodNo"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"],
                    OpenDate = reader["openDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["openDate"]
                });
            }
            return fiscalPeriods;
        }
        public async Task<IEnumerable<FiscalPeriod>> GetAllInActiveFiscalPeriodsAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        *
                    FROM 
                        ""Accounts.Fiscal.FiscalPeriods""
                    WHERE 
                        ""isActive"" = @isActive";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@isActive", 0);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<FiscalPeriod> fiscalPeriods = new List<FiscalPeriod>();

            while (await reader.ReadAsync())
            {
                fiscalPeriods.Add(new FiscalPeriod
                {
                    CloseDate = reader["closeDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["closeDate"],
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    FiscalPeriodNo = reader["fiscalPeriodNo"] is DBNull ? 0 : (int)reader["fiscalPeriodNo"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"],
                    OpenDate = reader["openDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["openDate"]
                });
            }
            return fiscalPeriods;
        }

        public async Task<FiscalPeriod?> GetFiscalPeriodDetailsAsync(int fiscalPeriodID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    *  
                                FROM 
                                    ""Accounts.Fiscal.FiscalPeriods""
                                WHERE 
                                    ""fiscalPeriodID"" = @fiscalPeriodID ";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new FiscalPeriod
                {
                    CloseDate = (DateTime)reader["closeDate"],
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    FiscalPeriodNo = reader["fiscalPeriodNo"] is DBNull ? 0 : (int)reader["fiscalPeriodNo"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"],
                    OpenDate = reader["openDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["openDate"]
                };
            }
            return null;
        }
        public async Task<bool> IsFiscalPeriodRangeOverlapAsync(FiscalPeriod fiscalPeriod)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                  SELECT COUNT(*)
                                    FROM 
                                        ""Accounts.Fiscal.FiscalPeriods""
                                    WHERE
                                        ""openDate"" BETWEEN @openDate AND @closeDate
                                    OR 
                                        ""closeDate"" BETWEEN @openDate AND @closeDate";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@openDate", fiscalPeriod.OpenDate);
            command.Parameters.AddWithValue("@closeDate", fiscalPeriod.CloseDate);

            await connection.OpenAsync();

            int overlapCount = Convert.ToInt32(command.ExecuteScalar());

            return overlapCount > 0;
        }
        public async Task<FiscalPeriod?> UpdateFiscalPeriodAsync(FiscalPeriod fiscalPeriod)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            DateTime closeDate = fiscalPeriod.CloseDate;
            closeDate = closeDate.Date.AddDays(1).AddSeconds(-1);

            string commandText = $@"UPDATE 
                                        ""Accounts.Fiscal.FiscalPeriods"" 
                                SET
                                    ""openDate"" = @openDate,
                                    ""closeDate"" = @closeDate,
                                    ""isActive"" = @isActive
                                WHERE
                                    ""fiscalPeriodNo"" = @fiscalPeriodNo
                                RETURNING
                                    ""fiscalPeriodNo"", ""openDate"", ""closeDate"", ""isActive"", ""isOpen""  ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@openDate", fiscalPeriod.OpenDate);
            command.Parameters.AddWithValue("@closeDate", closeDate);
            command.Parameters.AddWithValue("@isActive", fiscalPeriod.IsActive);
            command.Parameters.AddWithValue("@fiscalPeriodNo", fiscalPeriod.FiscalPeriodNo);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new FiscalPeriod
                {
                    CloseDate = reader["closeDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["closeDate"],
                    OpenDate = reader["openDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["openDate"],
                    FiscalPeriodNo = reader["fiscalPeriodNo"] is DBNull ? 0 : (int)reader["fiscalPeriodNo"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsOpen = reader["isOpen"] is DBNull ? 0 : (int)reader["isOpen"]
                };
            }
            return null;
        }
        public async Task<bool> DeleteFiscalPeriodAsync(int fiscalPeriodID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Accounts.Fiscal.FiscalPeriods""
                                WHERE 
                                    ""fiscalPeriodID"" = @fiscalPeriodID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }

        public async Task<bool> DoesFiscalPeriodExist(int fiscalPeriodID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                       ""Accounts.Fiscal.FiscalPeriods""
                                    WHERE 
                                        ""fiscalPeriodID"" = @fiscalPeriodID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}
