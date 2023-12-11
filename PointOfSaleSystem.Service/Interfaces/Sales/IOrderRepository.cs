using PointOfSaleSystem.Data.Inventory;

namespace PointOfSaleSystem.Service.Interfaces.Sales
{
    public interface IOrderRepository
    {
        //Order
        Task<bool> CreatePosOrderAsync();
        Task<bool> AddPosItemQuantityAsync(CustomerOrder customerOrder, int quantity, double subTotal, double unitPrice);
        Task<bool> AddPosItemToOrderAsync(CustomerOrder customerOrder, double unitPrice);
        Task<bool> RemoveOrderAsync(int customerOrderID);
        Task<IEnumerable<CustomerOrder>> GetUserPendingOrdersAsync();
        Task<bool> DoesOrderExist(int customerOrderID);
        Task<bool> IsOrderPaid(int customerOrderID);
        Task<(int Quantity, double SubTotal)> IsItemAvailable(CustomerOrder customerOrder);

        //Item
        Task<IEnumerable<Item>> GetAllItemsOnPOSAsync();
        Task<IEnumerable<ItemCategory>> GetAllItemCategoriesAsync();
        Task<IEnumerable<OrderedItem>> GetPosOrderItemsAsync(int customerOrderID);
        Task<double> GetItemUnitPriceAsync(int itemID);
        Task<int> CheckQuantity(OrderedItem orderItem);
        Task<IEnumerable<OrderedItem>> GetQuantity(int customerOrderID);
        Task<bool> UpdatePosItemQuantity(OrderedItem orderItem, double unitPrice);
        Task<bool> RemoveItem(OrderedItem orderItem);
        Task<bool> DoesItemExist(int itemID);
    }
}
