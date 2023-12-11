using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Interfaces.Accounts;

namespace PointOfSaleSystem.Repo.Accounts
{
    public class TaxRepository : ITaxRepository
    {
        private IConfiguration _configuration;
        public TaxRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Vat Tax
        public async Task<VatType?> CreateVATTypeAsync(VatType vatType)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    INSERT INTO 
                       ""Inventory.Inventory.VATTypes""
                            (""vatTypeName"",
                            ""perRate"",
                            ""vatLiabSubAccountID"")
                       VALUES 
                           (@vatTypeName,
                            @perRate,
                            @vatLiabSubAccountID)
                        RETURNING
                            ""vatTypeID"",""vatTypeName"", ""perRate"", ""vatLiabSubAccountID"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@vatTypeName", vatType.VATTypeName);
            command.Parameters.AddWithValue("@perRate", vatType.PerRate);
            command.Parameters.AddWithValue("@vatLiabSubAccountID", vatType.VATLiabSubAccountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new VatType
                {
                    VATTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"],
                    VATTypeName = reader["vatTypeName"] is DBNull ? string.Empty : (string)reader["vatTypeName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]
                };
            }
            return null;
        }
        public async Task<VatType?> UpdateVATTypeAsync(VatType vatType)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    UPDATE
                        ""Inventory.Inventory.VATTypes""
                    SET
                        ""vatTypeName"" = @vatTypeName,
                        ""perRate""  =@perRate,
                        ""vatLiabSubAccountID""  =@vatLiabSubAccountID
                    WHERE
                        ""vatTypeID"" = @vatTypeID 
                    RETURNING
                        ""vatTypeID"", ""vatTypeName"", ""perRate"", ""vatLiabSubAccountID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@vatTypeName", vatType.VATTypeName);
            command.Parameters.AddWithValue("@perRate", vatType.PerRate);
            command.Parameters.AddWithValue("@vatLiabSubAccountID", vatType.VATLiabSubAccountID);
            command.Parameters.AddWithValue("@vatTypeID", vatType.VATTypeID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new VatType
                {
                    VATTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"],
                    VATTypeName = reader["vatTypeName"] is DBNull ? string.Empty : (string)reader["vatTypeName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]

                };
            }
            return null;
        }
        public async Task<IEnumerable<VatType>> GetAllVATTypesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""vatTypeID"", ""vatTypeName"", ""perRate"", ""vatLiabSubAccountID""
                    FROM 
                         ""Inventory.Inventory.VATTypes""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<VatType> vatTypes = new List<VatType>();

