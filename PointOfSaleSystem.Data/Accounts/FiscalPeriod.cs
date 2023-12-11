using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Data.Accounts
{
    public class FiscalPeriod
    {
        [Key]
        public int FiscalPeriodID { get; set; }
        public int FiscalPeriodNo { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public int IsActive { get; set; }
        public int IsOpen { get; set; }
    }
}