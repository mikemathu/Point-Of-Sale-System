using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Interfaces.Inventory;

namespace PointOfSaleSystem.Repo.Inventory
{
    public class ItemRepository : IItemRepository
    {
        private IConfiguration _configuration;
        public ItemRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Item?> CreateItemAsync(Item itemModel)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string commandText = $@"
                    INSERT INTO 
                       ""Inventory.Inventory.Items""
                            (""itemName"",
                            ""unitCost"" ,
                            ""unitPrice"",
                            ""totalQuantity"" ,
                            ""availableQuantity"" , 
                            ""reorderLevel""  ,
                            ""expiryDate"",
                            ""itemCode"" , 
                            ""barcode"" ,  
                            ""batch""  ,
                            ""image"" , 
                            ""weight""  , 
                            ""length"" ,  
                            ""width""  , 
                            ""height"" , 
                            ""showInPOS""   ,
                            ""isActive"" ,
                            ""unitOfMeasureID"" ,  
                            ""itemClassID"",
                            ""itemCategoryID""  ,
                            ""assetSubAccountID"" ,
                            ""costOfSaleSubAccountID"", 
                            ""revenueSubAccountID"",
                            ""vatTypeID"",
                            ""otherTaxID"")
                       VALUES 
                           (@itemName,
                            @unitCost ,
                            @unitPrice ,
                            @totalQuantity ,
                            @availableQuantity ,
                            @reorderLevel,
                            @expiryDate,
                            @itemCode ,
                            @barcode , 
                            @batch,
                            @image ,
                            @weight  ,
                            @length  ,
                            @width ,
                            @height ,
                            @showInPOS  ,
                            @isActive ,
                            @unitOfMeasureID  ,
                            @itemClassID ,
                            @itemCategoryID ,
                            @assetSubAccountID ,
                            @costOfSaleSubAccountID ,
                            @revenueSubAccountID  ,
                            @vatTypeID  ,
                            @otherTaxID )
                        RETURNING
                            ""itemName"", ""batch"", ""unitCost"", ""unitPrice"", ""totalQuantity"", ""availableQuantity"", ""expiryDate"", ""itemID""";

                using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

                command.Parameters.AddWithValue("@itemName", itemModel.ItemName);
                command.Parameters.AddWithValue("@unitCost", itemModel.UnitCost);
                command.Parameters.AddWithValue("@unitPrice", itemModel.UnitPrice);
                command.Parameters.AddWithValue("@totalQuantity", itemModel.TotalQuantity);
                command.Parameters.AddWithValue("@availableQuantity", itemModel.TotalQuantity);
                command.Parameters.AddWithValue("@reorderLevel", itemModel.ReorderLevel);
                command.Parameters.AddWithValue("@expiryDate", itemModel.ExpiryDate);
                command.Parameters.AddWithValue("@itemCode", itemModel.ItemCode);
                command.Parameters.AddWithValue("@barcode", itemModel.Barcode);
                command.Parameters.AddWithValue("@batch", itemModel.Batch);
                command.Parameters.AddWithValue("@image", itemModel.Image);
                command.Parameters.AddWithValue("@weight", itemModel.Weight);
                command.Parameters.AddWithValue("@length", itemModel.Length);
                command.Parameters.AddWithValue("@width", itemModel.Width);
                command.Parameters.AddWithValue("@height", itemModel.Height);
                command.Parameters.AddWithValue("@showInPOS", itemModel.ShowInPOS);
                command.Parameters.AddWithValue("@isActive", 1);
                command.Parameters.AddWithValue("@unitOfMeasureID", itemModel.UnitOfMeasureID);
                command.Parameters.AddWithValue("@itemClassID", itemModel.ItemClassID);
                command.Parameters.AddWithValue("@itemCategoryID", itemModel.ItemCategoryID);
                command.Parameters.AddWithValue("@assetSubAccountID", itemModel.AssetSubAccountID);
                command.Parameters.AddWithValue("@costOfSaleSubAccountID", itemModel.CostOfSaleSubAccountID);
                command.Parameters.AddWithValue("@revenueSubAccountID", itemModel.RevenueSubAccountID);
                command.Parameters.AddWithValue("@vatTypeID", itemModel.VatTypeID);
                //command.Parameters.AddWithValue("@otherTaxID", itemModel.OtherTaxID);
                command.Parameters.AddWithValue("@otherTaxID", 1);//TODO

                await connection.OpenAsync();

                using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Item
                    {
                        ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                        Batch = reader["batch"] is DBNull ? string.Empty : (string)reader["batch"],
                        UnitCost = reader["unitCost"] is DBNull ? 0 : (double)reader["unitCost"],
                        UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                        TotalQuantity = reader["totalQuantity"] is DBNull ? 0 : (int)reader["totalQuantity"],
                        AvailableQuantity = reader["availableQuantity"] is DBNull ? 0 : (int)reader["availableQuantity"],
                        ExpiryDate = reader["expiryDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["expiryDate"],
                        ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"]
                    };
                }
                return null;
            }
        }

        public async Task<Item?> GetItemDetailsAsync(int itemID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                            SELECT 
                                I.*,
                                UOM.""unitOfMeasureName""
                            FROM 
                                ""Inventory.Inventory.Items"" AS I
                            LEFT JOIN 
                                ""Inventory.Inventory.UnitsOfMeasure"" AS UOM ON I.""unitOfMeasureID"" = UOM.""unitOfMeasureID""
                            WHERE 
                                I.""itemID"" = @itemID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemID", itemID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Item
                {
                    ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                    ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"],
                    UnitCost = reader["unitCost"] is DBNull ? 0 : (double)reader["unitCost"],
                    UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                    TotalQuantity = reader["totalQuantity"] is DBNull ? 0 : (int)reader["totalQuantity"],
                    AvailableQuantity = reader["availableQuantity"] is DBNull ? 0 : (int)reader["availableQuantity"],
                    ReorderLevel = reader["reorderLevel"] is DBNull ? 0 : (int)reader["reorderLevel"],
                    ExpiryDate = reader["expiryDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["expiryDate"],
                    ItemCode = reader["itemCode"] is DBNull ? string.Empty : (string)reader["itemCode"],
                    Barcode = reader["barcode"] is DBNull ? string.Empty : (string)reader["barcode"],
                    Batch = reader["batch"] is DBNull ? string.Empty : (string)reader["batch"],
                    Image = reader["image"] is DBNull ? string.Empty : (string)reader["image"],
                    Weight = reader["weight"] is DBNull ? 0 : (int)reader["weight"],
                    Length = reader["length"] is DBNull ? 0 : (int)reader["length"],
                    Width = reader["width"] is DBNull ? 0 : (int)reader["width"],
                    Height = reader["height"] is DBNull ? 0 : (int)reader["height"],
                    ShowInPOS = reader["showInPOS"] is DBNull ? 0 : (int)reader["showInPOS"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    UnitOfMeasure = new UnitOfMeasure { UnitOfMeasureName = reader["unitOfMeasureName"] is DBNull ? string.Empty : (string)reader["unitOfMeasureName"] },
                    UnitOfMeasureID = reader["unitOfMeasureID"] is DBNull ? 0 : (int)reader["unitOfMeasureID"],
                    ItemClassID = reader["itemClassID"] is DBNull ? 0 : (int)reader["itemClassID"],
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    AssetSubAccountID = reader["assetSubAccountID"] is DBNull ? 0 : (int)reader["assetSubAccountID"],
                    CostOfSaleSubAccountID = reader["costOfSaleSubAccountID"] is DBNull ? 0 : (int)reader["costOfSaleSubAccountID"],
                    RevenueSubAccountID = reader["revenueSubAccountID"] is DBNull ? 0 : (int)reader["revenueSubAccountID"],
                    VatTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"],
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"]
                };
            }
            return null;
        }

        private async Task<string> GetCommandTextByFlag(FilterItem filterFlag)
        {
            string commandText = $@"
                        SELECT 
                            *
                        FROM 
                            ""Inventory.Inventory.Items""
                        WHERE 
                            ""isActive"" = 1";
            if (filterFlag.FilterFlag == 1)
            {
                commandText = $@"
                        SELECT 
                            *
                        FROM 
                            ""Inventory.Inventory.Items""
                        WHERE 
                            ""isActive"" = 0";
            }
            if (filterFlag.FilterFlag == 2)
            {
                commandText = $@"
                        SELECT 
                            *
                        FROM 
                              ""Inventory.Inventory.Items""
                        WHERE 
                            ""availableQuantity"" = 0";
            }
            if (filterFlag.FilterFlag == 3)
            {
                commandText = $@"
                        SELECT 
                            *
                        FROM 
                             ""Inventory.Inventory.Items""
                        WHERE 
                            ""expiryDate"" < @dateToday";
            }

            if (filterFlag.FilterFlag == 4)
            {
                commandText = $@"
                        SELECT 
                            *
                        FROM 
                              ""Inventory.Inventory.Items""
                        WHERE 
                          ""availableQuantity"" < ""reorderLevel""";
            }

