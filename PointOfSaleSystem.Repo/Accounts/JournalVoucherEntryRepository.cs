using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class JournalVoucherEntryRepository : IJournalVoucherEntryRepository
    {
        private readonly IConfiguration _configuration;
        public JournalVoucherEntryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> CreateEntryAsync(JournalVoucherEntry journalVoucherEntry)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                        INSERT INTO 
                            ""Accounts.JV.AccountEntries""
                            (""creditAmount"", ""debitAmount"",  ""creditSubAccountID"",
                                ""debitSubAccountID"", ""journalVoucherID"") 
                        VALUES 
                            (@creditAmount, @debitAmount, @creditSubAccountID,
                                @debitSubAccountID, @journalVoucherID)";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@creditAmount", journalVoucherEntry.CreditAmount);
            command.Parameters.AddWithValue("@debitAmount", journalVoucherEntry.DebitAmount);
            command.Parameters.AddWithValue("@creditSubAccountID", journalVoucherEntry.CreditSubAccountID);
            command.Parameters.AddWithValue("@debitSubAccountID", journalVoucherEntry.DebitSubAccountID);
            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherEntry.JournalVoucherID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }
        public async Task<IEnumerable<JournalVoucherEntry>> GetJournalVoucherEntriesAsync(int journalVoucherID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var commandText = $@"
                        SELECT 
                            E.""accountEntryID"", E.""creditAmount"", E.""journalVoucherID"",
                            E.""creditSubAccountID"", E.""debitSubAccountID"",
                            S.""subAccountID"" ,
                            S.""subAccountName"" AS ""creditSubAccount"",
                            D.""subAccountName"" AS ""debitSubAccount"" 
                        FROM 
                            ""Accounts.JV.AccountEntries"" E INNER JOIN ""Accounts.Ledger.SubAccounts"" S 
                        ON 
                            E.""creditSubAccountID"" = S.""subAccountID"" INNER JOIN ""Accounts.Ledger.SubAccounts"" D 
                        ON 
                            E.""debitSubAccountID"" = D.""subAccountID"" 
                        WHERE 
                            ""journalVoucherID"" = @accountID ORDER BY ""accountEntryID"" ASC";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountID", journalVoucherID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<JournalVoucherEntry> journalVoucherEntries = new List<JournalVoucherEntry>();

            while (await reader.ReadAsync())
            {
                JournalVoucherEntry accountEntry = new JournalVoucherEntry();

                accountEntry.JournalVoucherEntryID = reader["accountEntryID"] is DBNull ? 0 : (int)reader["accountEntryID"];
                accountEntry.CreditAmount = reader["creditAmount"] is DBNull ? 0 : (double)reader["creditAmount"];
                accountEntry.JournalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"];
                accountEntry.CreditSubAccount = new SubAccount { SubAccountName = reader["creditSubAccount"] is DBNull ? string.Empty : (string)reader["creditSubAccount"] };
                accountEntry.DebitSubAccount = new SubAccount { SubAccountName = reader["debitSubAccount"] is DBNull ? string.Empty : (string)reader["debitSubAccount"] };

                journalVoucherEntries.Add(accountEntry);
            }
            return journalVoucherEntries;
        }
        public async Task<JournalVoucherEntry?> GetJournalVoucherEntryDetailsAsync(int accountID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var commandText = $@"
                            SELECT  
                                E.""accountEntryID"", E.""creditAmount"", E.""debitAmount"",  E.""journalVoucherID"",
                                E.""debitSubAccountID"" ,E.""creditSubAccountID"",D.""accountID"" AS ""debitAccountID"",S.""accountID"" AS ""creditAccountID""
                            FROM 
                                ""Accounts.JV.AccountEntries"" E INNER JOIN ""Accounts.Ledger.SubAccounts"" S
                            ON
	                            E.""creditSubAccountID""  = S.""subAccountID"" INNER JOIN ""Accounts.Ledger.SubAccounts"" D 
                            ON 
                                E.""debitSubAccountID"" = D.""subAccountID""
                            WHERE
	                            E.""accountEntryID"" = @accountEntryID    ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountEntryID", accountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new JournalVoucherEntry
                {

                    CreditAccountID = reader["creditAccountID"] is DBNull ? 0 : (int)reader["creditAccountID"],
                    DebitAccountID = reader["debitAccountID"] is DBNull ? 0 : (int)reader["debitAccountID"],
                    JournalVoucherEntryID = reader["accountEntryID"] is DBNull ? 0 : (int)reader["accountEntryID"],
                    CreditAmount = reader["creditAmount"] is DBNull ? 0 : (float)(double)reader["creditAmount"],
                    CreditSubAccountID = reader["creditSubAccountID"] is DBNull ? 0 : (int)reader["creditSubAccountID"],
                    DebitAmount = reader["debitAmount"] is DBNull ? 0 : (double)reader["debitAmount"],
                    DebitSubAccountID = reader["debitSubAccountID"] is DBNull ? 0 : (int)reader["debitSubAccountID"],
                    JournalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"]
                };
            }
            return null;
        }

        public async ValueTask<bool> IsEntryPostedAsync(int accountEntryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var commandText = $@"SELECT 
                                        J.""isPosted""
                                    FROM 
                                        ""Accounts.JV.AccountEntries"" E INNER JOIN ""Accounts.JV.JournalVouchers"" J
                                    ON
                                        E.""journalVoucherID"" = J.""journalVoucherID""
                                    WHERE 
                                        E.""accountEntryID"" = @entryID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@entryID", accountEntryID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            int isPosted = 0;

            if (await reader.ReadAsync())
            {
                isPosted = reader["isposted"] is DBNull ? 0 : (int)reader["isposted"];
            }
            return isPosted > 0;
        }
        public async Task<bool> UpdateEntryAsync(JournalVoucherEntry journalVoucherEntry)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"UPDATE 
                                        ""Accounts.JV.AccountEntries"" 
                                    SET  
                                        ""creditAmount"" = @creditAmount, 
                                        ""debitAmount"" = @debitAmount,  
                                        ""creditSubAccountID"" = @creditSubAccountID,
                                        ""debitSubAccountID"" = @debitSubAccountID, 
                                        ""journalVoucherID"" = @journalVoucherID 
                                    WHERE 
                                        ""accountEntryID"" = @accountEntryID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@creditAmount", journalVoucherEntry.CreditAmount);
            command.Parameters.AddWithValue("@debitAmount", journalVoucherEntry.DebitAmount);
            command.Parameters.AddWithValue("@creditSubAccountID", journalVoucherEntry.CreditSubAccountID);
            command.Parameters.AddWithValue("@debitSubAccountID", journalVoucherEntry.DebitSubAccountID);
            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherEntry.JournalVoucherID);
            command.Parameters.AddWithValue("@accountEntryID", journalVoucherEntry.JournalVoucherEntryID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }
        public async Task<bool> DeleteEntryAsync(int accountEntryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"DELETE FROM 
                                    ""Accounts.JV.AccountEntries"" 
                                WHERE 
                                    ""accountEntryID""=@accountEntryID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountEntryID", accountEntryID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<IEnumerable<JournalVoucherEntry>> GetJournalVoucherEntriesDetailsAsync(int journalVoucherID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var commandText = $@"
                        SELECT 
                            ""debitAmount"" , ""creditAmount"", ""debitSubAccountID"",
                            ""accountEntryID"", ""creditSubAccountID"", ""journalVoucherID""
                        FROM 
                            ""Accounts.JV.AccountEntries"" 
                        WHERE 
                            ""journalVoucherID"" = @journalVoucherID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<JournalVoucherEntry> journalVoucherEntries = new List<JournalVoucherEntry>();

            while (await reader.ReadAsync())
            {
                journalVoucherEntries.Add(new JournalVoucherEntry
                {
                    DebitSubAccount = new SubAccount { SubAccountID = reader["debitSubAccountID"] is DBNull ? 0 : (int)reader["debitSubAccountID"] },
                    CreditSubAccount = new SubAccount { SubAccountID = reader["creditSubAccountID"] is DBNull ? 0 : (int)reader["creditSubAccountID"] },
                    JournalVoucherEntryID = reader["accountEntryID"] is DBNull ? 0 : (int)reader["accountEntryID"],
                    DebitAmount = reader["debitAmount"] is DBNull ? 0 : (double)reader["debitAmount"],
                    CreditAmount = reader["creditAmount"] is DBNull ? 0 : (double)reader["creditAmount"],
                    JournalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"]
                });
            }
            return journalVoucherEntries;
        }


        public async Task<bool> DoesAccountEntryExist(int accountEntryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Accounts.JV.AccountEntries""
                                    WHERE 
                                        ""accountEntryID"" = @accountEntryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@accountEntryID", accountEntryID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}