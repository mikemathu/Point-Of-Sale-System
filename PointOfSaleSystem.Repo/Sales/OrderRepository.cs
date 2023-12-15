using Microsoft.Extensions.Configuration;
using Npgsql;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Data.Sales;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Interfaces.Sales;

namespace PointOfSaleSystem.Repo.Sales
{
    public class OrderRepository : IOrderRepository
    {
        private IConfiguration _configuration;
        public OrderRepository(IConfiguration config)
        {
            _configuration = config;
        }
        public async Task<bool> AddPosItemToOrderAsync(CustomerOrderDto customerOrderID, int quantity, double subTotal, double unitPrice)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string commandText = $@"UPDATE 
                                    ""Sales.POS.OrderItems""
                                SET
                                    ""quantity"" = @newQuantity,
                                    ""subTotal"" = @subTotal
                                WHERE
                                    ""customerOrderID"" = @customerOrderID
                                AND
                                    ""itemID"" = @itemID";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            int newQuantity = quantity + 1;
            double newsubTotal = subTotal + unitPrice;

            command.Parameters.AddWithValue("@newQuantity", newQuantity);
            command.Parameters.AddWithValue("@subTotal", newsubTotal);
            command.Parameters.AddWithValue("@customerOrderID", customerOrderID.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", customerOrderID.CustomerOrderItemID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> AddPosItemToOrderAsync(CustomerOrderDto customerOrderID, double unitPrice)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                INSERT INTO 
                    ""Sales.POS.OrderItems""
                    (""customerOrderID"", ""itemID"", ""quantity"",""subTotal"")
                VALUES
                    (@customerOrderID, @itemID, @quantity, @subTotal)";
            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", customerOrderID.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", customerOrderID.CustomerOrderItemID);
            command.Parameters.AddWithValue("@quantity", 1);
            command.Parameters.AddWithValue("@subTotal", unitPrice);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }
        public async Task<bool> CreatePosOrderAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                INSERT INTO 
                    ""Inventory.Inventory.CustomerOrders""
                    (""createdBySysUserID"", ""dateTimeCreated"", ""isOrderPaid"",  ""billIsPrinted"")
                VALUES 
                    (@createdBySysUserID, @dateTimeCreated, @isOrderPaid,  @billIsPrinted)";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);


            command.Parameters.AddWithValue("@createdBySysUserID", 1);
            command.Parameters.AddWithValue("@dateTimeCreated", DateTime.Now);
            command.Parameters.AddWithValue("@isOrderPaid", 0);
            command.Parameters.AddWithValue("@billIsPrinted", 0);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> RemoveOrderAsync(int orderID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                    ""Inventory.Inventory.CustomerOrders""
                                 WHERE 
                                    ""customerOrderID"" = @customerOrderID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", orderID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> UpdatePosItemQuantity(OrderedItem orderItem, double unitPrice)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"UPDATE 
                                        ""Sales.POS.OrderItems""
                                    SET
                                        ""quantity"" = @newQuantity,
                                        ""subTotal"" = @newSubTotal
                                    WHERE
                                        ""customerOrderID"" = @customerOrderID
                                    AND
                                        ""itemID"" = @itemID ";

            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            var newsubTotal = orderItem.Quantity * unitPrice;

            command.Parameters.AddWithValue("@newQuantity", orderItem.Quantity);
            command.Parameters.AddWithValue("@customerOrderID", orderItem.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", orderItem.ItemID);
            command.Parameters.AddWithValue("@newSubTotal", newsubTotal);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<bool> RemoveItem(OrderedItem orderItem)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"DELETE FROM 
                                        ""Sales.POS.OrderItems""
                                    WHERE 
                                        ""customerOrderID"" = @customerOrderID
                                    AND
                                        ""itemID"" = @itemID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", orderItem.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", orderItem.ItemID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }

        public async Task<IEnumerable<OrderedItem>> GetPosOrderItemsAsync(int orderID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                SELECT 
                    OI.""customerOrderID"", OI.""subTotal"", OI.""itemID"", OI.""quantity"", P.""itemName"", P.""unitPrice"", U.""unitOfMeasureName""
                FROM 
                    ""Sales.POS.OrderItems"" OI
                INNER JOIN 
                    ""Inventory.Inventory.Items"" P
                ON 
                    OI.""itemID"" = P.""itemID""
                INNER JOIN 
                    ""Inventory.Inventory.UnitsOfMeasure"" U
                ON 
                    P.""unitOfMeasureID"" = U.""unitOfMeasureID""
                WHERE 
                    OI.""customerOrderID"" = @customerOrderID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", orderID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<OrderedItem> orderItems = new List<OrderedItem>();

            while (await reader.ReadAsync())
            {
                orderItems.Add(new OrderedItem
                {
                    SubTotal = reader["subTotal"] is DBNull ? 0 : (double)reader["subTotal"],
                    Quantity = reader["quantity"] is DBNull ? 0 : (int)reader["quantity"],
                    ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"],
                    ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                    UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                    CustomerOrderID = reader["customerOrderID"] is DBNull ? 0 : (int)reader["customerOrderID"],
                    UnitOfMeasureName = reader["unitOfMeasureName"] is DBNull ? string.Empty : (string)reader["unitOfMeasureName"]
                });
            }
            return orderItems;
        }

        public async Task<double> GetItemUnitPriceAsync(CustomerOrderDto createCustomerOrderDto)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                    P.""unitPrice"", U.""unitOfMeasureName""                                           
                                    FROM 
                                        ""Inventory.Inventory.Items"" P INNER JOIN ""Inventory.Inventory.UnitsOfMeasure"" u
                                    ON 
                                            P.""unitOfMeasureID"" = U.""unitOfMeasureID""
                                    WHERE 
                                        p.""itemID"" = @itemID ";

            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemID", createCustomerOrderDto.CustomerOrderItemID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            double unitPrice = 0;

            if (await reader.ReadAsync())
            {
                unitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"];
            }
            return unitPrice;
        }

