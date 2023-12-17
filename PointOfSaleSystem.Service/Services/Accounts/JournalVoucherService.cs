using AutoMapper;
using Microsoft.AspNetCore.Http;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class JournalVoucherService
    {
        private readonly IJournalVoucherRepository _journalVoucherRepository;
        private readonly IJournalVoucherEntryRepository _journalVoucherEntryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public JournalVoucherService(
            IJournalVoucherRepository journalVoucherRepository,
            IJournalVoucherEntryRepository journalVoucherEntryRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _journalVoucherRepository = journalVoucherRepository;
            _journalVoucherEntryRepository = journalVoucherEntryRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private async Task IsJournalVoucherIdValid(int journalVoucherID)
        {
            if (journalVoucherID <= 0)
            {
                throw new ArgumentException("Invalid JournalVoucherID Id. It must be a positive integer.");
            }
            bool doesJournalVoucherExist = await _journalVoucherRepository.DoesJournalVoucherExist(journalVoucherID);
            if (!doesJournalVoucherExist)
            {
                throw new ItemNotFoundException($"Journal Voucher with Id {journalVoucherID} not found.");
            }
        }
        private async Task<IEnumerable<JournalVoucherEntryDto>> GetJournalVoucherEntriesAsync(int journalVoucherID)
        {
            IEnumerable<JournalVoucherEntry> journalVoucherEntries = await _journalVoucherEntryRepository.GetJournalVoucherEntriesAsync(journalVoucherID);
            if (!journalVoucherEntries.Any())
            {
                throw new ActionFailedException("This Journal Voucher has no entries");
            }
            return _mapper.Map<IEnumerable<JournalVoucherEntryDto>>(journalVoucherEntries);
        }
        private async Task<IEnumerable<JournalVoucherEntryDto>> GetJournalVoucherEntriesDetailsAsync(int journalVoucherID)
        {
            IEnumerable<JournalVoucherEntry> journalVoucherEntriesDetails = await _journalVoucherEntryRepository.GetJournalVoucherEntriesDetailsAsync(journalVoucherID);
            if (!journalVoucherEntriesDetails.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<JournalVoucherEntryDto>>(journalVoucherEntriesDetails);
        }
        private int GetSysUserID()
        {
            int sysUserID = 0;

            var sysUserIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("SysUserID");
            if (sysUserIdClaim != null)
            {
                sysUserID = Convert.ToInt32(sysUserIdClaim.Value);
            }
            return sysUserID;
        }
        public async Task<JournalVoucherDto> CreateUpdateJournalVoucherAsync(JournalVoucherDto journalVoucherDto)
        {
            JournalVoucher? journalVoucher = null;

            if (journalVoucherDto.JournalVoucherID == 0)//Create
            {
                int userId = GetSysUserID();
                journalVoucher = await _journalVoucherRepository.CreateJournalVoucherAsync(_mapper.Map<JournalVoucher>(journalVoucherDto), userId);
            }
            else//update
            {
                await IsJournalVoucherPostedAsync(journalVoucherDto.JournalVoucherID);
                //await IsJournalVoucherAutomaticallyPostedAsync(journalVoucherDto.JournalVoucherID);   
                journalVoucher = await _journalVoucherRepository.UpdateJournalVoucherAsync(_mapper.Map<JournalVoucher>(journalVoucherDto));
            }
            if (journalVoucher == null)
            {
                throw new ActionFailedException("Could not Create/Update Item.");
            }
            return _mapper.Map<JournalVoucherDto>(journalVoucher);
        }

        public async Task<JournalVoucherDto> GetJournalVoucherDetailsAsync(int journalVoucherID)
        {
            await IsJournalVoucherIdValid(journalVoucherID);
            JournalVoucher? journalVoucher = await _journalVoucherRepository.GetJournalVoucherDetailsAsync(journalVoucherID);
            if (journalVoucher == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<JournalVoucherDto>(journalVoucher);
        }

        private  (int, int) SetIsAutomaticAndIsPostedValuesBasedOnFlag(FilterJournalVoucherDto filterJournalVoucherDto)
        {
            int isAutomatic = 2;
            int isPosted = 2;
            if (filterJournalVoucherDto.Flag == 1)
            {
                isAutomatic = 0;
                isPosted = 0;
            }
            if (filterJournalVoucherDto.Flag == 2)
            {
                isAutomatic = 0;
                isPosted = 1;
            }
            if (filterJournalVoucherDto.Flag == 3)
            {
                isAutomatic = 1;
                isPosted = 0;
            }
            if (filterJournalVoucherDto.Flag == 4)
            {
                isAutomatic = 1;
                isPosted = 1;
            }
            if (filterJournalVoucherDto.Flag < 0 || filterJournalVoucherDto.Flag > 5)
            {
                throw new ArgumentOutOfRangeException(filterJournalVoucherDto.Flag.ToString(), $"Flag {filterJournalVoucherDto.Flag} is out of range.");
            }
            return (isAutomatic, isPosted);
        }
        public async Task<IEnumerable<JournalVoucherDto>> FilterJournalVouchersAsync(FilterJournalVoucherDto filterJournalVoucherDto)
        {
            (int isAutomatic, int isPosted) = SetIsAutomaticAndIsPostedValuesBasedOnFlag(filterJournalVoucherDto);
            IEnumerable<JournalVoucher> journalVouchers = await _journalVoucherRepository.FilterJournalVouchersAsync(
                _mapper.Map<FilterJournalVoucher>(filterJournalVoucherDto), isAutomatic, isPosted);
            if (!journalVouchers.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<JournalVoucherDto>>(journalVouchers);
        }
        private async Task IsJournalVoucherPostedAsync(int journalVoucherID)
        {
            bool isJournalVoucherPosted = await _journalVoucherRepository.IsJournalVoucherPostedAsync(journalVoucherID);
            if (isJournalVoucherPosted)
            {
                throw new ActionFailedException("This Journal Voucher has already been posted");
            }
        }
        public async Task PostJournalVoucherAsync(int journalVoucherID)
        {
            await IsJournalVoucherIdValid(journalVoucherID);
            await IsJournalVoucherPostedAsync(journalVoucherID);
            IEnumerable<JournalVoucherEntryDto> journalVoucherEntries = await GetJournalVoucherEntriesAsync(journalVoucherID);
            bool isPostJournalVoucherPosted = await _journalVoucherRepository.PostJournalVoucherAsync(_mapper.Map<IEnumerable<JournalVoucherEntry>>(journalVoucherEntries));
            if (!isPostJournalVoucherPosted)
            {
                throw new ActionFailedException("Could not post Journal voucher. Pleace Try again");
            }
        }
        public async Task UnPostJournalVoucherAsync(int journalVoucherID)
        {
            await IsJournalVoucherIdValid(journalVoucherID);
            bool isJournalVoucherPosted = await _journalVoucherRepository.IsJournalVoucherPostedAsync(journalVoucherID);
            if (!isJournalVoucherPosted)
            {
                throw new ActionFailedException("This Journal Voucher is not Posted");
            }
            await IsJournalVoucherAutomaticallyPostedAsync(journalVoucherID);
            IEnumerable<JournalVoucherEntryDto> journalVoucherEntriesDetails = await GetJournalVoucherEntriesDetailsAsync(journalVoucherID);
            bool isJournalVoucherUnPosted = await _journalVoucherRepository.UnPostJournalVoucherAsync(_mapper.Map<IEnumerable<JournalVoucherEntry>>(journalVoucherEntriesDetails));
            if (!isJournalVoucherUnPosted)
            {
                throw new ActionFailedException("Could not Unpost Journal voucher. Pleace Try again");
            }
        }
        private async Task IsJournalVoucherAutomaticallyPostedAsync(int journalVoucherID)
        {
            bool isJournalVoucherAutomaticallyPostedAsync = await _journalVoucherRepository.IsJournalVoucherAutomaticallyPostedAsync(journalVoucherID);
            if (isJournalVoucherAutomaticallyPostedAsync)
            {
                throw new ActionFailedException("You are attempting to unpost an automatically posted voucher. Post a reversal instead.");
            }
        }
        public async Task DeleteJournalVoucherAsync(int journalVoucherID)
        {
            await IsJournalVoucherIdValid(journalVoucherID);
            await IsJournalVoucherPostedAsync(journalVoucherID);
            bool isJournalVoucherDeleted = await _journalVoucherRepository.DeleteJournalVoucherAsync(journalVoucherID);
            if (!isJournalVoucherDeleted)
            {
                throw new ActionFailedException("Could not delete Journal Voucher. Try again later.");
            }
        }
    }
}