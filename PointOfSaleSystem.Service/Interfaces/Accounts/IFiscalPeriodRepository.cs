using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface IFiscalPeriodRepository
    {
        Task<FiscalPeriod?> CreateFiscalPeriodAsync(FiscalPeriod fiscalPeriod);
        void AddFiscalPeriodAsync(FiscalPeriod fiscalPeriodID);
        Task<IEnumerable<FiscalPeriod>> GetAllFiscalPeriodsAsync();
        Task<int?> GetActiveFiscalPeriodID();
        Task<FiscalPeriod?> GetFiscalPeriodDetailsAsync(int fiscalPeriodID);
        Task<IEnumerable<FiscalPeriod>> GetAllActiveFiscalPeriodsAsync();
        Task<IEnumerable<FiscalPeriod>> GetAllInActiveFiscalPeriodsAsync();
        Task<bool> IsFiscalPeriodActiveAsync(int fiscalPeriodID);
        Task<bool> IsFiscalPeriodOpenAsync(int fiscalPeriodID);
        Task<bool> IsFiscalPeriodRangeOverlapAsync(FiscalPeriod fiscalPeriod);
        Task<FiscalPeriod?> UpdateFiscalPeriodAsync(FiscalPeriod fiscalPeriod);
        Task<FiscalPeriod?> CloseFiscalPeriodAsync(int fiscalPeriodID);
        Task<bool> DeleteFiscalPeriodAsync(int fiscalPeriodID);
        Task<bool> DoesFiscalPeriodExist(int fiscalPeriodID);

    }
}
