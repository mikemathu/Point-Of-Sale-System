using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Services.Accounts;
using PointOfSaleSystem.Service.Services.Inventory;
using PointOfSaleSystem.Web.Filters;

namespace PointOfSaleSystem.Web.ApiControllers
{
    [Route("Inventory")]
    [ApiController]
    [Authorize]
    [ValidateModel, HandleException]
    public class InventoryController : ControllerBase
    {
        private readonly ItemService _itemService;
        private readonly ItemInfoService _itemInfoService;
        private readonly SubAccountService _subAccountService;
        private readonly InventoryConfigService _inventoryConfigService;
        private readonly UnitofMeasureService _unitofMeasureService;
        public InventoryController(
            ItemService inventoryService,
            ItemInfoService itemInfoService,
            SubAccountService subAccountService,
            InventoryConfigService inventoryConfigService,
            UnitofMeasureService unitofMeasureService)
        {
            _itemService = inventoryService;
            _itemInfoService = itemInfoService;
            _subAccountService = subAccountService;
            _inventoryConfigService = inventoryConfigService;
            _unitofMeasureService = unitofMeasureService;
        }

        /// <summary>
        /// Items/Inventory
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateItem")]
        [Authorize("CanManageInventory")]
        public async Task<IActionResult> CreateUpdateItem(ItemDto itemDto)
        {
            ItemDto item = await _itemService.CreateUpdateItemAsync(itemDto);
            return Ok(item);
        }

        [HttpPost("FilterItems")]
        public async Task<IActionResult> FilterItems(FilterItemDto filterFlag)
        {
            IEnumerable<ItemDto> items = await _itemService.FilterItemsAsync(filterFlag);
            return Ok(items);
        }

        [HttpPost("SearchItems")]
        public async Task<IActionResult> SearchItems([FromBody] string itemName)
        {
            ItemDto item = await _itemService.SearchItemsAsync(itemName);
            return Ok(item);
        }

        [HttpPost("GetItemDetails")]
        public async Task<IActionResult> GetItemDetails([FromBody] int itemID)
        {
            ItemDto itemDetails = await _itemService.GetItemDetailsAsync(itemID);
            return Ok(itemDetails);
        }

        [HttpPost("DeleteItem")]
        [Authorize("CanManageInventory")]
        public async Task<IActionResult> DeleteItem([FromBody] int itemID)
        {
            await _itemService.DeleteItemAsync(itemID);
            return Ok(new { Responce = "Item Deleted Successfully." });
        }

        /// <summary>
        /// Inventory Info
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllUnitOfMeasures")]
        public async Task<IActionResult> GetAllUnitOfMeasures()
        {
            IEnumerable<UnitOfMeasureDto> unitsOfMeasures = await _itemInfoService.GetAllUnitOfMeasures();
            return Ok(unitsOfMeasures);
        }

        [HttpGet("GetAllItemClasses")]
        public async Task<IActionResult> GetAllItemClasses()
        {
            IEnumerable<ItemClassDto> itemClasses = await _itemInfoService.GetAllItemClasses();
            return Ok(itemClasses);
        }

        [HttpGet("GetAllItemCategories")]
        public async Task<IActionResult> GetAllItemCategories()
        {
            IEnumerable<ItemCategoryDto> itemCategories = await _itemInfoService.GetAllItemCategoriesAsync();
            return Ok(itemCategories);
        }

        [HttpGet("GetAllOtherTaxes")]
        public async Task<IActionResult> GetAllOtherTaxes()
        {
            IEnumerable<OtherTaxDto> otherTaxes = await _itemInfoService.GetAllOtherTaxesAsync();
            return Ok(otherTaxes);
        }

        [HttpGet("GetAllVATTypes")]
        public async Task<IActionResult> GetAllVATTypes()
        {
            IEnumerable<VatTypeDto> vatTypes = await _itemInfoService.GetAllVATTypesAsync();
            return Ok(vatTypes);
        }

        [HttpGet("GetInventorySubAccounts")]
        public async Task<IActionResult> GetInventorySubAccounts()
        {
            IEnumerable<SubAccountDto> inventorySubAccounts = await _subAccountService.GetInventorySubAccountAsync();
            return Ok(inventorySubAccounts);
        }

