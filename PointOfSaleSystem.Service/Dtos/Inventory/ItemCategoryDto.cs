namespace PointOfSaleSystem.Service.Dtos.Inventory
{
    public class ItemCategoryDto
    {
        public int ItemCategoryID { get; set; }
        public string ItemCategoryName { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
