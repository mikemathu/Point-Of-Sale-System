namespace PointOfSaleSystem.Service.Dtos.Inventory
{
    public class ItemDto
    {
        public int ItemID { get; set; } = 111;
        public string ItemName { get; set; } = null!; 
        public double UnitCost { get; set; }
        public double UnitPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ItemCode { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string Batch { get; set; } = null!;
        public string Image { get; set; } = "";
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ShowInPOS { get; set; }
        public int IsActive { get; set; }
        public int UnitOfMeasureID { get; set; }
        public int ItemClassID { get; set; }
        public int ItemCategoryID { get; set; }
        public int AssetSubAccountID { get; set; }
        public int CostOfSaleSubAccountID { get; set; }
        public int RevenueSubAccountID { get; set; }
        public int VatTypeID { get; set; }
        public int OtherTaxID { get; set; }
    }
    public class FilterItemDto
    {
        public int FilterFlag { get; set; }
    }
}