using AutoMapper;
using Microsoft.AspNetCore.Http;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Interfaces.Inventory;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Inventory
{
    public class ItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public ItemService(
            IItemRepository itemRepository,
            IFiscalPeriodRepository fiscalPeriodRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _fiscalPeriodRepository = fiscalPeriodRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        private async Task ValidateFilterFlag(FilterItemDto filterFlag)
        {
            if (filterFlag.FilterFlag < 0 || filterFlag.FilterFlag > 6)
            {
                throw new ArgumentOutOfRangeException(filterFlag.FilterFlag.ToString(),
                    $"FilterFlag {filterFlag.FilterFlag} is beyond expected range");
            }
        }
        private async Task IsItemNameValid(string itemName)
        {
            if (itemName == null)
            {
                throw new ArgumentException("Type the item Name");
            }
        }
        private async Task ValidateItemId(int itemID)
        {
            if (itemID <= 0)
            {
                throw new ArgumentException("Invalid Item Id. It must be a positive integer.");
            }
            bool doesItemExist = await _itemRepository.DoesItemExist(itemID);
            if (!doesItemExist)
            {
                throw new ValidationRowNotFoudException($"Item with Id {itemID} not found.");
            }
        }
        private async Task<int> GetActiveFiscalPeriodID()
        {
            int? fiscalPeriodID = await _fiscalPeriodRepository.GetActiveFiscalPeriodID();
            if (fiscalPeriodID == null)
            {
                throw new FalseException("Something went wrong. Pleace try again");
            }
            return (int)fiscalPeriodID;
        }
        public async Task<ItemDto> CreateUpdateItemAsync(ItemDto itemDto)
        {
            Item? createdUpdatedItem = null;
            if (itemDto.ItemID == 0) //Create
            {
                createdUpdatedItem = await _itemRepository.CreateItemAsync(_mapper.Map<Item>(itemDto));
            }
            else//Update
            {
                Item? itemDetails = await _itemRepository.GetItemDetailsAsync(itemDto.ItemID);
                if (itemDto.TotalQuantity != itemDetails.AvailableQuantity)
                {
                    int userID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst("SysUserID").Value);
                    int fiscalPeriodID = await GetActiveFiscalPeriodID();
                    createdUpdatedItem = await _itemRepository.UpdateItemCreatingJVAsync(
                        _mapper.Map<Item>(itemDto), itemDetails, userID, fiscalPeriodID);
                }
                createdUpdatedItem = await _itemRepository.UpdateItemAsync(_mapper.Map<Item>(itemDto));
            }
            if (createdUpdatedItem == null)
            {
                throw new FalseException("Could not Create/Update Item.");
            }
            return _mapper.Map<ItemDto>(createdUpdatedItem);
        }
        public async Task<IEnumerable<ItemDto>> FilterItemsAsync(FilterItemDto filterFlag)
        {
            await ValidateFilterFlag(filterFlag);
            IEnumerable<Item> items = await _itemRepository.FilterItemsAsync(_mapper.Map<FilterItem>(filterFlag));
            if (!items.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<ItemDto>>(items);
        }
        public async Task<ItemDto> SearchItemsAsync(string itemName)
        {
            await IsItemNameValid(itemName);
            Item? item = await _itemRepository.SearchItemsAsync(itemName);
            if (item == null)
            {
                throw new NullException();
            }
            return _mapper.Map<ItemDto>(item);
        }
        public async Task<ItemDto> GetItemDetailsAsync(int itemID)
        {
            await ValidateItemId(itemID);
            Item? itemDetails = await _itemRepository.GetItemDetailsAsync(itemID);
            if (itemDetails == null)
            {
                throw new NullException();
            }
            return _mapper.Map<ItemDto>(itemDetails);
        }
        public async Task DeleteItemAsync(int itemID)
        {
            await ValidateItemId(itemID);
            bool isItemDeleted = await _itemRepository.DeleteItemAsync(itemID);
            if (!isItemDeleted)
            {
                throw new FalseException("Could not Delete Item.Try again later.");
            }
        }
    }
}