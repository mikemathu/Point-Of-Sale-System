using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Accounts
{
    public class JournalVoucher
    {
        [Key]
        public int JournalVoucherID { get; set; }
        public string Description { get; set; }
        public string SourceReference { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public int IsAutomatic { get; set; }
        public int IsPosted { get; set; }
        public int PostedBySyUID { get; set; }

        [ForeignKey("FiscalPeriodID")]
        public int FiscalPeriodID { get; set; }

    }

    public class FilterJournalVoucher
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int FiscallPeriodID { get; set; }
        public int Flag { get; set; }
    }
}
