namespace PointOfSaleSystem.Service.Dtos.Inventory
{
    public class VatTypeDto
    {
        public int VATTypeID { get; set; }
        public string VATTypeName { get; set; }
        public double PerRate { get; set; }
        public int VATLiabSubAccountID { get; set; }
    }
}