            if (filterFlag.FilterFlag == 5)
            {
                commandText = $@"
                        SELECT 
                            *
                        FROM 
                              ""Inventory.Inventory.Items""
                        WHERE 
                            ""showInPOS"" = 0";
            }

            if (filterFlag.FilterFlag == 6)
            {
                commandText = $@"
                        SELECT 
                            *
                        FROM 
                            ""Inventory.Inventory.Items""
                        WHERE 
                            ""showInPOS"" = 1";
            }
            return commandText;
        }
        public async Task<IEnumerable<Item>> FilterItemsAsync(FilterItem filterFlag)
        {
            string commandText = await GetCommandTextByFlag(filterFlag);

            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            DateTime dateToday = DateTime.Now;

            command.Parameters.AddWithValue("@dateToday", dateToday);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<Item> items = new List<Item>();

            while (await reader.ReadAsync())
            {
                items.Add(new Item
                {
                    ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                    ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"],
                    UnitCost = reader["unitCost"] is DBNull ? 0 : (double)reader["unitCost"],
                    UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                    TotalQuantity = reader["totalQuantity"] is DBNull ? 0 : (int)reader["totalQuantity"],
                    AvailableQuantity = reader["availableQuantity"] is DBNull ? 0 : (int)reader["availableQuantity"],
                    ReorderLevel = reader["reorderLevel"] is DBNull ? 0 : (int)reader["reorderLevel"],
                    ExpiryDate = reader["expiryDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["expiryDate"],
                    ItemCode = reader["itemCode"] is DBNull ? string.Empty : (string)reader["itemCode"],
                    Barcode = reader["barcode"] is DBNull ? string.Empty : (string)reader["barcode"],
                    Batch = reader["batch"] is DBNull ? string.Empty : (string)reader["batch"],
                    Image = reader["image"] is DBNull ? string.Empty : (string)reader["image"],
                    Weight = reader["weight"] is DBNull ? 0 : (int)reader["weight"],
                    Length = reader["length"] is DBNull ? 0 : (int)reader["length"],
                    Width = reader["width"] is DBNull ? 0 : (int)reader["width"],
                    Height = reader["height"] is DBNull ? 0 : (int)reader["height"],
                    ShowInPOS = reader["showInPOS"] is DBNull ? 0 : (int)reader["showInPOS"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    UnitOfMeasureID = reader["unitOfMeasureID"] is DBNull ? 0 : (int)reader["unitOfMeasureID"],
                    ItemClassID = reader["itemClassID"] is DBNull ? 0 : (int)reader["itemClassID"],
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    AssetSubAccountID = reader["assetSubAccountID"] is DBNull ? 0 : (int)reader["assetSubAccountID"],
                    CostOfSaleSubAccountID = reader["costOfSaleSubAccountID"] is DBNull ? 0 : (int)reader["costOfSaleSubAccountID"],
                    RevenueSubAccountID = reader["revenueSubAccountID"] is DBNull ? 0 : (int)reader["revenueSubAccountID"],
                    VatTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"],
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"]
                });
            }
            return items;
        }

        public async Task<Item?> SearchItemsAsync(string productName)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        *
                    FROM 
                        ""Inventory.Inventory.Items""
                    WHERE 
                        ""itemName"" LIKE @productName";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.Add(new NpgsqlParameter("@productName", NpgsqlTypes.NpgsqlDbType.Text) { Value = productName + "%" });

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Item
                {
                    ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                    ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"],
                    UnitCost = reader["unitCost"] is DBNull ? 0 : (double)reader["unitCost"],
                    UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                    TotalQuantity = reader["totalQuantity"] is DBNull ? 0 : (int)reader["totalQuantity"],
                    AvailableQuantity = reader["availableQuantity"] is DBNull ? 0 : (int)reader["availableQuantity"],
                    ReorderLevel = reader["reorderLevel"] is DBNull ? 0 : (int)reader["reorderLevel"],
                    ExpiryDate = reader["expiryDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["expiryDate"],
                    ItemCode = reader["itemCode"] is DBNull ? string.Empty : (string)reader["itemCode"],
                    Barcode = reader["barcode"] is DBNull ? string.Empty : (string)reader["barcode"],
                    Batch = reader["batch"] is DBNull ? string.Empty : (string)reader["batch"],
                    Image = reader["image"] is DBNull ? string.Empty : (string)reader["image"],
                    Weight = reader["weight"] is DBNull ? 0 : (int)reader["weight"],
                    Length = reader["length"] is DBNull ? 0 : (int)reader["length"],
                    Width = reader["width"] is DBNull ? 0 : (int)reader["width"],
                    Height = reader["height"] is DBNull ? 0 : (int)reader["height"],
                    ShowInPOS = reader["showInPOS"] is DBNull ? 0 : (int)reader["showInPOS"],
                    IsActive = reader["isActive"] is DBNull ? 0 : (int)reader["isActive"],
                    UnitOfMeasureID = reader["unitOfMeasureID"] is DBNull ? 0 : (int)reader["unitOfMeasureID"],
                    ItemClassID = reader["itemClassID"] is DBNull ? 0 : (int)reader["itemClassID"],
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    AssetSubAccountID = reader["assetSubAccountID"] is DBNull ? 0 : (int)reader["assetSubAccountID"],
                    CostOfSaleSubAccountID = reader["costOfSaleSubAccountID"] is DBNull ? 0 : (int)reader["costOfSaleSubAccountID"],
                    RevenueSubAccountID = reader["revenueSubAccountID"] is DBNull ? 0 : (int)reader["revenueSubAccountID"],
                    VatTypeID = reader["vatTypeID"] is DBNull ? 0 : (int)reader["vatTypeID"],
                    OtherTaxID = reader["otherTaxID"] is DBNull ? 0 : (int)reader["otherTaxID"]
                };
            }
            return null;
        }

        public async Task<Item> UpdateItemCreatingJVAsync(Item itemModel, Item itemDetails, int userID, int fiscalPeriodID)
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

                string commandText = $@"
                    UPDATE
                        ""Inventory.Inventory.Items""
                    SET
                        ""itemName"" = @itemName,
                        ""unitCost""  =@unitCost,
                        ""unitPrice""  = @unitPrice,
                        ""totalQuantity""  = @totalQuantity,
                        ""availableQuantity""  = @availableQuantity, 
                        ""reorderLevel""   = @reorderLevel,
                        ""expiryDate""  = @expiryDate,
                        ""itemCode""  = @itemCode, 
                        ""barcode""  = @barcode,  
                        ""batch""   = @batch,
                        ""image""  = @image, 
                        ""weight""   = @weight, 
                        ""length""  = @length,  
                        ""width""   = @width, 
                        ""height""  = @height, 
                        ""showInPOS""   = @showInPOS,
                        ""isActive"" =@isActive,
                        ""unitOfMeasureID"" = @unitOfMeasureID, 
                        ""itemClassID""  = @itemClassID,
                        ""itemCategoryID""  = @itemCategoryID, 
                        ""assetSubAccountID"" = @assetSubAccountID,
                        ""costOfSaleSubAccountID"" = @costOfSaleSubAccountID, 
                        ""revenueSubAccountID""   = @revenueSubAccountID,
                        ""vatTypeID""   = @vatTypeID,
                        ""otherTaxID""   = @otherTaxID 
                    WHERE
                        ""itemID"" = @itemID 
                    RETURNING
                        ""itemName"", ""batch"", ""unitCost"", ""unitPrice"", ""totalQuantity"", ""availableQuantity"", ""expiryDate"", ""itemID""";
                Item createdItem = null;
                using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                {
                    command.Parameters.AddWithValue("@itemID", itemModel.ItemID);
                    command.Parameters.AddWithValue("@itemName", itemModel.ItemName);
                    command.Parameters.AddWithValue("@unitCost", itemModel.UnitCost);
                    command.Parameters.AddWithValue("@unitPrice", itemModel.UnitPrice);
                    command.Parameters.AddWithValue("@totalQuantity", itemModel.TotalQuantity);
                    command.Parameters.AddWithValue("@availableQuantity", itemModel.TotalQuantity);
                    command.Parameters.AddWithValue("@reorderLevel", itemModel.ReorderLevel);
                    command.Parameters.AddWithValue("@expiryDate", itemModel.ExpiryDate);
                    command.Parameters.AddWithValue("@itemCode", itemModel.ItemCode);
                    command.Parameters.AddWithValue("@barcode", itemModel.Barcode);
                    command.Parameters.AddWithValue("@batch", itemModel.Batch);
                    command.Parameters.AddWithValue("@image", itemModel.Image);
                    command.Parameters.AddWithValue("@weight", itemModel.Weight);
                    command.Parameters.AddWithValue("@length", itemModel.Length);
                    command.Parameters.AddWithValue("@width", itemModel.Width);
                    command.Parameters.AddWithValue("@height", itemModel.Height);
                    command.Parameters.AddWithValue("@showInPOS", itemModel.ShowInPOS);
                    command.Parameters.AddWithValue("@isActive", 1);
                    command.Parameters.AddWithValue("@unitOfMeasureID", itemModel.UnitOfMeasureID);
                    command.Parameters.AddWithValue("@itemClassID", itemModel.ItemClassID);
                    command.Parameters.AddWithValue("@itemCategoryID", itemModel.ItemCategoryID);
                    command.Parameters.AddWithValue("@assetSubAccountID", itemModel.AssetSubAccountID);
                    command.Parameters.AddWithValue("@costOfSaleSubAccountID", itemModel.CostOfSaleSubAccountID);
                    command.Parameters.AddWithValue("@revenueSubAccountID", itemModel.RevenueSubAccountID);
                    command.Parameters.AddWithValue("@vatTypeID", itemModel.VatTypeID);
                    //command.Parameters.AddWithValue("@otherTaxID", itemModel.OtherTaxID);
                    command.Parameters.AddWithValue("@otherTaxID", 1);//TODO

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            createdItem = new Item
                            {
                                ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                                Batch = reader["batch"] is DBNull ? string.Empty : (string)reader["batch"],
                                UnitCost = reader["unitCost"] is DBNull ? 0 : (double)reader["unitCost"],
                                UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                                TotalQuantity = reader["totalQuantity"] is DBNull ? 0 : (int)reader["totalQuantity"],
                                AvailableQuantity = reader["availableQuantity"] is DBNull ? 0 : (int)reader["availableQuantity"],
                                ExpiryDate = reader["expiryDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["expiryDate"],
                                ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"]
                            };
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
                            ""journalVoucherID"" ";
                double amount = 0.0;
                int journalVoucherID = 0;
                using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                {
                    var sourceReference = $"Updated a product: {itemModel.ItemName}";
                    string description = "";

                    if (itemDetails.AvailableQuantity > itemModel.TotalQuantity)//Database value > incoming value -> Decreace
                    {
                        description = $"Quantity Decrease By: " +
                            $"{itemDetails.AvailableQuantity - itemModel.TotalQuantity} {itemDetails.UnitOfMeasure.UnitOfMeasureName}";
                        amount = (itemDetails.AvailableQuantity - itemModel.TotalQuantity) * itemDetails.UnitCost;
                    }
                    else//Database value < incoming value -> Increace
                    {
                        description = $"Quantity Increased By: " +
                            $"{itemModel.TotalQuantity - itemDetails.AvailableQuantity} {itemDetails.UnitOfMeasure.UnitOfMeasureName}";
                        amount = (itemModel.TotalQuantity - itemDetails.AvailableQuantity) * itemDetails.UnitCost;
                    }

                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@fiscalPeriodID", fiscalPeriodID);
                    command.Parameters.AddWithValue("@sourceReference", sourceReference);
                    command.Parameters.AddWithValue("@transactionDateTime", DateTime.Now);
                    command.Parameters.AddWithValue("@isAutomatic", 1);
                    command.Parameters.AddWithValue("@isPosted", 1);
                    command.Parameters.AddWithValue("@amount", amount);

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
                ///COG - Inventory
                commandText = $@"
                          INSERT INTO 
                                ""Accounts.JV.AccountEntries""
                                (""creditAmount"", ""debitAmount"",  ""creditSubAccountID"",
                                    ""debitSubAccountID"", ""journalVoucherID"") 
                          VALUES 
                                (@creditAmount, @debitAmount, @creditSubAccountID,
                                    @debitSubAccountID, @journalVoucherID)";

                using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection, transaction))
                {
                    command.Parameters.AddWithValue("@creditAmount", amount);
                    command.Parameters.AddWithValue("@debitAmount", amount);
                    command.Parameters.AddWithValue("@creditSubAccountID", itemDetails.AssetSubAccountID);//Inventory sub Account
                    command.Parameters.AddWithValue("@debitSubAccountID", 10);//Owners equity opening balance

                    command.Parameters.AddWithValue("@journalVoucherID", journalVoucherID);

                    int numberOfRowsAffectedCogsInventory = await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                return createdItem;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Item> UpdateItemAsync(Item itemModel)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    UPDATE
                        ""Inventory.Inventory.Items""
                    SET
                        ""itemName"" = @itemName,
                        ""unitCost""  =@unitCost,
                        ""unitPrice""  = @unitPrice,
                        ""totalQuantity""  = @totalQuantity,
                        ""availableQuantity""  = @availableQuantity, 
                        ""reorderLevel""   = @reorderLevel,
                        ""expiryDate""  = @expiryDate,
                        ""itemCode""  = @itemCode, 
                        ""barcode""  = @barcode,  
                        ""batch""   = @batch,
                        ""image""  = @image, 
                        ""weight""   = @weight, 
                        ""length""  = @length,  
                        ""width""   = @width, 
                        ""height""  = @height, 
                        ""showInPOS""   = @showInPOS,
                        ""isActive"" =@isActive,
                        ""unitOfMeasureID"" = @unitOfMeasureID, 
                        ""itemClassID""  = @itemClassID,
                        ""itemCategoryID""  = @itemCategoryID, 
                        ""assetSubAccountID"" = @assetSubAccountID,
                        ""costOfSaleSubAccountID"" = @costOfSaleSubAccountID, 
                        ""revenueSubAccountID""   = @revenueSubAccountID,
                        ""vatTypeID""   = @vatTypeID,
                        ""otherTaxID""   = @otherTaxID 
                    WHERE
                        ""itemID"" = @itemID 
                    RETURNING
                        ""itemName"", ""batch"", ""unitCost"", ""unitPrice"", ""totalQuantity"", ""availableQuantity"", ""expiryDate"", ""itemID""";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemID", itemModel.ItemID);
            command.Parameters.AddWithValue("@itemName", itemModel.ItemName);
            command.Parameters.AddWithValue("@unitCost", itemModel.UnitCost);
            command.Parameters.AddWithValue("@unitPrice", itemModel.UnitPrice);
            command.Parameters.AddWithValue("@totalQuantity", itemModel.TotalQuantity);
            command.Parameters.AddWithValue("@availableQuantity", itemModel.TotalQuantity);
            command.Parameters.AddWithValue("@reorderLevel", itemModel.ReorderLevel);
            command.Parameters.AddWithValue("@expiryDate", itemModel.ExpiryDate);
            command.Parameters.AddWithValue("@itemCode", itemModel.ItemCode);
            command.Parameters.AddWithValue("@barcode", itemModel.Barcode);
            command.Parameters.AddWithValue("@batch", itemModel.Batch);
            command.Parameters.AddWithValue("@image", itemModel.Image);
            command.Parameters.AddWithValue("@weight", itemModel.Weight);
            command.Parameters.AddWithValue("@length", itemModel.Length);
            command.Parameters.AddWithValue("@width", itemModel.Width);
            command.Parameters.AddWithValue("@height", itemModel.Height);
            command.Parameters.AddWithValue("@showInPOS", itemModel.ShowInPOS);
            command.Parameters.AddWithValue("@isActive", 1);
            command.Parameters.AddWithValue("@unitOfMeasureID", itemModel.UnitOfMeasureID);
            command.Parameters.AddWithValue("@itemClassID", itemModel.ItemClassID);
            command.Parameters.AddWithValue("@itemCategoryID", itemModel.ItemCategoryID);
            command.Parameters.AddWithValue("@assetSubAccountID", itemModel.AssetSubAccountID);
            command.Parameters.AddWithValue("@costOfSaleSubAccountID", itemModel.CostOfSaleSubAccountID);
            command.Parameters.AddWithValue("@revenueSubAccountID", itemModel.RevenueSubAccountID);
            command.Parameters.AddWithValue("@vatTypeID", itemModel.VatTypeID);
            //command.Parameters.AddWithValue("@otherTaxID", itemModel.OtherTaxID);
            command.Parameters.AddWithValue("@otherTaxID", 1);//TODO

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                Item createdItem = new Item
                {
                    ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                    Batch = reader["batch"] is DBNull ? string.Empty : (string)reader["batch"],
                    UnitCost = reader["unitCost"] is DBNull ? 0 : (double)reader["unitCost"],
                    UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                    TotalQuantity = reader["totalQuantity"] is DBNull ? 0 : (int)reader["totalQuantity"],
                    AvailableQuantity = reader["availableQuantity"] is DBNull ? 0 : (int)reader["availableQuantity"],
                    ExpiryDate = reader["expiryDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["expiryDate"],
                    ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"]
                };

                return createdItem;
            }
            return null;
        }
        public async Task<bool> DeleteItemAsync(int itemID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Inventory.Inventory.Items""
                                WHERE 
                                    ""itemID"" = @itemID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemID", itemID);

            await connection.OpenAsync();

            int numberOfRowsAffected = await command.ExecuteNonQueryAsync();

            return numberOfRowsAffected > 0;
        }


        public async Task<bool> DoesItemExist(int itemID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                       ""Inventory.Inventory.Items"" 
                                    WHERE 
                                        ""itemID"" = @itemID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemID", itemID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
        }


    }
}
