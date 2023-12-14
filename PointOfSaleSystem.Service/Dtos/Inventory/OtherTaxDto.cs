namespace PointOfSaleSystem.Service.Dtos.Inventory
{
    public class OtherTaxDto
    {
        public int OtherTaxID { get; set; }
        public string OtherTaxName { get; set; } = null!;
        public double PerRate { get; set; }
        public int VATLiabSubAccountID { get; set; }
    }
}
