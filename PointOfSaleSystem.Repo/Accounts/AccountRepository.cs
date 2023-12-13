using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConfiguration _configuration;
        public AccountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> CreateAccountAsync(Account account, int accountNumber)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                   INSERT INTO 
                       ""Accounts.Ledger.Accounts""
                       (""accountClassID"", ""accountNo"", ""cashFlowCategoryID"",""isLocked"", ""accountName"")
                   VALUES 
                       (@accountClassID, @accountNo, @cashFlowCategoryID, @isLocked, @accountName)";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@accountClassID", account.AccountClassID);
            command.Parameters.AddWithValue("@accountNo", accountNumber);
            command.Parameters.AddWithValue("@cashFlowCategoryID", account.CashFlowCategoryID);
            command.Parameters.AddWithValue("@accountName", account.AccountName);
            command.Parameters.AddWithValue("@isLocked", 0);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }
        public void AddAccountDetailsAsync(Account accountDetail)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string commandText = $@"
                            SELECT 
                                A.""accountName"", A.""accountNo"", A.""accountID"", AC.""className"" 
                            FROM 
                                ""Accounts.Ledger.Accounts"" A INNER JOIN ""Accounts.Ledger.AccountClasses"" AC 
                            ON 
                                A.""accountClassID"" = AC.""accountClassID"" ";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<Account> accounts = new List<Account>();

            while (await reader.ReadAsync())
            {
                Account account = new Account();
                account.AccountName = reader["accountName"] is DBNull ? string.Empty : (string)reader["accountName"];
                account.AccountNo = reader["accountNo"] is DBNull ? 0 : (int)reader["accountNo"];
                account.AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"];
                account.AccountClass = new AccountClass() { ClassName = reader["className"] is DBNull ? string.Empty : (string)reader["className"] };

                accounts.Add(account);
            }
            return accounts;
        }
        public async Task<Account?> GetAccountDetailsAsync(int accountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""accountClassID"", ""accountID"", ""accountNo"",""cashFlowCategoryID"",
                                    ""isLocked"", ""accountName""  
                                FROM 
                                    ""Accounts.Ledger.Accounts"" 
                                WHERE 
                                    ""accountID"" = @accountID ";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", accountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Account
                {
                    AccountClassID = reader["accountClassID"] is DBNull ? 0 : (int)reader["accountClassID"],
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    AccountNo = reader["accountNo"] is DBNull ? 0 : (int)reader["accountNo"],
                    CashFlowCategoryID = reader["cashFlowCategoryID"] is DBNull ? 0 : (int)reader["cashFlowCategoryID"],
                    IsLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"],
                    AccountName = reader["accountName"] is DBNull ? string.Empty : (string)reader["accountName"]
                };
            }
            return null;
        }
        public async Task<bool> UpdateAccountAsync(Account account)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"UPDATE 
                                    ""Accounts.Ledger.Accounts""
                                SET
                                    ""accountName"" = @accountName,
                                    ""accountClassID"" = @accountClassID,
                                    ""cashFlowCategoryID"" = @cashFlowCategoryID
                                WHERE
                                    ""accountID"" = @accountID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountName", account.AccountName);
            command.Parameters.AddWithValue("@accountClassID", account.AccountClassID);
            command.Parameters.AddWithValue("@cashFlowCategoryID", account.CashFlowCategoryID);
            command.Parameters.AddWithValue("@accountID", account.AccountID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }
        public async Task<bool> DeleteAccountAsync(int accountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Accounts.Ledger.Accounts"" 
                                WHERE 
                                    ""accountID""=@accountID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", accountID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }

        public async Task<bool> DoesAccountExist(int accountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Accounts.Ledger.Accounts"" 
                                    WHERE 
                                        ""accountID"" = @accountID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", accountID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
        public async Task<bool> IsAccountLockedAsync(int accountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT 
                                        ""isLocked""
                                    FROM 
                                        ""Accounts.Ledger.Accounts"" 
                                    WHERE 
                                        ""accountID"" = @accountID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", accountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            int isLocked = 0;

            if (await reader.ReadAsync())
            {
                isLocked = reader["isLocked"] is DBNull ? 0 : (int)reader["isLocked"];
            }
            return isLocked > 0;
        }
    }
}
