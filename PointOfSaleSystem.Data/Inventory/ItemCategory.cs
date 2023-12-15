using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Data.Inventory
{
    public class ItemCategory
    {
        [Key]
        public int ItemCategoryID { get; set; }
        public string ItemCategoryName { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
