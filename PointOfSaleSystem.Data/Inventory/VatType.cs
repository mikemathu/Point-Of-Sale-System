using PointOfSaleSystem.Data.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Inventory
{
    public class VatType
    {
        [Key]
        public int VATTypeID { get; set; }
        public string VATTypeName { get; set; } = "";
        public int PerRate { get; set; }

        [ForeignKey("VATLiabSubAccountID")]
        public SubAccount SubAccount { get; set; } = new SubAccount();
        public int VATLiabSubAccountID { get; set; }
    }
}
