using AutoMapper;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Data.Sales;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Dtos.Sales;
using PointOfSaleSystem.Service.Interfaces.Sales;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Sales
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository customerOrderRepository, IMapper mapper)
        {
            _orderRepository = customerOrderRepository;
            _mapper = mapper;
        }
        private async Task IsOrderIdValid(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentException("Invalid Order Id. It must be a positive integer.");
            }
            bool doesOrderExist = await _orderRepository.DoesOrderExist(orderID);
            if (!doesOrderExist)
            {
                throw new ValidationRowNotFoudException($"Order with Id {orderID} not found.");
            }
        }
        private async Task IsItemIdValid(int itemID)
        {
            if (itemID <= 0)
            {
                throw new ArgumentException("Invalid Item Id. It must be a positive integer.");
            }
            bool doesItemExist = await _orderRepository.DoesItemExist(itemID);
            if (!doesItemExist)
            {
                throw new ValidationRowNotFoudException($"Item with Id {itemID} not found.");
            }

        }
        private async Task IsOrderPaid(int customerOrderID)
        {
            bool isOrderPaid = await _orderRepository.IsOrderPaid(customerOrderID);
            if (isOrderPaid)
            {
                throw new FalseException("This order is already paid for.");
            }
        }
        private async Task<IEnumerable<ItemDto>> GetAllItemsOnPOSAsync()
        {
            IEnumerable<Item> items = await _orderRepository.GetAllItemsOnPOSAsync();
            if (!items.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<ItemDto>>(items);
        }
        private async Task<IEnumerable<ItemCategoryDto>> GetAllItemCategoriesAsync()
        {
            IEnumerable<ItemCategory> itemCategories = await _orderRepository.GetAllItemCategoriesAsync();
            if (!itemCategories.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<ItemCategoryDto>>(itemCategories);
        }
        public async Task<IEnumerable<CustomerOrderDto>> GetUserPendingOrdersAsync()
        {
            IEnumerable<CustomerOrder> pendingOrders = await _orderRepository.GetUserPendingOrdersAsync();
            if (!pendingOrders.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<CustomerOrderDto>>(pendingOrders);
        }
        public async Task CreatePosOrderAsync()
        {
            bool isPosOrderCreated = await _orderRepository.CreatePosOrderAsync();
            if (!isPosOrderCreated)
            {
                throw new FalseException("Could not create Order. Try again");
            }
        }
        public async Task RemoveOrderAsync(int orderID)
        {
            await IsOrderIdValid(orderID);
            bool isOrderRemoved = await _orderRepository.RemoveOrderAsync(orderID);
            if (!isOrderRemoved)
            {
                throw new FalseException("Could not Delete Order. Try again.");
            }
        }
        public async Task AddPosItemToOrderAsync(CustomerOrderDto customerOrderDto)
        {
            await IsOrderIdValid(customerOrderDto.CustomerOrderID);
            await IsOrderPaid(customerOrderDto.CustomerOrderID);
            await IsItemIdValid(customerOrderDto.CustomerOrderItemID);
            var (quantity, subTotal) = await _orderRepository.IsItemAvailable(_mapper.Map<CustomerOrder>(customerOrderDto));
            double unitPrice = await _orderRepository.GetItemUnitPriceAsync(customerOrderDto.CustomerOrderItemID);
            bool isPosItemAddedToOrder = false;
            if (quantity > 0)
            {
                isPosItemAddedToOrder = await _orderRepository.AddPosItemQuantityAsync(
                    _mapper.Map<CustomerOrder>(customerOrderDto), quantity, subTotal, unitPrice);
            }
            else
            {
                isPosItemAddedToOrder = await _orderRepository.AddPosItemToOrderAsync(_mapper.Map<CustomerOrder>(customerOrderDto), unitPrice);
            }
            if (!isPosItemAddedToOrder)
            {
                throw new FalseException("Could not Add Item to order. Try again.");
            }
        }
        public async Task<IEnumerable<OrderedItemDto>> GetPosOrderItemAsync(int orderID)
        {
            //await IsOrderIdValid(orderID);
            IEnumerable<OrderedItem> orderItems = await _orderRepository.GetPosOrderItemsAsync(orderID);
           /* if (!orderItems.Any())
            {
                throw new NullException();
            }*/
            return _mapper.Map<IEnumerable<OrderedItemDto>>(orderItems);
        }
        public async Task<IEnumerable<OrderedItemDto>> GetQuantity(int customerOrderID)
        {
            IEnumerable<OrderedItem> orderItemQuantity = await _orderRepository.GetQuantity(customerOrderID);
            if (!orderItemQuantity.Any())
            {
                throw new FalseException("Item Not Found.");
            }
            return _mapper.Map<IEnumerable<OrderedItemDto>>(orderItemQuantity);
        }
        public async Task UpdatePosItemQuantityAsync(OrderedItemDto orderItemDto)
        {
            var checkquantity = await CheckQuantity(orderItemDto);
            if (checkquantity == 0)
            {
                bool isItemRemoved = await _orderRepository.RemoveItem(_mapper.Map<OrderedItem>(orderItemDto));
                if (!isItemRemoved)
                {
                    throw new FalseException("Error happened. Please try again.");
                }
            }
            double unitPrice = await _orderRepository.GetItemUnitPriceAsync(orderItemDto.ItemID);
            bool customerOrders = await UpdatePosItemQuantity(orderItemDto, unitPrice);
        }

        public async Task<int> CheckQuantity(OrderedItemDto orderItemDto)
        {
            return await _orderRepository.CheckQuantity(_mapper.Map<OrderedItem>(orderItemDto));
        }
        public async Task<bool> UpdatePosItemQuantity(OrderedItemDto orderItemDto, double unitPrice)
        {
            return await _orderRepository
                 .UpdatePosItemQuantity(_mapper.Map<OrderedItem>(orderItemDto), unitPrice);
        }

        public async Task<(IEnumerable<ItemDto>, IEnumerable<ItemCategoryDto>)> GetAllItemOnPosAndItemCategoriesAsync()
        {
            IEnumerable<ItemDto> items = await GetAllItemsOnPOSAsync();
            IEnumerable<ItemCategoryDto> itemCategories = await GetAllItemCategoriesAsync();
            return (items, itemCategories);
        }
    }
}