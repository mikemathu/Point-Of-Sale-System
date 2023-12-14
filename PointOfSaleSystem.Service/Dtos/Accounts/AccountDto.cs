namespace PointOfSaleSystem.Service.Dtos.Accounts
{
    public class AccountDto
    {
        public int AccountClassID { get; set; }
        public int AccountID { get; set; }
        public int AccountNo { get; set; }
        public int CashFlowCategoryID { get; set; }
        public AccountClassDto? AccountClass { get; set; }
        public string AccountName { get; set; } = null!;
    }
}
