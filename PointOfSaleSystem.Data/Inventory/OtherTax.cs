using PointOfSaleSystem.Data.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Inventory
{
    public class OtherTax
    {
        [Key]
        public int OtherTaxID { get; set; }
        public string OtherTaxName { get; set; } = "";
        public double PerRate { get; set; }

        [ForeignKey("VATLiabSubAccountID")]
        public SubAccount SubAccount { get; set; } = new SubAccount();
        public int VATLiabSubAccountID { get; set; }
    }
}
