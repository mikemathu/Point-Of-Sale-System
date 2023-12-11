using InventoryMagement.Data.Accounts;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Inventory
{
    public class Item
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public double UnitCost { get; set; }
        public double UnitPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ItemCode { get; set; }
        public string Barcode { get; set; }
        public string Batch { get; set; }
        public string Image { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ShowInPOS { get; set; }
        public int IsActive { get; set; }

        [ForeignKey("UnitOfMeasureID")]
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public int UnitOfMeasureID { get; set; }

        [ForeignKey("ItemClassID")]
        public ItemClass ItemClass { get; set; }
        public int ItemClassID { get; set; }

        [ForeignKey("ItemCategoryID")]
        public ItemCategory ItemCategory { get; set; }
        public int ItemCategoryID { get; set; }

        [ForeignKey("AssetSubAccountID")]
        public SubAccount SubAccount { get; set; }
        public int AssetSubAccountID { get; set; }

        [ForeignKey("CostOfSaleSubAccountID")]
        public int CostOfSaleSubAccountID { get; set; }

        [ForeignKey("RevenueSubAccountID")]
        public int RevenueSubAccountID { get; set; }

        [ForeignKey("VatTypeID")]
        public VatType VATType { get; set; }
        public int VatTypeID { get; set; }

        [ForeignKey("OtherTaxID")]
        public int OtherTaxID { get; set; }
    }
    public class FilterItem
    {
        public int FilterFlag { get; set; }
    }
}
