using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class CashFlowCategoryRepository : ICashFlowCategoryRepository
    {
        private readonly IConfiguration _configuration;
        public CashFlowCategoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> CreateCashFlowCategoryAsync(CashFlowCategory cashFlowCategory)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"INSERT INTO 
                                            ""Accounts.Ledger.CashFlowCategories"" 
                                            (""cashFlowCategoryName"", ""cashFlowCategoryTypeID"") 
                                    VALUES 
                                            (@cashFlowCategoryName, @cashFlowCategoryTypeID)";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@cashFlowCategoryName", cashFlowCategory.CashFlowCategoryName);
            command.Parameters.AddWithValue("@cashFlowCategoryTypeID", cashFlowCategory.CashFlowCategoryTypeID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> UpdateCashFlowCategoryAsync(CashFlowCategory cashFlowCategory)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"UPDATE 
                                    ""Accounts.Ledger.CashFlowCategories""
                                SET
                                    ""cashFlowCategoryName"" = @cashFlowCategoryName,
                                    ""cashFlowCategoryTypeID"" = @cashFlowCategoryTypeID
                                WHERE
                                    ""cashFlowCategoryID"" = @cashFlowCategoryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@cashFlowCategoryName", cashFlowCategory.CashFlowCategoryName);
            command.Parameters.AddWithValue("@cashFlowCategoryTypeID", cashFlowCategory.CashFlowCategoryTypeID);
            command.Parameters.AddWithValue("@cashFlowCategoryID", cashFlowCategory.CashFlowCategoryID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }
        public async Task<IEnumerable<CashFlowCategory>> GetActiveCashFlowCategoriesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        C.""cashFlowCategoryID"", C.""cashFlowCategoryName"", T.""cashFlowCategoryTypeName"" 
                    FROM 
                        ""Accounts.Ledger.CashFlowCategories"" C INNER JOIN ""Accounts.Ledger.CashFlowCategoryTypes"" T 
                    ON 
                        C.""cashFlowCategoryTypeID"" = T.""cashFlowCategoryTypeID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<CashFlowCategory> cashFlowCategories = new List<CashFlowCategory>();
            while (await reader.ReadAsync())
            {
                cashFlowCategories.Add(new CashFlowCategory
                {
                    CashFlowCategoryID = reader["cashFlowCategoryID"] is DBNull ? 0 : (int)reader["cashFlowCategoryID"],
                    CashFlowCategoryName = reader["cashFlowCategoryName"] is DBNull ? string.Empty : (string)reader["cashFlowCategoryName"],
                    CashFlowCategoryType = new CashFlowCategoryType { CashFlowCategoryTypeName = reader["cashFlowCategoryTypeName"] is DBNull ? string.Empty : (string)reader["cashFlowCategoryTypeName"] }
                });
            }
            return cashFlowCategories;
        }
        public async Task<CashFlowCategory?> GetCashFlowCategoryDetailsAsync(int cashFlowCategoryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                        C.""cashFlowCategoryID"", C.""cashFlowCategoryName"",
                                        C.""cashFlowCategoryTypeID"", T.""cashFlowCategoryTypeName""
                                    FROM 
                                        ""Accounts.Ledger.CashFlowCategories"" C INNER JOIN ""Accounts.Ledger.CashFlowCategoryTypes"" T
                                    ON 
                                        C.""cashFlowCategoryTypeID"" = T.""cashFlowCategoryTypeID""
                                    WHERE 
                                        C.""cashFlowCategoryID"" = @cashFlowCategoryID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@cashFlowCategoryID", cashFlowCategoryID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new CashFlowCategory
                {
                    CashFlowCategoryID = reader["cashFlowCategoryID"] is DBNull ? 0 : (int)reader["cashFlowCategoryID"],
                    CashFlowCategoryName = reader["cashFlowCategoryName"] is DBNull ? string.Empty : (string)reader["cashFlowCategoryName"],
                    CashFlowCategoryTypeID = reader["cashFlowCategoryTypeID"] is DBNull ? 0 : (int)reader["cashFlowCategoryTypeID"],
                    CashFlowCategoryType = new CashFlowCategoryType { CashFlowCategoryTypeID = reader["cashFlowCategoryTypeID"] is DBNull ? 0 : (int)reader["cashFlowCategoryTypeID"], CashFlowCategoryTypeName = reader["cashFlowCategoryTypeName"] is DBNull ? string.Empty : (string)reader["cashFlowCategoryTypeName"] },

                };
            }
            return null;
        }

        public async Task<bool> DeleteCashFlowCategoryAsync(int cashFlowCategoryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Accounts.Ledger.CashFlowCategories""
                                WHERE 
                                    ""cashFlowCategoryID"" = @cashFlowCategoryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@cashFlowCategoryID", cashFlowCategoryID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> DoesCashFlowCategoryExist(int cashFlowCategoryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Accounts.Ledger.CashFlowCategories""
                                    WHERE 
                                        ""cashFlowCategoryID"" = @cashFlowCategoryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@cashFlowCategoryID", cashFlowCategoryID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}
