using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Data.Sales;
using PointOfSaleSystem.Service.Interfaces.Sales;

namespace PointOfSaleSystem.Repo.Sales
{
    public class PaymentRepository : IPaymentRepository
    {
        private IConfiguration _configuration;
        public PaymentRepository(IConfiguration config)
        {
            _configuration = config;
        }
        public async Task<IEnumerable<PaymentMethod>> GetAllPaymentModesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""paymentMethodName"", ""paymentMethodID"", ""isDefault""
                                FROM 
                                    ""Sales.POS.PaymentMethod""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();

            while (await reader.ReadAsync())
            {
                paymentMethods.Add(new PaymentMethod
                {
                    PaymentMethodName = reader["paymentMethodName"] is DBNull ? string.Empty : (string)reader["paymentMethodName"],
                    PaymentMethodID = reader["paymentMethodID"] is DBNull ? 0 : (int)reader["paymentMethodID"],
                    IsDefault = reader["isDefault"] is DBNull ? 0 : (int)reader["isDefault"]
                });
            }
            return paymentMethods;
        }

        public async Task<bool> ReceivePosPaymentsAsync(Payment payment, IEnumerable<OrderedItem> orderItemsProductQuantity, int userID, int fiscalPeriodID)//TODO: Refactor this method
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
                try
                {
                    string commandText = "";
                    (int billNo, int receiptNo) = (0, 0);
                    DateTime dateTimeBilled = DateTime.Now;

                    for (int i = 0; i < payment.PosPaymentItems.Length; i++)
                    {
                        commandText = $@"
                                    INSERT INTO 
                                            ""Sales.POS.CustomerOrderPaymentMethod""
                                        (""customerOrderID"", ""paymentMethodID"", ""amountPaid"",""amountTendered"", ""changeAmount"", ""dateTimeBilled"")
                                    VALUES
                                        (@customerOrderID, @paymentMethodID, @amountPaid, @amountTendered, @changeAmount, @dateTimeBilled)";

                        using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@dateTimeBilled", dateTimeBilled);
                            command.Parameters.AddWithValue("@customerOrderID", payment.CustomerOrderID);
                            command.Parameters.AddWithValue("@paymentMethodID", payment.PosPaymentItems[i].PaymentMethodID);
                            command.Parameters.AddWithValue("@amountTendered", payment.PosPaymentItems[i].AmountTendered);
                            command.Parameters.AddWithValue("@changeAmount", payment.PosPaymentItems[i].ChangeAmount);

                            if (payment.PosPaymentItems.Length == 1)
                            {
                                command.Parameters.AddWithValue("@amountPaid", payment.PosPaymentItems[i].AmountDue);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@amountPaid", payment.PosPaymentItems[i].AmountTendered);
                            }

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    //mark order as paid
                    commandText = $@"UPDATE 
                                         ""Inventory.Inventory.CustomerOrders""
                                     SET
                                         ""isOrderPaid"" = @isOrderPaid                                         
                                      WHERE
                                          ""customerOrderID"" = @customerOrderID
                                      RETURNING
                                            ""billNo"", ""receiptNo"" ";

                    using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@isOrderPaid", 1);
                        command.Parameters.AddWithValue("@customerOrderID", payment.CustomerOrderID);

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                billNo = reader["billNo"] is DBNull ? 0 : (int)reader["billNo"];
                                receiptNo = reader["receiptNo"] is DBNull ? 0 : (int)reader["receiptNo"];
                            }
                        }
                    }

                    List<int> quantity = new List<int>();
                    List<int> itemID = new List<int>();

                    List<double> unitCost = new List<double>();
                    List<double> unitPriceList = new List<double>();

                    List<int> assetSubAccountIDList = new List<int>();
                    List<int> costOfSaleSubAccountIDList = new List<int>();
                    List<int> revenueSubAccountIDList = new List<int>();

                    foreach (var item in orderItemsProductQuantity)
                    {
                        quantity.Add(item.Quantity);
                        itemID.Add(item.ItemID);

                        commandText = @"
                        UPDATE ""Inventory.Inventory.Items""
                        SET
                            ""availableQuantity"" = ""availableQuantity"" - @quantity
                        WHERE
                            ""itemID"" = @itemID
                        RETURNING 
                        ""unitCost"", ""unitPrice"", ""assetSubAccountID"", ""costOfSaleSubAccountID"", ""revenueSubAccountID""
                        ";

                        using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@quantity", quantity.Last());
                            command.Parameters.AddWithValue("@itemID", itemID.Last());

                            using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    double unitCostValue = reader["unitCost"] is DBNull ? double.MinValue : (double)reader["unitCost"];
                                    double unitPriceValue = reader["unitPrice"] is DBNull ? double.MinValue : (double)reader["unitPrice"];
                                    int assetSubAccountIDValue = reader["assetSubAccountID"] is DBNull ? 0 : (int)reader["assetSubAccountID"];
                                    int costOfSaleSubAccountIDValue = reader["costOfSaleSubAccountID"] is DBNull ? 0 : (int)reader["costOfSaleSubAccountID"];
                                    int revenueSubAccountIDValue = reader["revenueSubAccountID"] is DBNull ? 0 : (int)reader["revenueSubAccountID"];

                                    unitCost.Add(unitCostValue);
                                    unitPriceList.Add(unitPriceValue);
                                    assetSubAccountIDList.Add(assetSubAccountIDValue);
                                    costOfSaleSubAccountIDList.Add(costOfSaleSubAccountIDValue);
                                    revenueSubAccountIDList.Add(revenueSubAccountIDValue);
                                }
                            }
                        }
                    }

                    //Insert JV
                    commandText = $@"
                            INSERT INTO 
                                    ""Accounts.JV.JournalVouchers"" 
                                        (""amount"", ""description"", ""fiscalPeriodID"",
                                            ""sourceReference"", ""transactionDateTime"",
                                            ""isAutomatic"", ""isPosted"",
                                                ""postedBySysUID"")
                            VALUES 
                                   ( @amount, @description, @fiscalPeriodID,
                                    @sourceReference, @transactionDateTime,
                                     @isAutomatic, @isPosted,@postedBySysUID )
                            RETURNING
                            ""journalVoucherID""
                            ";
                    int journalVoucherID = 0;
                    using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                    {
                        var description = "Cash Sale- POS";
                        var sourceReference = $"Cash Sale- Bill No: {billNo}. Receipt No:{receiptNo}";

                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);
                        command.Parameters.AddWithValue("@sourceReference", sourceReference);
                        command.Parameters.AddWithValue("@transactionDateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@isAutomatic", 1);
                        command.Parameters.AddWithValue("@isPosted", 1);
                        for (int i = 0; i < 1; i++)
                        {
                            command.Parameters.AddWithValue("@amount", payment.PosPaymentItems[i].AmountDue);
                        }

                        command.Parameters.AddWithValue("@postedBySysUID", userID);

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                journalVoucherID = reader["journalVoucherID"] is DBNull ? 0 : (int)reader["journalVoucherID"];
                            }
                        }
                    }


                    ///Insert Entries
                    //COG - Inventory
                    commandText = $@"
                          INSERT INTO 
                                ""Accounts.JV.AccountEntries""
                                (""creditAmount"", ""debitAmount"",  ""creditSubAccountID"",
                                    ""debitSubAccountID"", ""journalVoucherID"") 
                          VALUES 
                                (@creditAmount, @debitAmount, @creditSubAccountID,
                                    @debitSubAccountID, @journalVoucherID)";


                    for (int i = 0; i < quantity.Count; i++)
                    {
                        using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                        {
                            double amount = unitCost[i] * quantity[i]; // Calculate the amount for the current item                   

                            command.Parameters.AddWithValue("@creditAmount", amount);
                            command.Parameters.AddWithValue("@debitAmount", amount);
                            command.Parameters.AddWithValue("@creditSubAccountID", assetSubAccountIDList[i]);//Inventory
                            command.Parameters.AddWithValue("@debitSubAccountID", costOfSaleSubAccountIDList[i]);//COGS

                            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

                            int numberOfRowsAffectedCogsInventory = await command.ExecuteNonQueryAsync();
                        }

                        //CASH - REVENUE
                        commandText = $@"
                              INSERT INTO 
                                    ""Accounts.JV.AccountEntries""
                                    (""creditAmount"", ""debitAmount"",  ""creditSubAccountID"",
                                        ""debitSubAccountID"", ""journalVoucherID"") 
                              VALUES 
                                    (@creditAmount, @debitAmount,  @creditSubAccountID,
                                        @debitSubAccountID, @journalVoucherID)";

                        using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                        {
                            double totalUnitPriceList = unitPriceList[i] * quantity[i];

                            command.Parameters.AddWithValue("@creditAmount", totalUnitPriceList);
                            command.Parameters.AddWithValue("@debitAmount", totalUnitPriceList);
                            command.Parameters.AddWithValue("@creditSubAccountID", revenueSubAccountIDList[i]); //Revenue
                            command.Parameters.AddWithValue("@debitSubAccountID", 8);//Cash at hand
                            command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

                            int numberOfRowsAffectedCashRevenue = await command.ExecuteNonQueryAsync();

                            command.Parameters.Clear();
                        }
                    }

                    /*     //CASH - VAT
                         commandText = $@"
                                INSERT INTO 
                                      ""Accounts.JV.AccountEntries""
                                      (""creditAmount"", ""debitAmount"", ""creditSubAccountID"",
                                          ""debitSubAccountID"", ""journalVoucherID"") 
                                VALUES 
                                      (@creditAmount, @debitAmount,  @creditSubAccountID,
                                          @debitSubAccountID, @journalVoucherID) ";

                         using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                         {
                             command.Parameters.AddWithValue("@creditAmount", );
                             command.Parameters.AddWithValue("@debitAmount", );
                             command.Parameters.AddWithValue("@creditSubAccountID", 13);//Vat
                             command.Parameters.AddWithValue("@debitSubAccountID", 8);//cash

                             command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID); //TODO: make this dynamic

                             int numberOfRowsAffectedCashVat = await command.ExecuteNonQueryAsync(); 
                         }*/

                    //Update Sub Account Balances
                    IEnumerable<JournalVoucherEntry> journalVoucherEntriesDetails = await GetJournalVoucherEntriesDetailsAsync(journalVoucherID);

                    await PostJournalVoucherAsync(journalVoucherEntriesDetails);

                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private async Task<IEnumerable<JournalVoucherEntry>> GetJournalVoucherEntriesDetailsAsync(int journalVoucherID)
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


        private async Task<bool> PostJournalVoucherAsync(IEnumerable<JournalVoucherEntry> journalVoucherEntry)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();
            using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
            try
            {
                foreach (var item in journalVoucherEntry)
                {
                    var (creditAccountTypeID, debitAccountTypeID) = await GetAccountTypesAsync(connection, item.JournalVoucherEntryID);

                    string? commandText = null;

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

        public async Task<double?> CalculateTotalOrderAmount(int customerOrderID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var commandText = $@"
                        SELECT 
                            ""customerOrderID"", SUM(""subTotal"") AS totalSubTotal
                        FROM 
                            ""Sales.POS.OrderItems""
                        WHERE 
                            ""customerOrderID"" = @customerOrderID
                        GROUP BY 
                            ""customerOrderID"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", customerOrderID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            List<JournalVoucherEntry> journalVoucherEntries = new List<JournalVoucherEntry>();

            if (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(reader.GetOrdinal("totalSubTotal")))
                {
                    return reader["totalSubTotal"] is DBNull ? 0.0 : (double)reader["totalSubTotal"];
                }
            }
            return null;
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
    }
}
