using AutoMapper;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Interfaces.Inventory;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Inventory
{
    public class ItemInfoService
    {
        private readonly IItemInfoRepository itemInfoRepository;
        private readonly IMapper _mapper;
        public ItemInfoService(
            IItemInfoRepository productConfigurationRepository,
            IMapper mapper)
        {
            itemInfoRepository = productConfigurationRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ItemCategoryDto?>> GetAllItemCategoriesAsync()
        {
            IEnumerable<ItemCategory> itemCategories = await itemInfoRepository.GetAllItemCategoriesAsync();
            if (!itemCategories.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<ItemCategoryDto>>(itemCategories);
        }
        public async Task<IEnumerable<ItemClassDto?>> GetAllItemClasses()
        {
            IEnumerable<ItemClass> itemClasses = await itemInfoRepository.GetAllItemClasses();
            if (!itemClasses.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<ItemClassDto>>(itemClasses);
        }
        public async Task<IEnumerable<UnitOfMeasureDto>> GetAllUnitOfMeasures()
        {
            IEnumerable<UnitOfMeasure> unitsOfMeasure = await itemInfoRepository.GetAllUnitOfMeasures();
            if (!unitsOfMeasure.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<UnitOfMeasureDto>>(unitsOfMeasure);
        }
        public async Task<IEnumerable<OtherTaxDto>> GetAllOtherTaxesAsync()
        {
            IEnumerable<OtherTax> otherTaxes = await itemInfoRepository.GetAllOtherTaxesAsync();
            if (!otherTaxes.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<OtherTaxDto>>(otherTaxes);
        }
        public async Task<IEnumerable<VatTypeDto>> GetAllVATTypesAsync()
        {
            IEnumerable<VatType> vatTypes = await itemInfoRepository.GetAllVATTypesAsync();
            if (!vatTypes.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<VatTypeDto>>(vatTypes);
        }
    }
}