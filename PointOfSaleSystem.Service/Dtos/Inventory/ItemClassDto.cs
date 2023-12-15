namespace PointOfSaleSystem.Service.Dtos.Inventory
{
    public class ItemClassDto
    {
        public int ItemClassID { get; set; }
        public string ItemClassName { get; set; } = null!; 
        public string Description { get; set; } = null!;
        public int ItemClassTypeID { get; set; }
    }
}
