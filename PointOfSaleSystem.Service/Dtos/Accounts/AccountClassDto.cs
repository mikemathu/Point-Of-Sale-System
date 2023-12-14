using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Dtos.Accounts
{
    public class AccountClassDto
    {
        public int AccountClassID { get; set; }
        public int AccountTypeID { get; set; }
        public string ClassName { get; set; } = null!; // Explicitly mark it as non-null to stop IDE from complaining
    }
}
