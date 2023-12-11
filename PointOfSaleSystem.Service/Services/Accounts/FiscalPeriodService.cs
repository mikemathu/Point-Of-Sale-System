using AutoMapper;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class FiscalPeriodService
    {
        private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
        private readonly IMapper _mapper;

        public FiscalPeriodService(IFiscalPeriodRepository fiscalPeriodRepository, IMapper mapper)
        {
            _fiscalPeriodRepository = fiscalPeriodRepository;
            _mapper = mapper;
        }
        private async Task IsFiscalPeriodRangeOverlapAsync(FiscalPeriodDto fiscalPeriodDto)
        {
            bool isFiscalPeriodRangeOverlap = await _fiscalPeriodRepository.IsFiscalPeriodRangeOverlapAsync(_mapper.Map<FiscalPeriod>(fiscalPeriodDto));
            if (isFiscalPeriodRangeOverlap)
            {
                throw new FalseException("This period is overlapping with another fiscal period.");
            }
        }
        private async Task IsFiscalPeriodOpenAsync(FiscalPeriodDto fiscalPeriodDto)
        {
            bool isFiscalPeriodOpen = await _fiscalPeriodRepository.IsFiscalPeriodOpenAsync(fiscalPeriodDto.FiscalPeriodID);
            if (!isFiscalPeriodOpen)
            {
                throw new FalseException("Fiscal Period is Closed");
            }
        }
        private async Task ValidateFiscalPeriodId(int fiscalPeriodID)
        {
            if (fiscalPeriodID <= 0)
            {
                throw new ArgumentException("Invalid Account Id. It must be a positive integer.");
            }
            bool doesFiscalPeriodExist = await _fiscalPeriodRepository.DoesFiscalPeriodExist(fiscalPeriodID);
            if (!doesFiscalPeriodExist)
            {
                throw new ValidationRowNotFoudException($"Fiscal Period with Id {fiscalPeriodID} not found.");
            }
        }
        public async Task<FiscalPeriodDto> CreateUpdateFiscalPeriodAsync(FiscalPeriodDto fiscalPeriodDto)
        {
            IsOpenDateGreaterThanCloseDate(fiscalPeriodDto);
            FiscalPeriod? fiscalPeriod = null;
            if (fiscalPeriodDto.FiscalPeriodNo == 0)//Create
            {
                await IsFiscalPeriodRangeOverlapAsync(fiscalPeriodDto);
                fiscalPeriod = await _fiscalPeriodRepository.CreateFiscalPeriodAsync(_mapper.Map<FiscalPeriod>(fiscalPeriodDto));
            }
            else //Update
            {
                await IsFiscalPeriodOpenAsync(fiscalPeriodDto);
                fiscalPeriod = await _fiscalPeriodRepository.UpdateFiscalPeriodAsync(_mapper.Map<FiscalPeriod>(fiscalPeriodDto));
            }
            if (fiscalPeriod == null)
            {
                throw new FalseException("Could Not Create Fiscal Period");
            }
            return _mapper.Map<FiscalPeriodDto>(fiscalPeriod);
        }
        public async Task<IEnumerable<FiscalPeriodDto>> GetAllFiscalPeriodsAsync()
        {
            IEnumerable<FiscalPeriod> fiscalPeriods = await _fiscalPeriodRepository.GetAllFiscalPeriodsAsync();
            if (!fiscalPeriods.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<FiscalPeriodDto>>(fiscalPeriods);
        }
        public async Task<IEnumerable<FiscalPeriodDto>> GetAllActiveFiscalPeriodsAsync()
        {
            IEnumerable<FiscalPeriod> fiscalPeriods = await _fiscalPeriodRepository.GetAllActiveFiscalPeriodsAsync();
            if (!fiscalPeriods.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<FiscalPeriodDto>>(fiscalPeriods);
        }
        public async Task<IEnumerable<FiscalPeriodDto>> GetAllInActiveFiscalPeriodsAsync()
        {
            IEnumerable<FiscalPeriod> inActivefiscalPeriods = await _fiscalPeriodRepository.GetAllInActiveFiscalPeriodsAsync();
            if (!inActivefiscalPeriods.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<FiscalPeriodDto>>(inActivefiscalPeriods);
        }
        public async Task<FiscalPeriodDto> GetFiscalPeriodDetailsAsync(int fiscalPeriodID)
        {
            await ValidateFiscalPeriodId(fiscalPeriodID);
            FiscalPeriod? fiscalPeriodDetails = await _fiscalPeriodRepository.GetFiscalPeriodDetailsAsync(fiscalPeriodID);
            if (fiscalPeriodDetails == null)
            {
                throw new NullException();
            }
            return _mapper.Map<FiscalPeriodDto>(fiscalPeriodDetails);
        }
        private async Task IsFiscalPeriodActiveAsync(int fiscalPeriodID)
        {
            bool isFiscalPeriodActive = await _fiscalPeriodRepository.IsFiscalPeriodActiveAsync(fiscalPeriodID);
            if (isFiscalPeriodActive)
            {
                throw new FalseException("This period is still active");
            }
        }
        public async Task<FiscalPeriodDto> CloseFiscalPeriodAsync(int fiscalPeriodID)
        {
            await IsFiscalPeriodActiveAsync(fiscalPeriodID);
            FiscalPeriod? fiscalPeriod = await _fiscalPeriodRepository.CloseFiscalPeriodAsync(fiscalPeriodID);
            if (fiscalPeriod == null)
            {
                throw new FalseException("First close the previous fiscal period.");
            }
            return _mapper.Map<FiscalPeriodDto>(fiscalPeriod);
        }
        public async Task DeleteFiscalPeriodAsync(int fiscalPeriodID)
        {
            await ValidateFiscalPeriodId(fiscalPeriodID);
            await IsFiscalPeriodActiveAsync(fiscalPeriodID);
            bool isFiscalPeriodDeleted = await _fiscalPeriodRepository.DeleteFiscalPeriodAsync(fiscalPeriodID);
            if (!isFiscalPeriodDeleted)
            {
                throw new FalseException("Could not delete Fiscal Period");
            }
        }
        private void IsOpenDateGreaterThanCloseDate(FiscalPeriodDto fiscalPeriodDto)
        {
            if (fiscalPeriodDto.OpenDate > fiscalPeriodDto.CloseDate)
            {
                throw new FalseException("Cannot create the selected period. Pleace verify the open and/or close dates.");
            }
        }
    }
}