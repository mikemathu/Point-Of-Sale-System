using AutoMapper;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class AccountClassService
    {
        private readonly IAccountClassRepository _accountClassRepository;
        private readonly IMapper _mapper;
        public AccountClassService(IAccountClassRepository accountClassRepository, IMapper mapper)
        {
            _accountClassRepository = accountClassRepository;
            _mapper = mapper;
        }
        public async Task CreateAccountClassAsync(AccountClassDto accountClassDto)
        {
            bool isAccountClassCreated = await _accountClassRepository.CreateAccountClassAsync(_mapper.Map<AccountClass>(accountClassDto));
            if (!isAccountClassCreated)
            {
                throw new FalseException("Could not create Account Class.");
            }
        }
        public async Task<IEnumerable<AccountClassDto>> GetAllAccountClassesAsync()
        {
            IEnumerable<AccountClass> accountClasses = await _accountClassRepository.GetAllAccountClassesAsync();
            if (!accountClasses.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<AccountClassDto>>(accountClasses);
        }
    }
}
