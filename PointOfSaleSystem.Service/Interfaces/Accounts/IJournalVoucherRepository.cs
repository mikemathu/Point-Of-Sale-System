using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface IJournalVoucherRepository
    {
        Task<JournalVoucher?> CreateJournalVoucherAsync(JournalVoucher journalVoucher, int userId);
        Task<JournalVoucher?> UpdateJournalVoucherAsync(JournalVoucher journalVoucher);
        Task<JournalVoucher?> GetJournalVoucherDetailsAsync(int journalVoucherID);
        Task<IEnumerable<JournalVoucher>> FilterJournalVouchersAsync(FilterJournalVoucher filterJournalVoucher, int isAutomatic, int isPosted);
        Task<bool> IsJournalVoucherPostedAsync(int journalVoucherID);
        Task<bool> IsJournalVoucherAutomaticallyPostedAsync(int journalVoucherID);
        Task<bool> PostJournalVoucherAsync(IEnumerable<JournalVoucherEntry> journalVoucherEntry);
        Task<bool> UnPostJournalVoucherAsync(IEnumerable<JournalVoucherEntry> journalVoucherEntry);
        Task<bool> DeleteJournalVoucherAsync(int journalVoucherID);
        Task<bool> DoesJournalVoucherExist(int journalVoucherID);
    }
}
