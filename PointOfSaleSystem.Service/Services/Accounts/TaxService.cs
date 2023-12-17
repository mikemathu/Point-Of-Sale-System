using AutoMapper;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class TaxService
    {

        private readonly ITaxRepository _taxRepository;
        private readonly IMapper _mapper;
        public TaxService(ITaxRepository taxRepository, IMapper mapper)
        {
            _taxRepository = taxRepository;
            _mapper = mapper;
        }
        private async Task ValidateVatTypeId(int vatTypeID)
        {
            if (vatTypeID <= 0)
            {
                throw new ArgumentException("Invalid Vat Type Id. It must be a positive integer.");
            }
            bool doesVATTypeExist = await _taxRepository.DoesVATTypeExistAsync(vatTypeID);
            if (!doesVATTypeExist)
            {
                throw new ItemNotFoundException($"Vat Type with Id {vatTypeID} not found.");
            }
        }
        private async Task ValidateOtherTaxId(int otherTaxID)
        {
            if (otherTaxID <= 0)
            {
                throw new ArgumentException("Invalid Vat Type Id. It must be a positive integer.");
            }
            bool doesOtherTaxExist = await _taxRepository.DoesOtherTaxExistAsync(otherTaxID);
            if (!doesOtherTaxExist)
            {
                throw new ItemNotFoundException($"Other Tax with Id {otherTaxID} not found.");
            }
        }
        public async Task<VatTypeDto> CreateUpdateVATTypeAsync(VatTypeDto vatTypeDto)
        {
            VatType? vatType = null;

            if (vatTypeDto.VATTypeID == 0)//Create
            {
                vatType = await _taxRepository.CreateVATTypeAsync(_mapper.Map<VatType>(vatTypeDto));
            }
            else//update
            {
                vatType = await _taxRepository.UpdateVATTypeAsync(_mapper.Map<VatType>(vatTypeDto));
            }
            if (vatType == null)
            {
                throw new ActionFailedException("Could not Create/Update Item Category.");
            }
            return _mapper.Map<VatTypeDto>(vatType);
        }
        public async Task<IEnumerable<VatTypeDto>> GetAllVATTypesAsync()
        {
            IEnumerable<VatType> vatTypes = await _taxRepository.GetAllVATTypesAsync();
            if (!vatTypes.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<VatTypeDto>>(vatTypes);
        }
        public async Task<VatTypeDto> GetVATTypeDetails(int vatTypeID)
        {
            await ValidateVatTypeId(vatTypeID);
            VatType? vatTypeDetails = await _taxRepository.GetVATTypeDetailsAsync(vatTypeID);
            if (vatTypeDetails == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<VatTypeDto>(vatTypeDetails);
        }
        public async Task DeleteVATTypeAsync(int vatTypeID)
        {
            await ValidateVatTypeId(vatTypeID);
            bool isVatTypeDeleted = await _taxRepository.DeleteVATTypeAsync(vatTypeID);
            if (!isVatTypeDeleted)
            {
                throw new ActionFailedException("Could not Vat Type. Try again later.");
            }
        }

        //Other Taxes
        public async Task<OtherTaxDto> CreateUpdateOtherTaxAsync(OtherTaxDto otherTaxDto)
        {
            OtherTax? otherTax = null;

            if (otherTaxDto.OtherTaxID == 0)//Create
            {
                otherTax = await _taxRepository.CreateOtherTaxAsync(_mapper.Map<OtherTax>(otherTaxDto));
            }
            else//update
            {
                otherTax = await _taxRepository.UpdateOtherTaxAsync(_mapper.Map<OtherTax>(otherTaxDto));
            }
            if (otherTax == null)
            {
                throw new ActionFailedException("Could not Create/Update Item Category.");
            }
            return _mapper.Map<OtherTaxDto>(otherTax);
        }
        public async Task<IEnumerable<OtherTaxDto>> GetAllOtherTaxesAsync()
        {
            IEnumerable<OtherTax> otherTax = await _taxRepository.GetAllOtherTaxesAsync();
            if (!otherTax.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<OtherTaxDto>>(otherTax);
        }
        public async Task<OtherTaxDto> GetOtherTaxDetailsAsync(int otherTaxID)
        {
            await ValidateOtherTaxId(otherTaxID);
            OtherTax? otherTaxDetails = await _taxRepository.GetOtherTaxDetailsAsync(otherTaxID);
            if (otherTaxDetails == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<OtherTaxDto>(otherTaxDetails);
        }
        public async Task DeleteOtherTaxAsync(int otherTaxID)
        {
            await ValidateOtherTaxId(otherTaxID);
            bool isOtherTaxDeleted = await _taxRepository.DeleteOtherTaxAsync(otherTaxID);
            if (!isOtherTaxDeleted)
            {
                throw new ActionFailedException("Could not delete Other Tax. Try again later.");
            }
        }

        //Config
        public async Task<IEnumerable<SubAccountDto>> GetAllLiabilitySubAccountsAsync()
        {
            IEnumerable<SubAccount> subAccounts = await _taxRepository.GetAllLiabilitySubAccountsAsync();
            if (!subAccounts.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<SubAccountDto>>(subAccounts);
        }
    }
}