            while (await reader.ReadAsync())
            {
                vatTypes.Add(new VatType
                {
                    VATTypeName = reader["vatTypeName"] is DBNull ? string.Empty : (string)reader["vatTypeName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"],
                    VATTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"]
                });
            }
            return vatTypes;
        }

        public async Task<VatType?> GetVATTypeDetailsAsync(int vatTypeID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""vatTypeID"", ""vatTypeName"", ""perRate"", ""vatLiabSubAccountID""
                    FROM 
                       ""Inventory.Inventory.VATTypes""
                    WHERE 
                        ""vatTypeID"" = @vatTypeID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@vatTypeID", vatTypeID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new VatType
                {
                    VATTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"],
                    VATTypeName = reader["vatTypeName"] is DBNull ? string.Empty : (string)reader["vatTypeName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]
                };
            }
            return null;
        }

        public async Task<bool> DeleteVATTypeAsync(int vatTypeID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Inventory.Inventory.VATTypes""
                                WHERE 
                                    ""vatTypeID"" = @vatTypeID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@vatTypeID", vatTypeID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }
        public async Task<bool> DoesVATTypeExistAsync(int vatTypeID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Inventory.Inventory.VATTypes""
                                    WHERE 
                                          ""vatTypeID"" = @vatTypeID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@vatTypeID", vatTypeID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }

        //Other Tax
        public async Task<OtherTax?> CreateOtherTaxAsync(OtherTax otherTax)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    INSERT INTO 
                       ""Inventory.Inventory.OtherTaxes""
                            (""otherTaxName"",
                            ""perRate"",
                            ""vatLiabSubAccountID"")
                       VALUES 
                           (@otherTaxName,
                            @perRate,
                            @vatLiabSubAccountID)
                        RETURNING
                            ""otherTaxID"", ""otherTaxName"", ""perRate"", ""vatLiabSubAccountID"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@otherTaxName", otherTax.OtherTaxName);
            command.Parameters.AddWithValue("@perRate", otherTax.PerRate);
            command.Parameters.AddWithValue("@vatLiabSubAccountID", otherTax.VATLiabSubAccountID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new OtherTax
                {
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"],
                    OtherTaxName = reader["otherTaxName"] is DBNull ? string.Empty : (string)reader["otherTaxName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]
                };
            }
            return null;
        }
        public async Task<OtherTax?> UpdateOtherTaxAsync(OtherTax otherTax)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    UPDATE
                        ""Inventory.Inventory.OtherTaxes""
                    SET
                        ""otherTaxName"" = @otherTaxName,
                        ""perRate""  =@perRate,
                        ""vatLiabSubAccountID""  =@vatLiabSubAccountID
                    WHERE
                        ""otherTaxID"" = @otherTaxID 
                    RETURNING
                       ""otherTaxID"" ,""otherTaxName"", ""perRate"", ""vatLiabSubAccountID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@otherTaxName", otherTax.OtherTaxName);
            command.Parameters.AddWithValue("@perRate", otherTax.PerRate);
            command.Parameters.AddWithValue("@vatLiabSubAccountID", otherTax.VATLiabSubAccountID);
            command.Parameters.AddWithValue("@otherTaxID", otherTax.OtherTaxID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new OtherTax
                {
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"],
                    OtherTaxName = reader["otherTaxName"] is DBNull ? string.Empty : (string)reader["otherTaxName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]
                };
            }
            return null;
        }

        public async Task<IEnumerable<OtherTax>> GetAllOtherTaxesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""otherTaxID"", ""otherTaxName"", ""perRate"", ""vatLiabSubAccountID""
                    FROM 
                          ""Inventory.Inventory.OtherTaxes""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<OtherTax> otherTaxes = new List<OtherTax>();

            while (await reader.ReadAsync())
            {
                otherTaxes.Add(new OtherTax
                {
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"],
                    OtherTaxName = reader["otherTaxName"] is DBNull ? string.Empty : (string)reader["otherTaxName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]
                });
            }
            return otherTaxes;
        }

        public async Task<OtherTax?> GetOtherTaxDetailsAsync(int otherTaxID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""otherTaxID"", ""otherTaxName"", ""perRate"", ""vatLiabSubAccountID""
                    FROM 
                       ""Inventory.Inventory.OtherTaxes""
                    WHERE 
                        ""otherTaxID"" = @otherTaxID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@otherTaxID", otherTaxID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new OtherTax
                {
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"],
                    OtherTaxName = reader["otherTaxName"] is DBNull ? string.Empty : (string)reader["otherTaxName"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (int)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]
                };
            }
            return null;
        }
        public async Task<bool> DeleteOtherTaxAsync(int otherTaxID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Inventory.Inventory.OtherTaxes""
                                WHERE 
                                    ""otherTaxID"" = @otherTaxID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@otherTaxID", otherTaxID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> DoesOtherTaxExistAsync(int otherTaxID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                       ""Inventory.Inventory.OtherTaxes""
                                    WHERE 
                                        ""otherTaxID"" = @otherTaxID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@otherTaxID", otherTaxID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }

        //config
        public async Task<IEnumerable<SubAccount>> GetAllLiabilitySubAccountsAsync()
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

            command.Parameters.AddWithValue("@accountID", 132);

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
    }
}