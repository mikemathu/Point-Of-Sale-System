namespace PointOfSaleSystem.Service.Dtos.Accounts
{
    public class FiscalPeriodDto
    {
        public int FiscalPeriodID { get; set; }
        public int FiscalPeriodNo { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public int IsActive { get; set; }
        public int IsOpen { get; set; }
    }
}
