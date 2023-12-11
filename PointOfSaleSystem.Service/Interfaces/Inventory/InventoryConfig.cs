using PointOfSaleSystem.Data.Inventory;

namespace PointOfSaleSystem.Service.Interfaces.Inventory
{
    public interface InventoryConfig
    {
        //ItemCategory
        Task<ItemCategory?> CreateItemCategoryAsync(ItemCategory itemCategory);
        Task<ItemCategory?> UpdateItemCategoryAsync(ItemCategory itemCategory);
        Task<ItemCategory?> GetItemCategoryDetailsAsync(int itemCategoryID);
        Task<bool> DeleteItemCategoryAsync(int itemCategoryID);
        Task<bool> DoesItemCategoryExist(int itemCategoryID);

        //ItemClass
        Task<ItemClass?> CreateItemClassAsync(ItemClass itemClass);
        Task<ItemClass?> UpdateItemClassAsync(ItemClass itemClass);
        Task<ItemClass?> GetItemClassDetailsAsync(int itemClassID);
        Task<bool> DeleteItemClassAsync(int itemClassID);
        Task<bool> DoesItemClassExist(int itemClassID);
    }
}
