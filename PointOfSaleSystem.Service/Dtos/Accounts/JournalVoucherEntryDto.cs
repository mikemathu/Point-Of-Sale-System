﻿using InventoryMagement.Data.Accounts;

namespace PointOfSaleSystem.Service.Dtos.Accounts
{
    public class JournalVoucherEntryDto
    {
        public int CreditAccountID { get; set; }
        public int DebitAccountID { get; set; }
        public int JournalVoucherEntryID { get; set; }
        public float CreditAmount { get; set; }
        public int CreditSubAccountID { get; set; }
        public float DebitAmount { get; set; }
        public int DebitSubAccountID { get; set; }
        public int JournalVoucherID { get; set; }
        public SubAccount CreditSubAccount { get; set; }
        public SubAccount DebitSubAccount { get; set; }
    }

    public class JournalVoucherEntryDto2
    {
        public int JournalVoucherID { get; set; }
        public int JournalVoucherEntryID { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public int DebitSubAccountID { get; set; }
        public int CreditSubAccountID { get; set; }
    }
}