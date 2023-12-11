using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class AccountClassRepository : IAccountClassRepository
    {
        private readonly IConfiguration _configuration;
        public AccountClassRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> CreateAccountClassAsync(AccountClass accountClass)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"INSERT INTO 
                                    ""Accounts.Ledger.AccountClasses"" 
                                        (""className"", ""accountTypeID"")
                                VALUES 
                                    (@className, @accountTypeID)";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@className", accountClass.ClassName);
            command.Parameters.AddWithValue("@accountTypeID", accountClass.AccountTypeID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }

        public async Task<Account?> GetAccountDetailsAndAccountClassNameAsync(Account account)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                        A.""accountID"", A.""accountNo"", A.""accountName"", AC.""className"" 
                                    FROM 
                                        ""Accounts.Ledger.Accounts"" A INNER JOIN ""Accounts.Ledger.AccountClasses"" AC 
                                    ON 
                                        A.""accountClassID"" = AC.""accountClassID""
                                    WHERE 
                                        A.""accountName"" = @accountName";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountName", account.AccountName);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Account
                {
                    AccountID = reader["accountID"] is DBNull ? 0 : (int)reader["accountID"],
                    AccountNo = reader["accountNo"] is DBNull ? 0 : (int)reader["accountNo"],
                    AccountName = reader["accountName"] is DBNull ? string.Empty : (string)reader["accountName"],
                    AccountClass = new AccountClass { ClassName = reader["className"] is DBNull ? string.Empty : (string)reader["className"] }
                };
            }
            return null;
        }
        public async Task<IEnumerable<AccountClass>> GetAllAccountClassesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                        ""accountClassID"",""accountTypeID"" , ""className""
                                    FROM 
                                        ""Accounts.Ledger.AccountClasses"" ";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<AccountClass> accountClasses = new List<AccountClass>();

            while (await reader.ReadAsync())
            {
                accountClasses.Add(new AccountClass
                {
                    AccountClassID = reader["accountClassID"] is DBNull ? 0 : (int)reader["accountClassID"],
                    AccountTypeID = reader["accountTypeID"] is DBNull ? 0 : (int)reader["accountTypeID"],
                    ClassName = reader["className"] is DBNull ? string.Empty : (string)reader["className"]
                });
            }
            return accountClasses;
        }

        public async Task<int?> GetAccountTypeIdAsync(int accountClassID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                        ""accountTypeID""  
                                    FROM 
                                        ""Accounts.Ledger.AccountClasses"" 
                                    WHERE 
                                        ""accountClassID"" = @accountClassID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountClassID", accountClassID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader["accountTypeID"] is DBNull ? 0 : (int)reader["accountTypeID"];
            }
            return null;
        }
    }
}
