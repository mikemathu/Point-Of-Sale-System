using AutoMapper;
using Microsoft.AspNetCore.Http;
using PointOfSaleSystem.Data.Security;
using PointOfSaleSystem.Service.Dtos.Security;
using PointOfSaleSystem.Service.Interfaces.Security;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Security
{
    public class RoleService
    {
        private readonly IRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoleService(IRoleRepository userRoleRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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
        public int? GetServicePointId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userIdClaim = user.FindFirst("SysUserID");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userID))
            {
                return userID;
            }
            return null;
        }
        public IEnumerable<string> GetUserPrivileges()
        {
            var sysUserID = GetServicePointId();
            //return _userRoleRepository.GetUserPrivilege((int)sysUserID);
            if (sysUserID.HasValue) // Check if sysUserID is not null
            {
                return _userRoleRepository.GetUserPrivilege(sysUserID.Value);
            }
            else
            {
                return Enumerable.Empty<string>(); // Return an empty collection
            }
        }
        public async Task<RoleDto> CreateUpdateRoleAsync(RoleDto roleDto)
        {
            Role? createdUpdatedUserRole = null;
            if (roleDto.RoleID == 0) //Create
            {
                createdUpdatedUserRole = await _userRoleRepository.CreateRoleAsync(_mapper.Map<Role>(roleDto));
            }
            else//Update
            {
                createdUpdatedUserRole = await _userRoleRepository.UpdateRoleAsync(_mapper.Map<Role>(roleDto));
            }
            if (createdUpdatedUserRole == null)
            {
                throw new ActionFailedException("Could not Create/Update Role");
            }
            return _mapper.Map<RoleDto>(createdUpdatedUserRole);
        }
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            IEnumerable<Role> roles = await _userRoleRepository.GetAllRolesAsync();
            if (!roles.Any())
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
        public async Task<RoleDto> GetRoleDetailsAsync(int roleID)
        {
            await IsRoleIdValid(roleID);
            Role? role = await _userRoleRepository.GetRoleDetailsAsync(roleID);
            if (role == null)
            {
                throw new EmptyDataResultException();
            }
            return _mapper.Map<RoleDto>(role);
        }
        public async Task DeleteRoleAsync(int roleID)
        {
            await IsRoleIdValid(roleID);
            bool isRoleDeleted = await _userRoleRepository.DeleteRoleAsync(roleID);
            if (!isRoleDeleted)
            {
                throw new ActionFailedException("Could not delete role. Try again.");
            }
        }
    }
}
