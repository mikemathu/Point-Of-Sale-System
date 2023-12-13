using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Data.Inventory
{
    public class UnitOfMeasure
    {
        [Key]
        public int UnitOfMeasureID { get; set; }
        public string UnitOfMeasureName { get; set; } = "";
        public int IsSmallestUnit { get; set; }

    }
}
