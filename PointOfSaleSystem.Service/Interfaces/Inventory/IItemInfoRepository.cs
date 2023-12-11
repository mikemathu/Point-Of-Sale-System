using PointOfSaleSystem.Data.Inventory;

namespace PointOfSaleSystem.Service.Interfaces.Inventory
{
    public interface IItemInfoRepository
    {
        Task<IEnumerable<OtherTax>> GetAllOtherTaxesAsync();
        Task<IEnumerable<ItemCategory>> GetAllItemCategoriesAsync();
        Task<IEnumerable<ItemClass>> GetAllItemClasses();
        Task<IEnumerable<UnitOfMeasure>> GetAllUnitOfMeasures();
        Task<IEnumerable<VatType>> GetAllVATTypesAsync();
    }
}
