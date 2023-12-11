using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Data.Accounts
{
    public class CashFlowCategoryType
    {
        [Key]
        public int CashFlowCategoryTypeID { get; set; }
        public string CashFlowCategoryTypeName { get; set; } = "";
    }
}
