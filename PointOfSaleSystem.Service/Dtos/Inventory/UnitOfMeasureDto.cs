namespace PointOfSaleSystem.Service.Dtos.Inventory
{
    public class UnitOfMeasureDto
    {
        public int UnitOfMeasureID { get; set; }
        public string UnitOfMeasureName { get; set; } = null!;
        public int IsSmallestUnit { get; set; }
    }
}
