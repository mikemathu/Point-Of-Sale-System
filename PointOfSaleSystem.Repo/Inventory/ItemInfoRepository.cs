using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Interfaces.Inventory;

namespace PointOfSaleSystem.Repo.Inventory
{
    public class ItemInfoRepository : IItemInfoRepository
    {
        private readonly IConfiguration _configuration;
        public ItemInfoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IEnumerable<ItemCategory>> GetAllItemCategoriesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""description"",  ""itemCategoryID"", ""itemCategoryName""
                    FROM 
                        ""Inventory.Inventory.ItemCategories""
                    ORDER BY
                            ""itemCategoryID"" ASC";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<ItemCategory> itemCategories = new List<ItemCategory>();

            while (await reader.ReadAsync())
            {
                itemCategories.Add(new ItemCategory
                {
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    ItemCategoryName = reader["itemCategoryName"] is DBNull ? string.Empty : (string)reader["itemCategoryName"]
                });
            }
            return itemCategories;
        }
        public async Task<IEnumerable<UnitOfMeasure>> GetAllUnitOfMeasures()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""isSmallestUnit"", ""unitOfMeasureName"", ""unitOfMeasureID""
                    FROM 
                        ""Inventory.Inventory.UnitsOfMeasure""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<UnitOfMeasure> products = new List<UnitOfMeasure>();

            while (await reader.ReadAsync())
            {
                products.Add(new UnitOfMeasure
                {
                    IsSmallestUnit = reader["isSmallestUnit"] is DBNull ? 0 : (int)reader["isSmallestUnit"],
                    UnitOfMeasureName = reader["unitOfMeasureName"] is DBNull ? string.Empty : (string)reader["unitOfMeasureName"],
                    UnitOfMeasureID = reader["unitOfMeasureID"] is DBNull ? 0 : (int)reader["unitOfMeasureID"]
                });
            }
            return products;
        }
        public async Task<IEnumerable<ItemClass>> GetAllItemClasses()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""description"", ""itemClassID"", ""itemClassTypeID"", ""itemClassName""
                    FROM 
                        ""Invetory.Inventory.ItemClasses"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<ItemClass> itemClasses = new List<ItemClass>();

            while (await reader.ReadAsync())
            {
                itemClasses.Add(new ItemClass
                {
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    ItemClassID = reader["itemClassID"] is DBNull ? 0 : (int)reader["itemClassID"],
                    ItemClassTypeID = reader["itemClassTypeID"] is DBNull ? 0 : (int)reader["itemClassTypeID"],
                    ItemClassName = reader["itemClassName"] is DBNull ? string.Empty : (string)reader["itemClassName"]
                });
            }
            return itemClasses;
        }

        public async Task<IEnumerable<OtherTax>> GetAllOtherTaxesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""otherTaxName"", ""otherTaxID"", ""perRate"", ""vatLiabSubAccountID""
                    FROM 
                        ""Inventory.Inventory.OtherTaxes""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<OtherTax> items = new List<OtherTax>();

            while (await reader.ReadAsync())
            {
                items.Add(new OtherTax
                {
                    OtherTaxName = reader["otherTaxName"] is DBNull ? string.Empty : (string)reader["otherTaxName"],
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"],
                    PerRate = reader["perRate"] is DBNull ? 0 : (double)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"]
                });
            }
            return items;
        }

        public async Task<IEnumerable<VatType>> GetAllVATTypesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""vatTypeName"", ""perRate"", ""vatLiabSubAccountID"", ""vatTypeID""
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
                    PerRate = reader["perRate"] is DBNull ? 0 : (double)reader["perRate"],
                    VATLiabSubAccountID = reader["vatLiabSubAccountID"] is DBNull ? 0 : (int)reader["vatLiabSubAccountID"],
                    VATTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"]
                });
            }
            return vatTypes;
        }
    }
}
