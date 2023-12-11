using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Accounts
{
    public class SubAccount
    {
        [Key]
        public int SubAccountID { get; set; }
        public string SubAccountName { get; set; }
        // Override ToString() to return the SubAccountName
        public override string ToString()
        {
            return SubAccountName;
        }
        public double CurrentBalance { get; set; }
        public int IsActive { get; set; }
        public int IsLocked { get; set; }

        [ForeignKey("AccountID")]
        public int AccountID { get; set; }
        public int ConfigurationTypeID { get; set; }
    }
    public class TransferSubAccountBalance
    {
        public float DestSubAccountID { get; set; }
        public float SourceSubAccountID { get; set; }
    }
}