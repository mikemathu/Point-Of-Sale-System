using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Data.Accounts
{
    public class Account
    {

        [Key]
        public int AccountID { get; set; }
        public string AccountName { get; set; } = "";
        public int AccountNo { get; set; }
        public int IsLocked { get; set; }
        public int ConfigurationType { get; set; }

        [ForeignKey("CashFlowCategoryID")]
        public int CashFlowCategoryID { get; set; }

        [ForeignKey("AccountClassID")]
        public AccountClass AccountClass { get; set; } = new AccountClass();
        public int AccountClassID { get; set; }
    }
}
