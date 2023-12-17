using AutoMapper;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Interfaces.Inventory;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Inventory
{
    public class InventoryConfigService
    {
        private readonly InventoryConfig _inventoryConfig;
        private readonly IMapper _mapper;
        public InventoryConfigService(InventoryConfig inventoryConfig, IMapper mapper)
        {
            _inventoryConfig = inventoryConfig;
            _mapper = mapper;
        }
        private async Task ValidateItemCategoryId(int itemCategoryID)
        {
            if (itemCategoryID <= 0)
            {
                throw new ArgumentException("Invalid Item Categoty Id. It must be a positive integer.");
            }
            bool doesItemCategoryExist = await _inventoryConfig.DoesItemCategoryExist(itemCategoryID);
            if (!doesItemCategoryExist)
            {
                throw new ItemNotFoundException($"Item Category with Id {itemCategoryID} not found.");
            }
        }
        private async Task ValidateItemClassId(int itemClassID)
        {
            if (itemClassID <= 0)
            {
                throw new ArgumentException("Invalid Item Class Id. It must be a positive integer.");
            }
            bool doesItemClassExist = await _inventoryConfig.DoesItemClassExist(itemClassID);
            if (!doesItemClassExist)
            {
                throw new ItemNotFoundException($"Item Class with Id {itemClassID} not found.");
            }
        }

        //ItemCategory
        public async Task<ItemCategoryDto> CreateUpdateItemCategoryAsync(ItemCategoryDto itemCategoryDto)
        {
            ItemCategory? itemCategory = null;

            if (itemCategoryDto.ItemCategoryID == 0)//Create
            {
                itemCategory = await _inventoryConfig.CreateItemCategoryAsync(_mapper.Map<ItemCategory>(itemCategoryDto));
            }
            else//update
            {
                itemCategory = await _inventoryConfig.UpdateItemCategoryAsync(_mapper.Map<ItemCategory>(itemCategoryDto));
            }
            if (itemCategory == null)
            {
                throw new ActionFailedException("Could not Create/Update Item Category.");
            }
            return _mapper.Map<ItemCategoryDto>(itemCategory);
        }
        public async Task<ItemCategoryDto> GetItemCategoryDetailsAsync(int itemCategoryID)
        {
            await ValidateItemCategoryId(itemCategoryID);
            ItemCategory? itemCategory = await _inventoryConfig.GetItemCategoryDetailsAsync(itemCategoryID);
            if (itemCategory == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<ItemCategoryDto>(itemCategory);
        }
        public async Task DeleteItemCategoryAsync(int itemCategoryID)
        {
            await ValidateItemCategoryId(itemCategoryID);
            bool isItemCategoryDeleted = await _inventoryConfig.DeleteItemCategoryAsync(itemCategoryID);
            if (!isItemCategoryDeleted)
            {
                throw new ActionFailedException("Could not delete Item Category. Try again later.");
            }
        }
        //ItemClass  
        public async Task<ItemClassDto> CreateUpdateItemClassAsync(ItemClassDto itemClassDto)
        {
            ItemClass? itemClass = null;

            if (itemClassDto.ItemClassID == 0)//Create
            {
                itemClass = await _inventoryConfig.CreateItemClassAsync(_mapper.Map<ItemClass>(itemClassDto));
            }
            else//update
            {
                itemClass = await _inventoryConfig.UpdateItemClassAsync(_mapper.Map<ItemClass>(itemClassDto));
            }
            if (itemClass == null)
            {
                throw new ActionFailedException("Could not Create/Update Item Category.");
            }
            return _mapper.Map<ItemClassDto>(itemClass);
        }
        public async Task<ItemClassDto> GetItemClassDetailsAsync(int itemClassID)
        {
            await ValidateItemClassId(itemClassID);
            ItemClass? itemClass = await _inventoryConfig.GetItemClassDetailsAsync(itemClassID);
            if (itemClass == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<ItemClassDto>(itemClass);
        }
        public async Task DeleteItemClassAsync(int itemClassID)
        {
            await ValidateItemClassId(itemClassID);
            bool isItemClassDeleted = await _inventoryConfig.DeleteItemClassAsync(itemClassID);
            if (!isItemClassDeleted)
            {
                throw new ActionFailedException("Could not delete Item Class. Try again later.");
            }
        }
    }
}
