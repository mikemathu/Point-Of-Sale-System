using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface IJournalVoucherEntryRepository
    {
        Task<bool> CreateEntryAsync(JournalVoucherEntry journalVoucherEntry);
        Task<IEnumerable<JournalVoucherEntry>> GetJournalVoucherEntriesAsync(int journalVoucherID);
        Task<JournalVoucherEntry?> GetJournalVoucherEntryDetailsAsync(int accountID);
        Task<bool> DoesAccountEntryExist(int accountEntryID);
        Task<IEnumerable<JournalVoucherEntry>> GetJournalVoucherEntriesDetailsAsync(int journalVoucherID);
        ValueTask<bool> IsEntryPostedAsync(int accountEntryID);
        Task<bool> UpdateEntryAsync(JournalVoucherEntry journalVoucherEntry);
        Task<bool> DeleteEntryAsync(int accountEntryID);
    }
}
