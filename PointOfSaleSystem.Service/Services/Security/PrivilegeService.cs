using AutoMapper;
using PointOfSaleSystem.Data.Security;
using PointOfSaleSystem.Service.Dtos.Security;
using PointOfSaleSystem.Service.Interfaces.Security;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Security
{
    public class PrivilegeService
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;

        public PrivilegeService(IPrivilegeRepository privilegeRepository, IRoleRepository userRoleRepository, IMapper mapper)
        {
            _privilegeRepository = privilegeRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }
        private async Task IsPrivilegeIdValid(int privilegeID)
        {
            if (privilegeID <= 0)
            {
                throw new ArgumentException("Invalid Privilege Id. It must be a positive integer.");
            }
            bool doesPrivilegeExist = await _privilegeRepository.DoesPrivilegeExist(privilegeID);
            if (!doesPrivilegeExist)
            {
                throw new ItemNotFoundException($"Privilege with Id {privilegeID} not found.");
            }
        }
        private async Task IsRoleIdValid(int roleID)
        {
            if (roleID <= 0)
            {
                throw new ArgumentException("Invalid Role Id. It must be a positive integer.");
            }
            bool doesRoleExist = await _userRoleRepository.DoesRoleExist(roleID);
            if (!doesRoleExist)
            {
                throw new ItemNotFoundException($"Role with Id {roleID} not found.");
            }
        }
        public async Task<PrivilegeDto> CreateUpdatePrivilegeAsync(PrivilegeDto privilegeDto)
        {
            Privilege? createdUpdatedUserRole = null;
            if (privilegeDto.PrivilegeID == 0) //Create
            {
                createdUpdatedUserRole = await _privilegeRepository.CreatePrivilegeAsync(_mapper.Map<Privilege>(privilegeDto));
            }
            else//Update
            {
                createdUpdatedUserRole = await _privilegeRepository.UpdatePrivilegeAsync(_mapper.Map<Privilege>(privilegeDto));
            }
            if (createdUpdatedUserRole == null)
            {
                throw new ActionFailedException("Could not Create/Update Privilege");
            }
            return _mapper.Map<PrivilegeDto>(createdUpdatedUserRole);
        }
        public async Task<IEnumerable<PrivilegeDto>> GetAllPrivilegesAsync()
        {
            IEnumerable<Privilege> privileges = await _privilegeRepository.GetAllPrivilegesAsync();
            if (!privileges.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<PrivilegeDto>>(privileges);
        }
        public async Task<IEnumerable<PrivilegeDto>> GetRolePrivilegesAsync(int roleID)
        {
            await IsRoleIdValid(roleID);
            IEnumerable<Privilege> privileges = await _privilegeRepository.GetRolePrivilegesAsync(roleID);
            if (!privileges.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<PrivilegeDto>>(privileges);
        }
        public async Task<IEnumerable<PrivilegeDto>> AddPrivilegesToRoleAsync(RolePrivilegeDto rolePrivilegeDto)
        {
            if (rolePrivilegeDto.RoleID == 0)
            {
                throw new ArgumentException("Select Role First");
            }
            IEnumerable<Privilege> rolePrivilege = await _privilegeRepository.AddPrivilegesToRoleAsync(_mapper.Map<RolePrivilege>(rolePrivilegeDto));
            if (!rolePrivilege.Any())
            {
                throw new ActionFailedException("Could not Add Privilege to Role. Try again");
            }
            return _mapper.Map<IEnumerable<PrivilegeDto>>(rolePrivilege);
        }
        public async Task<IEnumerable<PrivilegeDto>> DeletePrivilegesFromRoleAsync(RolePrivilegeDto rolePrivilegeDto)
        {
            IEnumerable<Privilege> rolePrivilege = await _privilegeRepository.DeletePrivilegesFromRoleAsync(_mapper.Map<RolePrivilege>(rolePrivilegeDto));
            if (!rolePrivilege.Any())
            {
                throw new ActionFailedException("Could not Delete Privilege(s) from Role. Try Again");
            }
            return _mapper.Map<IEnumerable<PrivilegeDto>>(rolePrivilege);
        }
        public async Task DeleteRolePrivilegeAsync(int privilegeID)
        {
            await IsPrivilegeIdValid(privilegeID);
            bool isPriviledgeDeleted = await _privilegeRepository.DeleteRolePrivilegeAsync(privilegeID);
            if (!isPriviledgeDeleted)
            {
                throw new ActionFailedException("Could not Delete Privilege. Try again.");
            }
        }
    }
}
