namespace PointOfSaleSystem.Service.Dtos.Sales
{
    public class OrderedItemDto
    {
        public double SubTotal { get; set; }
        public int Quantity { get; set; }
        public int ItemID { get; set; }
        public int CustomerOrderID { get; set; }
        public string ItemName { get; set; } = null!;
        public double UnitPrice { get; set; }
        public string UnitOfMeasureName { get; set; } = null!;
    }
}
