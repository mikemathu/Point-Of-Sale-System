namespace PointOfSaleSystem.Service.Dtos.Accounts
{
    public class SubAccountDto
    {
        public int AccountID { get; set; }
        public int ConfigurationType { get; set; }
        public int CurrentBalance { get; set; }
        public int IsActive { get; set; }
        public int IsLocked { get; set; }
        public string SubAccountName { get; set; }
        public int SubAccountID { get; set; }
    }
    public class TransferSubAccountBalanceDto
    {
        public float DestSubAccountID { get; set; }
        public float SourceSubAccountID { get; set; }
    }
}
