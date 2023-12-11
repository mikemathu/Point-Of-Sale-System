using PointOfSaleSystem.Data.Inventory;

namespace PointOfSaleSystem.Service.Interfaces.Inventory
{
    public interface IItemRepository
    {
        Task<Item?> CreateItemAsync(Item itemModel);
        Task<Item?> SearchItemsAsync(string productName);
        Task<IEnumerable<Item>> FilterItemsAsync(FilterItem filterFlag);
        Task<Item?> GetItemDetailsAsync(int itemID);
        Task<Item> UpdateItemAsync(Item product);
        Task<Item> UpdateItemCreatingJVAsync(Item product, Item itemDetails, int userID, int fiscalPeriodID);
        Task<bool> DeleteItemAsync(int itemID);
        Task<bool> DoesItemExist(int itemID);
    }
}
