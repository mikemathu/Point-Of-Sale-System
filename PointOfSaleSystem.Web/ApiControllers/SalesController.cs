using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Dtos.Sales;
using PointOfSaleSystem.Service.Services.Sales;
using PointOfSaleSystem.Web.Filters;

namespace PointOfSaleSystem.Web.ApiControllers
{
    [Route("Sales")]
    [ApiController]
    [Authorize]
    [ValidateModel, HandleException]
    public class SalesController : ControllerBase
    {
        private readonly OrderService _customerOrderService;
        private readonly PaymentService _paymentMethodService;
        public SalesController(
            OrderService customerOrderService,
            PaymentService paymentMethodService)
        {
            _customerOrderService = customerOrderService;
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet("CreatePosOrder")]
        public async Task<IActionResult> CreatePosOrder()
        {
            await _customerOrderService.CreatePosOrderAsync();
            return Ok(new { message = "Order Created Successfully." });
        }

        [HttpGet("GetUserPendingOrders")]
        public async Task<IActionResult> GetUserPendingOrders()
        {
            IEnumerable<CustomerOrderDto> pendingOrders = await _customerOrderService.GetUserPendingOrdersAsync();
            return Ok(pendingOrders);
        }

        [HttpPost("RemoveOrder")]
        public async Task<IActionResult> RemoveOrder([FromBody] int orderID)
        {
            await _customerOrderService.RemoveOrderAsync(orderID);
            return Ok(new { Responce = "Order Removed Deleted Successfully." });
        }

        [HttpPost("AddPosItemToOrder")]
        public async Task<IActionResult> AddPosItemToOrder(CustomerOrderDto customerOrderDto)
        {
            await _customerOrderService.AddPosItemToOrderAsync(customerOrderDto);
            return Ok(new { Responce = "Added Successfully." });
        }

        [HttpPost("GetPosOrderItems")]
        public async Task<IActionResult> GetPosOrderItems([FromBody] int orderID)
        {
            IEnumerable<OrderedItemDto> customerOrders = await _customerOrderService.GetPosOrderItemAsync(orderID);
            return Ok(customerOrders);
        }

        [HttpGet("GetAllPaymentModes")]
        public async Task<IActionResult> GetAllPaymentModes()
        {
            IEnumerable<PaymentMethodDto> paymentMethods = await _paymentMethodService.GetAllPaymentModesAsync();
            return Ok(paymentMethods);
        }

        [HttpPost("ReceivePosPayments")]
        public async Task<IActionResult> ReceivePosPayments(PaymentDto paymentDto)
        {
            await _paymentMethodService.ReceivePosPaymentsAsync(paymentDto);
            return Ok(new { message = "Payment Received Sussessfully." });
        }

        [HttpPost("UpdatePosItemQuantity")]
        public async Task<IActionResult> UpdatePosItemQuantity(UpdateOrderedItemDto orderItemDto)
        {
            await _customerOrderService.UpdatePosItemQuantityAsync(orderItemDto);
            return Ok(new { message = "Pos Item Updated Sussessfully." });
        }

        [HttpGet("GetAllItemsOnPOS")]
        public async Task<IActionResult> GetAllItemsOnPOS()
        {
            (IEnumerable<ItemDto>, IEnumerable<ItemCategoryDto>) result = await _customerOrderService.GetAllItemOnPosAndItemCategoriesAsync();
            return Ok(new { Items = result.Item1, Categories = result.Item2 });
        }

        [HttpPost("PrintSalesOrder")]
        public IActionResult PrintSalesOrder([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("CheckPosKeypadMode")]
        public IActionResult CheckPosKeypadMode([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("UpdatePosItemDiscount")]
        public IActionResult UpdatePosItemDiscount([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("UpdatePosItemPrice")]
        public IActionResult UpdatePosItemPrice([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("GetActiveCustomers")]
        public IActionResult GetActiveCustomers([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("ModifyCustomerOrder")]
        public IActionResult ModifyCustomerOrder([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("MergePosOrders")]
        public IActionResult MergePosOrders([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("SplitPosOrder")]
        public IActionResult SplitPosOrder([FromBody] int mode)
        {
            return Ok(mode);
        }

        [HttpPost("QueryMpesaC2BPayment")]
        public IActionResult QueryMpesaC2BPayment([FromBody] int mode)
        {
            return Ok(mode);
        }
    }
}