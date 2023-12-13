namespace PointOfSaleSystem.Data.Sales
{
    public class OrderedItem
    {
        public double SubTotal { get; set; }
        public int Quantity { get; set; }
        public int ItemID { get; set; }
        public int CustomerOrderID { get; set; }
        public string ItemName { get; set; } = "";
        public double UnitPrice { get; set; }
        public string UnitOfMeasureName { get; set; }
    }
}
