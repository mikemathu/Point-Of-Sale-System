﻿using AutoMapper;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class JournalVoucherEntryService
    {
        private readonly IJournalVoucherEntryRepository _journalVoucherEntryRepository;
        private readonly IJournalVoucherRepository _journalVoucherRepository;
        private readonly IMapper _mapper;
        public JournalVoucherEntryService(
            IJournalVoucherEntryRepository journalVoucherEntryRepository,
            IJournalVoucherRepository journalVoucherRepository,
            IMapper mapper)
        {
            _journalVoucherEntryRepository = journalVoucherEntryRepository;
            _journalVoucherRepository = journalVoucherRepository;
            _mapper = mapper;
        }

        private async Task IsJournalVoucherPostedAsync(JournalVoucherEntryDto2 journalVoucherEntryDto)
        {
            bool isJournalVoucherPosted = await _journalVoucherRepository.IsJournalVoucherPostedAsync(journalVoucherEntryDto.JournalVoucherID);
            if (isJournalVoucherPosted)
            {
                throw new ActionFailedException("This Journal Voucher has already been posted");
            }
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
        private async Task IsAccountEntryIdValid(int accountEntryID)
        {
            if (accountEntryID <= 0)
            {
                throw new ArgumentException("Invalid JournalVoucherID Id. It must be a positive integer.");
            }
            bool doesAccountEntryExist = await _journalVoucherEntryRepository.DoesAccountEntryExist(accountEntryID);
            if (!doesAccountEntryExist)
            {
                throw new ItemNotFoundException($"Account Entry with Id {accountEntryID} not found.");
            }
        }
        public async Task CreateUpdateEntryAsync(JournalVoucherEntryDto2 journalVoucherEntryDto)
        {
            await IsJournalVoucherIdValid(journalVoucherEntryDto.JournalVoucherID);
            AreSubAccountSame(journalVoucherEntryDto);
            await IsJournalVoucherPostedAsync(journalVoucherEntryDto);
            bool isEntryCreateUpdateSuccess = false;
            if (journalVoucherEntryDto.JournalVoucherEntryID == 0)//Create
            {
                isEntryCreateUpdateSuccess = await _journalVoucherEntryRepository.CreateEntryAsync(
                    _mapper.Map<JournalVoucherEntry>(journalVoucherEntryDto));
            }
            else //update
            {
                isEntryCreateUpdateSuccess = await _journalVoucherEntryRepository.UpdateEntryAsync(
                    _mapper.Map<JournalVoucherEntry>(journalVoucherEntryDto));
            }
            if (!isEntryCreateUpdateSuccess)
            {
                throw new ActionFailedException("Could Not Create/Update Entry");
            }
        }
        public async Task<JournalVoucherEntryDto> GetJournalVoucherEntryDetailsAsync(int accountEntryID)
        {
            await IsAccountEntryIdValid(accountEntryID);
            JournalVoucherEntry? journalVoucherEntry = await _journalVoucherEntryRepository.GetJournalVoucherEntryDetailsAsync(accountEntryID);
            if (journalVoucherEntry == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<JournalVoucherEntryDto>(journalVoucherEntry);
        }
        public async Task<IEnumerable<JournalVoucherEntryDto>> GetJournalVoucherEntriesDetailsAsync(int accountEntryID)
        {
            IEnumerable<JournalVoucherEntry> journalVoucherEntry = await _journalVoucherEntryRepository.GetJournalVoucherEntriesDetailsAsync(accountEntryID);
            if (!journalVoucherEntry.Any())
            {
                //throw new ActionFailedException("Entries not Found.");
            }
            return _mapper.Map<IEnumerable<JournalVoucherEntryDto>>(journalVoucherEntry);
        }
        private async Task IsEntryPostedAsync(int accountEntryID)
        {
            bool isEntryPosted = await _journalVoucherEntryRepository.IsEntryPostedAsync(accountEntryID);
            if (isEntryPosted)
            {
                throw new ActionFailedException("This Entry is already Posted");
            }
        }
        public async Task<IEnumerable<JournalVoucherEntryDto>> GetJournalVoucherEntriesAsync(int journalVoucherID)
        {
            await IsJournalVoucherIdValid(journalVoucherID);
            IEnumerable<JournalVoucherEntry> journalVoucherEntries = await _journalVoucherEntryRepository.GetJournalVoucherEntriesAsync(journalVoucherID);
            if (!journalVoucherEntries.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<JournalVoucherEntryDto>>(journalVoucherEntries);
        }
        public async Task DeleteEntryAsync(int accountEntryID)
        {
            await IsAccountEntryIdValid(accountEntryID);
            await IsEntryPostedAsync(accountEntryID);
            bool isEntryDeleted = await _journalVoucherEntryRepository.DeleteEntryAsync(accountEntryID);
            if (!isEntryDeleted)
            {
                throw new ActionFailedException("Could not delete the account entry. Try again later.");
            }
        }
        private void AreSubAccountSame(JournalVoucherEntryDto2 journalVoucherEntryDto)
        {
            if (journalVoucherEntryDto.DebitSubAccountID == journalVoucherEntryDto.CreditSubAccountID)
            {
                throw new ActionFailedException("The entry cannot have the same sub-accounts.");
            }
        }
    }
}
