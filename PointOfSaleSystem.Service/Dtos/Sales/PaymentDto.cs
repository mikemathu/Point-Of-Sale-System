using Point_Of_Slae.Models.Sales;

namespace PointOfSaleSystem.Service.Dtos.Sales
{
    public class PaymentDto
    {
        public int CustomerOrderID { get; set; }
        public PosPaymentItems[] PosPaymentItems { get; set; }
    }
    public class PaymentMethodDto
    {
        public int PaymentMethodID { get; set; }
        public int IsDefault { get; set; }
        public string PaymentMethodName { get; set; }
        public int SubAccountID { get; set; }
        public int CanBeReceived { get; set; } = 1; //TODO: Get this from DB instead of hard coding it
    }
}
