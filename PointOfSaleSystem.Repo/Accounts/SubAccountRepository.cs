using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class SubAccountRepository : ISubAccountRepository
    {
        private readonly IConfiguration _configuration;
        public SubAccountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<SubAccount>> GetAllSubAccountsByAccountIDAsync(int accountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT
                                        ""accountID"", ""currentBalance"", ""isActive"", ""isLocked"", 
                                        ""subAccountName"", ""subAccountID""  
                                    FROM 
                                        ""Accounts.Ledger.SubAccounts""
                                    WHERE 
                                        ""accountID"" = @accountID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", accountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<SubAccount> accountSubAccounts = new List<SubAccount>();

            while (await reader.ReadAsync())
            {
                accountSubAccounts.Add(new SubAccount
                {
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"],
                    SubAccountName = reader["subAccountName"] is DBNull ? string.Empty : (string)reader["subAccountName"],
                    SubAccountID = reader["subAccountID"] is DBNull ? 0 : (int)reader["subAccountID"]
                });
            }
            return accountSubAccounts;
        }

        public async Task<bool> CreateSubAccountAsync(SubAccount subAccount)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    INSERT INTO 
                            ""Accounts.Ledger.SubAccounts"" 
                            (""accountID"", ""currentBalance"", ""subAccountName"",""isActive"", ""isLocked"")
                    VALUES 
                            (@accountID, @currentBalance, @subAccountName,@isActive, @isLocked)";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", subAccount.AccountID);
            command.Parameters.AddWithValue("@currentBalance", subAccount.CurrentBalance);
            command.Parameters.AddWithValue("@subAccountName", subAccount.SubAccountName);
            command.Parameters.AddWithValue("@isActive", 1);
            command.Parameters.AddWithValue("@isLocked", 0);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }
        public async Task<IEnumerable<SubAccount>> GetAllSubAccountsAsync(int accountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT
                                        ""accountID"", ""currentBalance"", ""isActive"", ""isLocked"", 
                                        ""subAccountName"", ""subAccountID""  
                                    FROM 
                                        ""Accounts.Ledger.SubAccounts""
                                    WHERE 
                                        ""accountID"" = @accountID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", accountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<SubAccount> accountSubAccounts = new List<SubAccount>();

            while (await reader.ReadAsync())
            {
                accountSubAccounts.Add(new SubAccount
                {
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"],
                    SubAccountName = reader["subAccountName"] is DBNull ? string.Empty : (string)reader["subAccountName"],
                    SubAccountID = reader["subAccountID"] is DBNull ? 0 : (int)reader["subAccountID"]
                });
            }
            return accountSubAccounts;
        }
        public async Task<SubAccount?> GetSubAccountDetailsAsync(int subAccountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                    ""accountID"", ""currentBalance"", ""isActive"", ""isLocked"", 
                                    ""subAccountName"", ""subAccountID""
                                    FROM 
                                    ""Accounts.Ledger.SubAccounts"" 
                                    WHERE 
                                    ""subAccountID"" = @accountID ";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", subAccountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new SubAccount
                {
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    CurrentBalance = reader["currentBalance"] is DBNull ? 0 : (double)reader["currentBalance"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    IsLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"],
                    SubAccountName = reader["subAccountName"] is DBNull ? string.Empty : (string)reader["subAccountName"],
                    SubAccountID = reader["subAccountID"] is DBNull ? 0 : (int)reader["subAccountID"]
                };
            }
            return null;
        }
        public async Task<bool> UpdateSubAccountAsync(SubAccount subAccount)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"UPDATE 
                                    ""Accounts.Ledger.SubAccounts""
                                SET 
                                    ""subAccountName"" = @subAccountName 
                                WHERE 
                                    ""subAccountID"" = @subAccountID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@subAccountName", subAccount.SubAccountName);
            command.Parameters.AddWithValue("@subAccountID", subAccount.SubAccountID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }

        public async Task<bool> DeleteSubAccountAsync(int subAccountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Accounts.Ledger.SubAccounts"" 
                                WHERE 
                                    ""subAccountID""=@subAccountID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@subAccountID", subAccountID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }

        public async Task<double> GetSourceSubAccountBalanceAsync(TransferSubAccountBalance sourceSubAccount)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                        ""currentBalance""
                                    FROM 
                                        ""Accounts.Ledger.SubAccounts""
                                    WHERE 
                                        ""subAccountID"" = @subAccountID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@subAccountID", sourceSubAccount.SourceSubAccountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader["currentBalance"] is DBNull ? 0 : (double)reader["currentBalance"];
            }
            return 0;
        }  
        public async Task<double> GetSourceSubAccountBalanceAsync(int sourceSubAccountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                        ""currentBalance""
                                    FROM 
                                        ""Accounts.Ledger.SubAccounts""
                                    WHERE 
                                        ""subAccountID"" = @subAccountID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@subAccountID", sourceSubAccountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader["currentBalance"] is DBNull ? 0 : (double)reader["currentBalance"];
            }
            return 0;
        }

        public async Task<bool> TransferSubAccountBalanceAsync(TransferSubAccountBalance destSubAccountBalance, double sourceSubAccountBalance)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
            try
            {
                string commandText = $@"
                            UPDATE 
                                ""Accounts.Ledger.SubAccounts"" 
                            SET 
                                ""currentBalance"" = ""currentBalance"" - @sourceSubAccountBalance
                            WHERE 
                                ""subAccountID"" = @sourceSubAccountID";
                using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                {
                    command.Parameters.AddWithValue("@sourceSubAccountBalance", sourceSubAccountBalance);
                    command.Parameters.AddWithValue("@sourceSubAccountID", destSubAccountBalance.SourceSubAccountID);

                    await command.ExecuteNonQueryAsync();
                }

                commandText = $@"
                            UPDATE 
                                ""Accounts.Ledger.SubAccounts"" 
                            SET 
                                ""currentBalance"" = ""currentBalance"" + @sourceSubAccountBalance
                            WHERE 
                                ""subAccountID"" = @destSubAccountID";
                int rowsAffected = 0;
                using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                {
                    command.Parameters.AddWithValue("@sourceSubAccountBalance", sourceSubAccountBalance);
                    command.Parameters.AddWithValue("@destSubAccountID", destSubAccountBalance.DestSubAccountID);

                    rowsAffected = await command.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();

                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<SubAccount>> GetInventorySubAccountAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""accountID"", ""currentBalance"", ""isLocked"", ""subAccountName"", ""subAccountID""
                    FROM 
                            ""Accounts.Ledger.SubAccounts""
                    WHERE
                        ""accountID"" = @accountID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", 1);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<SubAccount> subAccounts = new List<SubAccount>();

            while (await reader.ReadAsync())
            {
                subAccounts.Add(new SubAccount
                {
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    CurrentBalance = reader["currentBalance"] is DBNull ? 0 : (double)reader["currentBalance"],
                    IsLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"],
                    SubAccountName = reader["subAccountName"] is DBNull ? string.Empty : (string)reader["subAccountName"],
                    SubAccountID = reader["subAccountID"] is DBNull ? 0 : (int)reader["subAccountID"]
                });
            }
            return subAccounts;
        }

        public async Task<IEnumerable<SubAccount>> GetCostOfSalesSubAccountsAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""accountID"", ""currentBalance"", ""isLocked"", ""subAccountName"", ""subAccountID""
                    FROM 
                            ""Accounts.Ledger.SubAccounts""
                    WHERE
                        ""accountID"" = @accountID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", 2);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<SubAccount> subAccounts = new List<SubAccount>();

            while (await reader.ReadAsync())
            {
                subAccounts.Add(new SubAccount
                {
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    CurrentBalance = reader["currentBalance"] is DBNull ? 0 : (double)reader["currentBalance"],
                    IsLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"],
                    SubAccountName = reader["subAccountName"] is DBNull ? string.Empty : (string)reader["subAccountName"],
                    SubAccountID = reader["subAccountID"] is DBNull ? 0 : (int)reader["subAccountID"]
                });
            }
            return subAccounts;
        }

        public async Task<IEnumerable<SubAccount>> GetIncomeSubAccountsAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""accountID"", ""currentBalance"", ""isLocked"", ""subAccountName"", ""subAccountID""
                    FROM 
                            ""Accounts.Ledger.SubAccounts""
                    WHERE
                        ""accountID"" = @accountID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", 3);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<SubAccount> subAccounts = new List<SubAccount>();

            while (await reader.ReadAsync())
            {
                subAccounts.Add(new SubAccount
                {
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    CurrentBalance = reader["currentBalance"] is DBNull ? 0 : (double)reader["currentBalance"],
                    IsLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"],
                    SubAccountName = reader["subAccountName"] is DBNull ? string.Empty : (string)reader["subAccountName"],
                    SubAccountID = reader["subAccountID"] is DBNull ? 0 : (int)reader["subAccountID"]
                });
            }
            return subAccounts;

        }

        public async Task<bool> DoesSubAccountExist(int subAccountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Accounts.Ledger.SubAccounts"" 
                                    WHERE 
                                        ""subAccountID"" = @subAccountID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@subAccountID", subAccountID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}
