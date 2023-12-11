using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Sales
{
    public class Payment
    {
        public int CustomerOrderID { get; set; }
        public PosPaymentItems[] PosPaymentItems { get; set; }
    }
    public class PosPaymentItems
    {
        public double AmountDue { get; set; }
        public double AmountTendered { get; set; }
        public double ChangeAmount { get; set; }
        public int PaymentMethodID { get; set; }
    }

    public class PaymentMethod
    {
        public int PaymentMethodID { get; set; }
        public int IsDefault { get; set; }
        public string PaymentMethodName { get; set; }

        [ForeignKey("SubAccountID")]
        public int SubAccountID { get; set; }
    }
}