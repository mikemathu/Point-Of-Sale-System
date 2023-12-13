using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Inventory
{
    public class CustomerOrder
    {
        [Key]
        public int CustomerOrderID { get; set; }
        public string CustomerName { get; set; } = "";
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int NetAmount { get; set; }
        public DateTime DateTimeBilled { get; set; }
        public int SaleStatus { get; set; }
        public int SalesType { get; set; }
        public int Status { get; set; }
        public string ColorClass { get; set; }

        [ForeignKey("CustomerOrderItemID")]
        public Item Item { get; set; }
        public int CustomerOrderItemID { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int CreatedBySysUID { get; set; }
        public int TotalAmountPaid { get; set; }
        public int BillIsPrinted { get; set; }
    }
}