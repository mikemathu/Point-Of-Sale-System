using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Data.Accounts
{
    public class AccountType
    {
        [Key]
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
    }
}
