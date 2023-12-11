using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class JournalVoucherRepository : IJournalVoucherRepository
    {
        private readonly IConfiguration _configuration;
        public JournalVoucherRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<JournalVoucher?> CreateJournalVoucherAsync(JournalVoucher journalVoucher, int userId)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                INSERT INTO 
                        ""Accounts.JV.JournalVouchers"" 
                            (""amount"", ""description"", ""fiscalPeriodID"",
                                ""sourceReference"", ""transactionDateTime"",
                                ""isAutomatic"",  ""isPosted"",
                                    ""postedBySysUID"")
                VALUES 
                        ( @amount, @description, @fiscalPeriodID,
                        @sourceReference, @transactionDateTime,
                            @isAutomatic, @isPosted,@postedBySysUID )
                RETURNING  
                        ""journalVoucherID"", ""fiscalPeriodID"", ""transactionDateTime"",
                        ""sourceReference"", ""description"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@amount", journalVoucher.Amount);
            command.Parameters.AddWithValue("@description", journalVoucher.Description);
            command.Parameters.AddWithValue("@fiscalPeriodID", journalVoucher.FiscalPeriodID);
            command.Parameters.AddWithValue("@sourceReference", journalVoucher.SourceReference);
            command.Parameters.AddWithValue("@transactionDateTime", journalVoucher.TransactionDateTime);

            command.Parameters.AddWithValue("@isAutomatic", 0);
            command.Parameters.AddWithValue("@isPosted", 0);

            command.Parameters.AddWithValue("@postedBySysUID", userId);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new JournalVoucher
                {
                    JournalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"],
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    TransactionDateTime = reader["transactionDateTime"] is DBNull ? DateTime.MinValue : (DateTime)reader["transactionDateTime"],
                    SourceReference = reader["sourceReference"] is DBNull ? string.Empty : (string)reader["sourceReference"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"]
                };
            }
            return null;
        }
        public async Task<JournalVoucher?> UpdateJournalVoucherAsync(JournalVoucher journalVoucher)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"UPDATE 
                            ""Accounts.JV.JournalVouchers"" 
                        SET
                            ""sourceReference"" = @sourceReference,
                            ""description"" = @description
                        WHERE
                            ""journalVoucherID"" = @journalVoucherID
                        RETURNING
                            ""journalVoucherID"", ""sourceReference"", ""description""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@sourceReference", journalVoucher.SourceReference);
            command.Parameters.AddWithValue("@description", journalVoucher.Description);
            command.Parameters.AddWithValue("@journalVoucherID", journalVoucher.JournalVoucherID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new JournalVoucher
                {
                    JournalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"],
                    SourceReference = reader["sourceReference"] is DBNull ? string.Empty : (string)reader["sourceReference"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"]
                };
            }

            return null;
        }

        public async Task<JournalVoucher?> GetJournalVoucherDetailsAsync(int journalVoucherID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var commandText = $@"
            SELECT
                JV.""amount"", JV.""description"", JV.""fiscalPeriodID"", JV.""isAutomatic"",
                JV.""isPosted"", JV.""journalVoucherID"", 
                JV.""postedBySysUID"", JV.""sourceReference"", JV.""transactionDateTime"", SU.""userName""
            FROM 
                ""Accounts.JV.JournalVouchers"" JV INNER JOIN ""Security.SystemUsers"" SU
            ON   
                JV.""postedBySysUID""   = SU.""sysUserID""
            WHERE 
                ""journalVoucherID"" = @journalVoucherID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new JournalVoucher
                {
                    Amount = reader["amount"] is DBNull ? 0 : (double)reader["amount"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    IsAutomatic = reader["isAutomatic"] is DBNull ? 0 : (int)reader["isAutomatic"],
                    IsPosted = reader["isPosted"] is DBNull ? 0 : (int)reader["isPosted"],
                    JournalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"],
                    PostedBySyUID = reader["postedBySysUID"] is DBNull ? 0 : (int)reader["postedBySysUID"],
                    SourceReference = reader["sourceReference"] is DBNull ? string.Empty : (string)reader["sourceReference"],
                    TransactionDateTime = reader["transactionDateTime"] is DBNull ? DateTime.MinValue : (DateTime)reader["transactionDateTime"]
                };
            }
            return null;
        }
        public async Task<IEnumerable<JournalVoucher>> FilterJournalVouchersAsync(FilterJournalVoucher filterJournalVoucher, int isAutomatic, int isPosted)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            DateTime dateTo = filterJournalVoucher.DateTo;
            dateTo = dateTo.Date.AddDays(1).AddSeconds(-1);

            var commandText = $@"SELECT 
                                    ""amount"", ""description"", ""fiscalPeriodID"", ""isAutomatic"", 
                                    ""isPosted"", ""journalVoucherID"",  
                                    ""postedBySysUID"", ""sourceReference"", ""transactionDateTime"" 
                                FROM 
                                    ""Accounts.JV.JournalVouchers"" 
                                WHERE 
                                        (""isAutomatic"" = @isAutomatic OR @isAutomatic = 2) 
                                        AND 
                                        (""isPosted"" = @isPosted OR @isPosted = 2) 
                                        AND 
                                        ""transactionDateTime"" BETWEEN @DateFrom AND @DateTo";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@isAutomatic", isAutomatic);
            command.Parameters.AddWithValue("@isPosted", isPosted);
            command.Parameters.AddWithValue("@DateFrom", filterJournalVoucher.DateFrom);
            command.Parameters.AddWithValue("@DateTo", dateTo);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<JournalVoucher> journalVouchers = new List<JournalVoucher>();

            while (await reader.ReadAsync())
            {
                journalVouchers.Add(new JournalVoucher
                {
                    Amount = reader["amount"] is DBNull ? 0 : (double)reader["amount"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    FiscalPeriodID = reader["fiscalPeriodID"] is DBNull ? 0 : (int)reader["fiscalPeriodID"],
                    IsAutomatic = reader["isAutomatic"] is DBNull ? 0 : (int)reader["isAutomatic"],
                    IsPosted = reader["isPosted"] is DBNull ? 0 : (int)reader["isPosted"],
                    JournalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"],
                    SourceReference = reader["sourceReference"] is DBNull ? string.Empty : (string)reader["sourceReference"],
                    TransactionDateTime = reader["transactionDateTime"] is DBNull ? DateTime.MinValue : (DateTime)reader["transactionDateTime"]
                });
            }
            return journalVouchers;
        }
        public async Task<bool> IsJournalVoucherPostedAsync(int journalVoucherID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var commandText = $@"SELECT 
                                        ""isPosted""
                                    FROM 
                                        ""Accounts.JV.JournalVouchers"" 
                                    WHERE 
                                        ""journalVoucherID"" = @journalVoucherID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            int isPosted = 0;
            if (await reader.ReadAsync())
            {
                isPosted = reader["isposted"] is DBNull ? 0 : (int)reader["isposted"];
            }
            return isPosted > 0;
        }
        public async Task<bool> IsJournalVoucherAutomaticallyPostedAsync(int journalVoucherID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var commandText = $@"SELECT 
                                        ""isAutomatic""
                                    FROM 
                                        ""Accounts.JV.JournalVouchers"" 
                                    WHERE 
                                        ""journalVoucherID"" = @journalVoucherID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            int isAutomatic = 0;
            if (await reader.ReadAsync())
            {
                isAutomatic = reader["isAutomatic"] is DBNull ? 0 : (int)reader["isAutomatic"];
            }
            return isAutomatic > 0;
        }
        private async Task<(int, int)> GetAccountTypesAsync(NpgsqlConnection connection, int accountEntryID)
        {
            string commandText2 = @"
                    SELECT
                        AC_credit.""accountTypeID"" AS creditAccountTypeID,
                        AC_debit.""accountTypeID"" AS debitAccountTypeID
                    FROM ""Accounts.JV.AccountEntries"" AE
                        JOIN ""Accounts.Ledger.SubAccounts"" SA_credit ON AE.""creditSubAccountID"" = SA_credit.""subAccountID""
                        JOIN ""Accounts.Ledger.SubAccounts"" SA_debit ON AE.""debitSubAccountID"" = SA_debit.""subAccountID""
                        JOIN ""Accounts.Ledger.Accounts"" A_credit ON SA_credit.""accountID"" = A_credit.""accountID""
                        JOIN ""Accounts.Ledger.Accounts"" A_debit ON SA_debit.""accountID"" = A_debit.""accountID""
                        JOIN ""Accounts.Ledger.AccountClasses"" AC_credit ON A_credit.""accountClassID"" = AC_credit.""accountClassID""
                        JOIN ""Accounts.Ledger.AccountClasses"" AC_debit ON A_debit.""accountClassID"" = AC_debit.""accountClassID""
                    WHERE AE.""accountEntryID"" = @accountEntryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText2, connection);

            command.Parameters.AddWithValue("@accountEntryID", accountEntryID);

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int creditAccountTypeID = reader["creditAccountTypeID"] is DBNull ? 0 : (int)reader["creditAccountTypeID"];
                int debitAccountTypeID = reader["debitAccountTypeID"] is DBNull ? 0 : (int)reader["debitAccountTypeID"];
                return (creditAccountTypeID, debitAccountTypeID);
            }
            return (0, 0);
        }
        public async Task<bool> PostJournalVoucherAsync(IEnumerable<JournalVoucherEntry> journalVoucherEntry)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();
            using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
            try
            {
                foreach (var item in journalVoucherEntry)
                {
                    var (creditAccountTypeID, debitAccountTypeID) = await GetAccountTypesAsync(connection, item.JournalVoucherEntryID);

                    string commandText = null;

                    //Debit
                    if (debitAccountTypeID == 1 || debitAccountTypeID == 5)//asset and expence increace recorded ans debit
                    {
                        commandText = $@"UPDATE 
                                        ""Accounts.Ledger.SubAccounts"" 
                                    SET  
                                        ""currentBalance"" = ""currentBalance"" + @debitAmount
                                    WHERE 
                                        ""subAccountID"" = @debitSubAccountID";
                    }
                    else
                    {
                        commandText = $@"UPDATE 
                                        ""Accounts.Ledger.SubAccounts"" 
                                    SET  
                                        ""currentBalance"" = ""currentBalance"" - @debitAmount
                                    WHERE 
                                        ""subAccountID"" = @debitSubAccountID";
                    }
                    using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@debitAmount", item.DebitAmount);
                        command.Parameters.AddWithValue("@debitSubAccountID", item.DebitSubAccount.SubAccountID);
                        await command.ExecuteNonQueryAsync();
                    }
                    //Credit
                    if (creditAccountTypeID == 1 || creditAccountTypeID == 5)//Asset and expence decreace
                    {
                        commandText = $@"UPDATE 
                                        ""Accounts.Ledger.SubAccounts"" 
                                    SET  
                                        ""currentBalance"" = ""currentBalance"" - @creditAmount
                                    WHERE 
                                        ""subAccountID"" = @creditSubAccountID";
                    }
                    else
                    {
                        commandText = $@"UPDATE 
                                        ""Accounts.Ledger.SubAccounts"" 
                                    SET  
                                        ""currentBalance"" = ""currentBalance"" + @creditAmount
                                    WHERE 
                                        ""subAccountID"" = @creditSubAccountID";
                    }

                    using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@creditAmount", item.CreditAmount);
                        command.Parameters.AddWithValue("@creditSubAccountID", item.CreditSubAccount.SubAccountID);

                        await command.ExecuteNonQueryAsync();
                    }

                    //Update Entry "isPosted" status
                    commandText = $@"UPDATE 
                                        ""Accounts.JV.JournalVouchers"" 
                                    SET  
                                        ""isPosted"" = @isPosted
                                    WHERE 
                                        ""journalVoucherID"" = @journalVoucherID";

                    using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isPosted", 1);
                        command.Parameters.AddWithValue("@journalVoucherID", item.JournalVoucherID);

                        await command.ExecuteNonQueryAsync();
                    }

                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UnPostJournalVoucherAsync(IEnumerable<JournalVoucherEntry> journalVoucherEntry)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
                try
                {
                    int count = 0;

                    foreach (var item in journalVoucherEntry)
                    {
                        var (creditAccountTypeID, debitAccountTypeID) = await GetAccountTypesAsync(connection, item.JournalVoucherEntryID);

                        string commandText = null;

                        //Debit
                        if (debitAccountTypeID == 1 || debitAccountTypeID == 5)//asset and expence increace recorded ans debit
                        {
                            commandText = $@"UPDATE 
                                          ""Accounts.Ledger.SubAccounts"" 
                                      SET  
                                          ""currentBalance"" = ""currentBalance"" - @debitAmount
                                      WHERE 
                                         ""subAccountID"" = @debitSubAccountID";
                        }
                        else
                        {
                            commandText = $@"UPDATE 
                                          ""Accounts.Ledger.SubAccounts"" 
                                      SET  
                                          ""currentBalance"" = ""currentBalance"" + @debitAmount
                                      WHERE 
                                         ""subAccountID"" = @debitSubAccountID";
                        }
                        using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@debitAmount", item.DebitAmount);
                            command.Parameters.AddWithValue("@debitSubAccountID", item.DebitSubAccount.SubAccountID);

                            count = await command.ExecuteNonQueryAsync();
                        }

                        //Credit
                        if (creditAccountTypeID == 1 || creditAccountTypeID == 5)//Asset and expence decreace
                        {
                            commandText = $@"UPDATE 
                                          ""Accounts.Ledger.SubAccounts"" 
                                      SET  
                                          ""currentBalance"" = ""currentBalance"" + @creditAmount
                                      WHERE 
                                         ""subAccountID"" = @creditSubAccountID";
                        }
                        else
                        {
                            commandText = $@"UPDATE 
                                          ""Accounts.Ledger.SubAccounts"" 
                                      SET  
                                          ""currentBalance"" = ""currentBalance"" - @creditAmount
                                      WHERE 
                                         ""subAccountID"" = @creditSubAccountID";
                        }

                        using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@creditAmount", item.CreditAmount);
                            command.Parameters.AddWithValue("@creditSubAccountID", item.CreditSubAccount.SubAccountID);

                            await command.ExecuteNonQueryAsync();
                        }

                        //Update Entry "isPosted" status
                        commandText = $@"UPDATE 
                                          ""Accounts.JV.JournalVouchers"" 
                                      SET  
                                          ""isPosted"" = @isPosted
                                      WHERE 
                                         ""journalVoucherID"" = @journalVoucherID";

                        using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
                        {
                            command.Parameters.AddWithValue("@isPosted", 0);
                            command.Parameters.AddWithValue("@journalVoucherID", item.JournalVoucherID);

                            count = await command.ExecuteNonQueryAsync();
                        }
                    }
                    await transaction.CommitAsync();
                    return count != 0 ? true : false;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteJournalVoucherAsync(int journalVoucherID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM
                                        ""Accounts.JV.JournalVouchers"" 
                                    WHERE 
                                        ""journalVoucherID""=@journalVoucherID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> DoesJournalVoucherExist(int journalVoucherID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Accounts.JV.JournalVouchers""
                                    WHERE 
                                        ""journalVoucherID"" = @journalVoucherID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}
