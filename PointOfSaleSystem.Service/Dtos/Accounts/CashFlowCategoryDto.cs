using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Dtos.Accounts
{
    public class CashFlowCategoryDto
    {
        public int CashFlowCategoryID { get; set; }
        public string CashFlowCategoryName { get; set; }
        public int AccountTypeID { get; set; }
        public int CashFlowCategoryTypeID { get; set; }
        public int IsActive { get; set; }
        public CashFlowCategoryType CashFlowCategoryType { get; set; } = new CashFlowCategoryType();
    }
}