        [HttpGet("GetCostOfSalesSubAccounts")]
        public async Task<IActionResult> GetCostOfSalesSubAccounts()
        {
            IEnumerable<SubAccountDto> costOfSaleSubAccount = await _subAccountService.GetCostOfSalesSubAccountsAsync();
            return Ok(costOfSaleSubAccount);
        }

        [HttpGet("GetIncomeSubAccounts")]
        public async Task<IActionResult> GetIncomeSubAccounts()
        {
            IEnumerable<SubAccountDto> incomeSubAccounts = await _subAccountService.GetIncomeSubAccountsAsync();
            return Ok(incomeSubAccounts);
        }

        /// <summary>
        /// Configurations
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>\

        //Item Category
        [HttpPost("CreateUpdateItemCategory")]
        [Authorize("CanManageInventory")]
        public async Task<IActionResult> CreateUpdateItemCategory(ItemCategoryDto itemCategoryDto)
        {
            ItemCategoryDto itemCategory = await _inventoryConfigService.CreateUpdateItemCategoryAsync(itemCategoryDto);
            return Ok(itemCategory);
        }
        [HttpPost("GetItemCategoryDetails")]
        public async Task<IActionResult> GetItemCategoryDetails([FromBody] int itemCategoryID)
        {
            ItemCategoryDto itemCategoryDetails = await _inventoryConfigService.GetItemCategoryDetailsAsync(itemCategoryID);
            return Ok(itemCategoryDetails);
        }
        [HttpPost("DeleteItemCategory")]
        [Authorize("CanManageInventory")]
        public async Task<IActionResult> DeleteItemCategory([FromBody] int itemCategoryID)
        {
            await _inventoryConfigService.DeleteItemCategoryAsync(itemCategoryID);
            return Ok(new { Response = "Item Category Deleted Successfully." });
        }

        //Item Class
        [HttpPost("CreateUpdateItemClass")]
        public async Task<IActionResult> CreateUpdateItemClass(ItemClassDto itemClassDto)
        {
            ItemClassDto itemClass = await _inventoryConfigService.CreateUpdateItemClassAsync(itemClassDto);
            return Ok(itemClass);
        }
        [HttpPost("GetItemClassDetails")]
        public async Task<IActionResult> GetItemClassDetails([FromBody] int itemClassID)
        {
            ItemClassDto itemClassDetails = await _inventoryConfigService.GetItemClassDetailsAsync(itemClassID);
            return Ok(itemClassDetails);
        }
        [HttpPost("DeleteItemClass")]
        [Authorize("CanManageInventory")]
        public async Task<IActionResult> DeleteItemClass([FromBody] int itemClassID)
        {
            await _inventoryConfigService.DeleteItemClassAsync(itemClassID);
            return Ok(new { Response = "Item Class Deleted Successfully." });
        }


        /// <summary>
        /// Unit of Measures
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// 
        [HttpPost("CreateUpdateUnitOfMeasure")]
        [Authorize("CanManageUnitOfMeasures")]
        public async Task<IActionResult> CreateUpdateUnitOfMeasure(UnitOfMeasureDto unitOfMeasureDto)
        {
            UnitOfMeasureDto unitOfMeasure = await _unitofMeasureService.CreateUpdateUnitOfMeasureAsync(unitOfMeasureDto);
            return Ok(unitOfMeasure);
        }
        [HttpPost("GetUnitOfMeasureDetails")]
        public async Task<IActionResult> GetUnitOfMeasureDetails([FromBody] int unitOfMeasureID)
        {
            UnitOfMeasureDto unitOfMeasure = await _unitofMeasureService.GetUnitOfMeasureDetailsAsync(unitOfMeasureID);
            return Ok(unitOfMeasure);
        }

        [HttpPost("DeleteUnitOfMeasure")]
        [Authorize("CanManageUnitOfMeasures")]
        public async Task<IActionResult> DeleteUnitOfMeasure([FromBody] int unitOfMeasureID)
        {
            await _unitofMeasureService.DeleteUnitOfMeasureAsync(unitOfMeasureID);
            return Ok(new { Response = "Unit of Measure Deleted Successfully." });
        }
    }
}