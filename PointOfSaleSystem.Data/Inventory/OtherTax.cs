﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryMagement.Data.Accounts;

namespace PointOfSaleSystem.Data.Inventory
{
    public class OtherTax
    {
        [Key]
        public int OtherTaxID { get; set; }
        public string OtherTaxName { get; set; }
        public int PerRate { get; set; }

        [ForeignKey("VATLiabSubAccountID")]
        public SubAccount SubAccount { get; set; }
        public int VATLiabSubAccountID { get; set; }
    }
}