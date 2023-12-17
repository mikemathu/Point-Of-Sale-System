using AutoMapper;
using Microsoft.AspNetCore.Http;
using PointOfSaleSystem.Data.Sales;
using PointOfSaleSystem.Service.Dtos.Sales;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Interfaces.Sales;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Sales
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentMethodRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public PaymentService(
            IPaymentRepository customerOrderConfigRepository,
            IOrderRepository orderRepository,
            IFiscalPeriodRepository fiscalPeriodRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _paymentMethodRepository = customerOrderConfigRepository;
            _orderRepository = orderRepository;
            _fiscalPeriodRepository = fiscalPeriodRepository;
            _httpContextAccessor = httpContextAccessor;
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
                throw new ItemNotFoundException($"Order with Id {orderID} not found.");
            }
        }
        private async Task IsOrderPaid(int customerOrderID)
        {
            bool isOrderPaid = await _orderRepository.IsOrderPaid(customerOrderID);
            if (isOrderPaid)
            {
                throw new ActionFailedException("This order is already paid for.");
            }
        }
        private void IsPaymentValidValid(PaymentDto paymentDto)
        {
            double tenderedAmountTotal = 0.0;

            foreach (var paymentItem in paymentDto.PosPaymentItems)
            {
                tenderedAmountTotal = tenderedAmountTotal + paymentItem.AmountTendered;
            }

            for (int i = 0; i < paymentDto.PosPaymentItems.Length; i++)
            {
                if (tenderedAmountTotal < paymentDto.PosPaymentItems[i].AmountDue)
                {
                    throw new ActionFailedException("Amount tendered is less than the amount due.");
                }
                double changeAmount = 0.0;
                double change = paymentDto.PosPaymentItems[i].AmountTendered - paymentDto.PosPaymentItems[i].AmountDue;
                if (change < 0)
                {
                    changeAmount = 0.0;
                }
                else
                {
                    changeAmount = change;
                }

                if (paymentDto.PosPaymentItems[i].ChangeAmount != changeAmount)
                {
                    throw new ActionFailedException("Change Amount is Incorrect");
                }
            }
        }
        private async Task ConfirmAmountDueMatch(PaymentDto paymentDto)
        {
            double? totalAmountDue = await _paymentMethodRepository.CalculateTotalOrderAmount(paymentDto.CustomerOrderID);

            if (totalAmountDue == null)
            {
                throw new ActionFailedException("Item(s) not found");
            }

            for (int i = 0; i < 1; i++)
            {
                if (totalAmountDue != paymentDto.PosPaymentItems[0].AmountDue)
                {
                    throw new ActionFailedException("Amount Due is incorrect.");
                }
            }
        }
        private async Task ValidateOrder(PaymentDto paymentDto)
        {
            await IsOrderIdValid(paymentDto.CustomerOrderID);
            await IsOrderPaid(paymentDto.CustomerOrderID);
            IsPaymentValidValid(paymentDto);
            await ConfirmAmountDueMatch(paymentDto);
        }

        private async Task<int> GetActiveFiscalPeriodID()
        {
            int? fiscalPeriodID = await _fiscalPeriodRepository.GetActiveFiscalPeriodID();
            if (fiscalPeriodID == null)
            {
                throw new ActionFailedException("Something went wrong. Pleace try again");
            }
            return (int)fiscalPeriodID;
        }
        public async Task<IEnumerable<PaymentMethodDto>> GetAllPaymentModesAsync()
        {
            IEnumerable<PaymentMethod> paymentMethods = await _paymentMethodRepository.GetAllPaymentModesAsync();
            if (!paymentMethods.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<PaymentMethodDto>>(paymentMethods);
        }
        private int GetSysUserID()
        {
            int sysUserID = 0;

            var sysUserIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("SysUserID");
            if (sysUserIdClaim != null)
            {
                sysUserID = Convert.ToInt32(sysUserIdClaim.Value);
            }
            return sysUserID;
        }
        public async Task ReceivePosPaymentsAsync(PaymentDto paymentDto)
        {
            await ValidateOrder(paymentDto);
            IEnumerable<OrderedItem> orderItemsQuantity = await _orderRepository.GetQuantity(paymentDto.CustomerOrderID);
            int userID = GetSysUserID();
            int fiscalPeriodID = await GetActiveFiscalPeriodID();
            bool isPaymentReceived = await _paymentMethodRepository.ReceivePosPaymentsAsync(
                _mapper.Map<Payment>(paymentDto), _mapper.Map<IEnumerable<OrderedItem>>(orderItemsQuantity), userID, fiscalPeriodID);
            if (!isPaymentReceived)
            {
                throw new ActionFailedException("Payment Failed. Try again.");
            }
        }
    }
}
