using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Accounts
{
    public class CashFlowCategory
    {
        [Key]
        public int CashFlowCategoryID { get; set; }
        public string CashFlowCategoryName { get; set; } = "";

        [ForeignKey("CashFlowCategoryTypeID")]
        public CashFlowCategoryType CashFlowCategoryType { get; set; } = new CashFlowCategoryType();
        public int CashFlowCategoryTypeID { get; set; }
    }
    public enum CashFlowCategoryTypee
    {
        OperatingActivities = 1,
        InvestingActivities = 2,
        FinancingActivities = 3

    }
}
