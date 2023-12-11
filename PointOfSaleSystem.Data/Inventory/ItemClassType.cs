using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Data.Inventory
{
    public class ItemClassType
    {
        [Key]
        public int ProductClassTypeID { get; set; }
        public string ProductClassTypeName { get; set; }
    }
}
