namespace PointOfSaleSystem.Service.Dtos.Accounts
{
    public class JournalVoucherDto
    {
        public int JournalVoucherID { get; set; }
        public string Description { get; set; }
        public string SourceReference { get; set; }
        public float Amount { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public int IsAutomatic { get; set; }
        public int IsPosted { get; set; }
        public int FiscalPeriodID { get; set; }
    }

    public class FilterJournalVoucherDto
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int FiscallPeriodID { get; set; }
        public int Flag { get; set; }
    }
}