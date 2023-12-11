using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Accounts
{
    public class AccountClass
    {
        [Key]
        public int AccountClassID { get; set; }
        public string ClassName { get; set; }

        [ForeignKey("AccountTypeID")]
        public int AccountTypeID { get; set; }
    }

    public enum AccountTypeEnum
    {
        Asset = 1,
        Liability = 2,
        OwnersEquity = 3,
        Revenue = 4,
        Expenses = 5
    }
}
