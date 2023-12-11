namespace PointOfSaleSystem.Service.Dtos.Inventory
{
    public class CustomerOrderDto
    {
        public int CustomerOrderID { get; set; }
        public int CustomerOrderItemID { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
