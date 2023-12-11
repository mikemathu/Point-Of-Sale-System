using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Interfaces.Inventory;

namespace PointOfSaleSystem.Repo.Inventory
{
    public class InventoryConfigRepository : InventoryConfig
    {
        private IConfiguration _configuration;
        public InventoryConfigRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //ItemCategory
        public async Task<ItemCategory?> CreateItemCategoryAsync(ItemCategory itemCategory)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    INSERT INTO 
                       ""Inventory.Inventory.ItemCategories""
                            (""itemCategoryName"",
                            ""description"" )
                       VALUES 
                           (@itemCategoryName,
                            @description )
                        RETURNING
                            ""itemCategoryID"", ""itemCategoryName"", ""description"" ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemCategoryName", itemCategory.ItemCategoryName);
            command.Parameters.AddWithValue("@description", itemCategory.Description);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ItemCategory
                {
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    ItemCategoryName = reader["itemCategoryName"] is DBNull ? string.Empty : (string)reader["itemCategoryName"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"]
                };
            }
            return null;
        }

        public async Task<ItemCategory?> UpdateItemCategoryAsync(ItemCategory itemCategory)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    UPDATE
                        ""Inventory.Inventory.ItemCategories""
                    SET
                        ""itemCategoryName"" = @itemCategoryName,
                        ""description""  =@description
                    WHERE
                        ""itemCategoryID"" = @itemCategoryID 
                    RETURNING
                        ""itemCategoryID"", ""itemCategoryName"", ""description""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemCategoryName", itemCategory.ItemCategoryName);
            command.Parameters.AddWithValue("@description", itemCategory.Description);
            command.Parameters.AddWithValue("@itemCategoryID", itemCategory.ItemCategoryID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ItemCategory
                {
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    ItemCategoryName = reader["itemCategoryName"] is DBNull ? string.Empty : (string)reader["itemCategoryName"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"]
                };
            }
            return null;
        }
        public async Task<ItemCategory?> GetItemCategoryDetailsAsync(int itemCategory)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""itemCategoryName"", ""description"", ""itemCategoryID""
                    FROM 
                       ""Inventory.Inventory.ItemCategories""
                    WHERE 
                        ""itemCategoryID"" = @itemCategoryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemCategoryID", itemCategory);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ItemCategory
                {
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    ItemCategoryName = reader["itemCategoryName"] is DBNull ? string.Empty : (string)reader["itemCategoryName"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"]
                };
            }
            return null;
        }

        public async Task<bool> DeleteItemCategoryAsync(int itemCategoryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Inventory.Inventory.ItemCategories""
                                WHERE 
                                    ""itemCategoryID"" = @itemCategoryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemCategoryID", itemCategoryID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> DoesItemCategoryExist(int itemCategoryID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                       ""Inventory.Inventory.ItemCategories""
                                    WHERE 
                                        ""itemCategoryID"" = @itemCategoryID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemCategoryID", itemCategoryID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }

        //ItemClass
        public async Task<ItemClass?> CreateItemClassAsync(ItemClass itemClass)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string commandText = $@"
                    INSERT INTO 
                       ""Invetory.Inventory.ItemClasses""
                            (""itemClassName"",
                            ""description"" ,
                            ""itemClassTypeID"" )
                       VALUES 
                           (@itemClassName,
                            @description ,
                            @itemClassTypeID )
                        RETURNING
                            ""itemClassID"",""itemClassName"", ""description"", ""itemClassTypeID"" ";

                using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@itemClassName", itemClass.ItemClassName);
                command.Parameters.AddWithValue("@description", itemClass.Description);
                command.Parameters.AddWithValue("@itemClassTypeID", itemClass.ItemClassTypeID);

                await connection.OpenAsync();

                using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ItemClass
                    {
                        ItemClassID = reader["itemClassID"] is DBNull ? 0 : (int)reader["itemClassID"],
                        ItemClassName = reader["itemClassName"] is DBNull ? string.Empty : (string)reader["itemClassName"],
                        Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                        ItemClassTypeID = reader["itemClassTypeID"] is DBNull ? 0 : (int)reader["itemClassTypeID"]
                    };
                }
                return null;
            }
        }

        public async Task<ItemClass?> UpdateItemClassAsync(ItemClass itemClass)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    UPDATE
                       ""Invetory.Inventory.ItemClasses""
                    SET
                        ""itemClassName"" = @itemClassName,
                        ""description""  =@description,
                        ""itemClassTypeID""  =@itemClassTypeID
                    WHERE
                        ""itemClassID"" = @itemClassID 
                    RETURNING
                        ""itemClassID"",""itemClassName"", ""description"", ""itemClassTypeID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemClassName", itemClass.ItemClassName);
            command.Parameters.AddWithValue("@description", itemClass.Description);
            command.Parameters.AddWithValue("@itemClassTypeID", itemClass.ItemClassTypeID);
            command.Parameters.AddWithValue("@itemClassID", itemClass.ItemClassID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ItemClass
                {
                    ItemClassID = reader["itemClassID"] is DBNull ? 0 : (int)reader["itemClassID"],
                    ItemClassName = reader["itemClassName"] is DBNull ? string.Empty : (string)reader["itemClassName"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    ItemClassTypeID = reader["itemClassTypeID"] is DBNull ? 0 : (int)reader["itemClassTypeID"]
                };
            }
            return null;
        }

        public async Task<ItemClass?> GetItemClassDetailsAsync(int itemClassID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""itemClassID"",""itemClassName"", ""description"", ""itemClassTypeID""
                    FROM 
                      ""Invetory.Inventory.ItemClasses""
                    WHERE 
                        ""itemClassID"" = @itemClassID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemClassID", itemClassID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ItemClass
                {
                    ItemClassID = reader["itemClassID"] is DBNull ? 0 : (int)reader["itemClassID"],
                    ItemClassName = reader["itemClassName"] is DBNull ? string.Empty : (string)reader["itemClassName"],
                    Description = reader["description"] is DBNull ? string.Empty : (string)reader["description"],
                    ItemClassTypeID = reader["itemClassTypeID"] is DBNull ? 0 : (int)reader["itemClassTypeID"]
                };
            }
            return null;
        }

        public async Task<bool> DeleteItemClassAsync(int itemClassID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Invetory.Inventory.ItemClasses""
                                WHERE 
                                    ""itemClassID"" = @itemClassID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemClassID", itemClassID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }
        public async Task<bool> DoesItemClassExist(int itemClassID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                       ""Invetory.Inventory.ItemClasses""
                                    WHERE 
                                        ""itemClassID"" = @itemClassID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemClassID", itemClassID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }
    }
}
