using PointOfSaleSystem.Data.Inventory;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface ITaxRepository
    {
        //Vat Tax
        Task<VatType?> CreateVATTypeAsync(VatType vatType);
        Task<VatType?> UpdateVATTypeAsync(VatType vatType);
        Task<IEnumerable<VatType>> GetAllVATTypesAsync();
        Task<VatType?> GetVATTypeDetailsAsync(int vatTypeID);
        Task<bool> DeleteVATTypeAsync(int vatTypeID);
        Task<bool> DoesVATTypeExistAsync(int vatTypeID);

        //Other Tax
        Task<OtherTax?> CreateOtherTaxAsync(OtherTax otherTax);
        Task<OtherTax?> UpdateOtherTaxAsync(OtherTax otherTax);
        Task<IEnumerable<OtherTax>> GetAllOtherTaxesAsync();
        Task<OtherTax?> GetOtherTaxDetailsAsync(int otherTaxID);
        Task<bool> DeleteOtherTaxAsync(int otherTaxID);
        Task<bool> DoesOtherTaxExistAsync(int otherTaxID);

        //Tax config
        Task<IEnumerable<SubAccount>> GetAllLiabilitySubAccountsAsync();
    }
}
