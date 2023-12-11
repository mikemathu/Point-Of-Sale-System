using PointOfSaleSystem.Data.Sales;

namespace PointOfSaleSystem.Service.Interfaces.Sales
{
    public interface IPaymentRepository
    {
        //Payment
        Task<IEnumerable<PaymentMethod>> GetAllPaymentModesAsync();
        Task<bool> ReceivePosPaymentsAsync(Payment payment, IEnumerable<OrderedItem> orderItemsProductQuantity, int userID, int fiscalPeriodID);
        Task<double?> CalculateTotalOrderAmount(int customerOrderID);
    }
}
