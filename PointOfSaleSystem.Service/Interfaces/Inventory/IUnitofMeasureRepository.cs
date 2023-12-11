using PointOfSaleSystem.Data.Inventory;

namespace PointOfSaleSystem.Service.Interfaces.Inventory
{
    public interface IUnitofMeasureRepository
    {
        Task<UnitOfMeasure?> CreateUnitOfMeasureAsync(UnitOfMeasure unitOfMeasureDto);
        Task<UnitOfMeasure?> UpdateUnitOfMeasureAsync(UnitOfMeasure unitOfMeasureDto);
        Task<UnitOfMeasure?> GetUnitOfMeasureDetailsAsync(int unitOfMeasureID);
        Task<bool> DeleteUnitOfMeasureAsync(int unitOfMeasureID);
        Task<bool> DoesUnitOfMeasureExist(int unitOfMeasureID);
    }
}
