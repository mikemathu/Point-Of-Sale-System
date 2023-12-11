using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Accounts
{
    public class JournalVoucherEntry
    {
        public int JournalVoucherEntryID { get; set; }
        public double CreditAmount { get; set; }
        public double DebitAmount { get; set; }

        [ForeignKey("CreditAccountID")]
        public int CreditAccountID { get; set; }

        [ForeignKey("DebitAccountID")]
        public int DebitAccountID { get; set; }

        [ForeignKey("CreditSubAccountID")]
        public SubAccount CreditSubAccount { get; set; }
        public int CreditSubAccountID { get; set; }

        [ForeignKey("DebitSubAccountID")]
        public SubAccount DebitSubAccount { get; set; }
        public int DebitSubAccountID { get; set; }

        [ForeignKey("JournalVoucherID")]
        public int JournalVoucherID { get; set; }
    }
}