        public async Task<double> GetItemUnitPriceAsync(int itemID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"SELECT 
                                        ""unitPrice""

                                        FROM 
                                            ""Inventory.Inventory.Items""
                                        WHERE 
                                            ""itemID"" = @itemID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@itemID", itemID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"];
            }
            return 0;
        }

        public async Task<int> CheckQuantity(OrderedItem orderItem)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""quantity""
                                FROM 
                                    ""Sales.POS.OrderItems""
                                WHERE 
                                    ""customerOrderID"" = @customerOrderID 
                                AND
                                    ""itemID"" = @itemID";

            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", orderItem.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", orderItem.ItemID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            int quantity = 0;

            if (await reader.ReadAsync())
            {
                quantity = reader["quantity"] is DBNull ? 0 : (int)reader["quantity"];
            }
            return quantity;
        }

        public async Task<(int Quantity, double SubTotal)> IsItemAvailable(CustomerOrder customerOrderID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""quantity"", ""subTotal""
                                FROM 
                                    ""Sales.POS.OrderItems""
                                WHERE 
                                    ""customerOrderID"" = @customerOrderID 
                                AND
                                    ""itemID"" = @itemID";

            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", customerOrderID.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", customerOrderID.CustomerOrderItemID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
            int quantity = 0;
            double subTotal = 0;

            if (await reader.ReadAsync())
            {
                quantity = reader["quantity"] is DBNull ? 0 : (int)reader["quantity"];
                subTotal = reader["subTotal"] is DBNull ? 0 : (double)reader["subTotal"];
            }
            return (quantity, subTotal);
        }
        public async Task<IEnumerable<OrderedItem>> GetQuantity(int customerOrderID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""quantity"", ""itemID""
                                FROM 
                                    ""Sales.POS.OrderItems""
                                WHERE 
                                    ""customerOrderID"" = @customerOrderID ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", customerOrderID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<OrderedItem> orderItemsProductQuantity = new List<OrderedItem>();

            while (await reader.ReadAsync())
            {
                orderItemsProductQuantity.Add(new OrderedItem
                {
                    ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"],
                    Quantity = reader["quantity"] is DBNull ? 0 : (int)reader["quantity"]
                });
            }
            return orderItemsProductQuantity;
        }

        public async Task<IEnumerable<CustomerOrder>> GetUserPendingOrdersAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""billIsPrinted"",  ""customerOrderID"", ""dateTimeCreated""
                    FROM 
                        ""Inventory.Inventory.CustomerOrders""
                    WHERE 
                        ""isOrderPaid"" = @isOrderPaid ";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);


            command.Parameters.AddWithValue("@isOrderPaid", 0);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            List<CustomerOrder> pendingOrders = new List<CustomerOrder>();

            while (await reader.ReadAsync())
            {
                pendingOrders.Add(new CustomerOrder
                {
                    BillIsPrinted = reader["billIsPrinted"] is DBNull ? 0 : (int)reader["billIsPrinted"],
                    CustomerOrderID = reader["customerOrderID"] is DBNull ? 0 : (int)reader["customerOrderID"],
                    DateTimeCreated = reader["dateTimeCreated"] is DBNull ? DateTime.MinValue : (DateTime)reader["dateTimeCreated"],
                });
            }
            return pendingOrders;
        }
        public async Task<bool> IsOrderPaid(int customerOrderID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                    SELECT 
                        ""isOrderPaid""
                    FROM 
                        ""Inventory.Inventory.CustomerOrders""
                    WHERE 
                        ""customerOrderID"" = @customerOrderID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", customerOrderID);

            await connection.OpenAsync();

            using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            int isOrderPaid = 0;

            if (await reader.ReadAsync())
            {
                isOrderPaid = reader["isOrderPaid"] is DBNull ? 0 : (int)reader["isOrderPaid"];
            }
            return isOrderPaid > 0;
        }

        public async Task<bool> DoesOrderExist(int orderID)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = @"SELECT COUNT(*) 
                                    FROM 
                                        ""Inventory.Inventory.CustomerOrders""
                                    WHERE 
                                        ""customerOrderID"" = @customerOrderID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", orderID);

            await connection.OpenAsync();

            var count = await command.ExecuteScalarAsync();

            return Convert.ToInt32(count) > 0;
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

        public async Task<IEnumerable<Item>> GetAllItemsOnPOSAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""barcode"", ""image"", ""itemName"", ""itemCategoryID"", ""unitPrice"", ""itemID""
                                FROM 
                                    ""Inventory.Inventory.Items""
                                WHERE 
                                ""showInPOS"" = 1";

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
                    Barcode = reader["barcode"] is DBNull ? string.Empty : (string)reader["barcode"],
                    Image = reader["image"] is DBNull ? string.Empty : (string)reader["image"],
                    ItemName = reader["itemName"] is DBNull ? string.Empty : (string)reader["itemName"],
                    ItemCategoryID = reader["itemCategoryID"] is DBNull ? 0 : (int)reader["itemCategoryID"],
                    UnitPrice = reader["unitPrice"] is DBNull ? 0 : (double)reader["unitPrice"],
                    ItemID = reader["itemID"] is DBNull ? 0 : (int)reader["itemID"]
                });
            }
            return items;
        }


        public async Task<IEnumerable<ItemCategory>> GetAllItemCategoriesAsync()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                                SELECT 
                                    ""description"", ""itemCategoryID"", ""itemCategoryName""
                                FROM 
                                    ""Inventory.Inventory.ItemCategories""";

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

        public async Task<bool> AddPosItemQuantityAsync(CustomerOrder customerOrder, int quantity, double subTotal, double unitPrice)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"UPDATE 
                                    ""Sales.POS.OrderItems""
                                SET
                                    ""quantity"" = @newQuantity,
                                    ""subTotal"" = @subTotal
                                WHERE
                                    ""customerOrderID"" = @customerOrderID
                                AND
                                    ""itemID"" = @itemID";

            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            int newQuantity = quantity + 1;
            double newsubTotal = subTotal + unitPrice;

            command.Parameters.AddWithValue("@newQuantity", newQuantity);
            command.Parameters.AddWithValue("@subTotal", newsubTotal);
            command.Parameters.AddWithValue("@customerOrderID", customerOrder.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", customerOrder.CustomerOrderItemID);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }


        public async Task<bool> AddPosItemToOrderAsync(CustomerOrder customerOrder, double unitPrice)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            string commandText = $@"
                INSERT INTO 
                    ""Sales.POS.OrderItems""
                    (""customerOrderID"", ""itemID"", ""quantity"",""subTotal"")
                VALUES                       
                    (@customerOrderID, @itemID, @quantity, @subTotal)";
            using NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            command.Parameters.AddWithValue("@customerOrderID", customerOrder.CustomerOrderID);
            command.Parameters.AddWithValue("@itemID", customerOrder.CustomerOrderItemID);
            command.Parameters.AddWithValue("@quantity", 1);
            command.Parameters.AddWithValue("@subTotal", unitPrice);

            await connection.OpenAsync();

            int count = await command.ExecuteNonQueryAsync();

            return count > 0;
        }
    }
}
