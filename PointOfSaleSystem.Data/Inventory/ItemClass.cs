using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Inventory
{
    public class ItemClass
    {
        [Key]
        public int ItemClassID { get; set; }
        public string ItemClassName { get; set; } = "";
        public string Description { get; set; }

        [ForeignKey("ItemClassTypeID")]
        public ItemClassType ItemClassType { get; set; }
        public int ItemClassTypeID { get; set; }
    }
}
