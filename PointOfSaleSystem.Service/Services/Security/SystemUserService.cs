using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using PointOfSaleSystem.Data.Security;
using PointOfSaleSystem.Service.Dtos.Security;
using PointOfSaleSystem.Service.Interfaces.Security;
using PointOfSaleSystem.Service.Services.Exceptions;
using System.Security.Authentication;
using System.Security.Claims;

namespace PointOfSaleSystem.Service.Services.Security
{
    public class SystemUserService
    {
        private readonly ISystemUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SystemUserService(ISystemUserRepository systemUserRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = systemUserRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private async Task ValidateSystemUserId(int systemUserID)
        {
            if (systemUserID <= 0)
            {
                throw new ArgumentException("Invalid User Id. It must be a positive integer.");
            }
            bool doesSubAccountExist = await _userRepository.DoesUserExist(systemUserID);
            if (!doesSubAccountExist)
            {
                throw new ValidationRowNotFoudException($"User with Id {systemUserID} not found.");
            }
        }
        public async Task AuthenticateUserAsync(RegisterLoginDto systemUserDto)
        {
            SystemUser? systemUser = await _userRepository.AuthenticateUserAsync(_mapper.Map<SystemUser>(systemUserDto));
            if (systemUser == null)
            {
                throw new AuthenticationException("Validation failed due to incorrect Username and/or Password");
            }
            ClaimsIdentity claimsIdentity =  CreateClaimsIdentity(systemUser);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
        private int GetSysUserID()
        {
            var sysUserIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("SysUserID");
            int sysUserID = 0;
            if (sysUserIdClaim != null)
            {
                sysUserID = Convert.ToInt32(sysUserIdClaim.Value);
            }
            return sysUserID;
        }
        public async Task AuthenticateAccessPasswordAsync(string password)
        {
            //int userID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst("SysUserID").Value);
            int userID = GetSysUserID();
            bool success = await _userRepository.AuthenticateAccessPasswordAsync(userID, password);
            if (!success)
            {
                throw new AuthenticationException("Access Denied. Provided access password is incorrect.");
            }
        }
        private void ValidateUserDetails(RegisterLoginDto systemUserDto)
        {
            if (systemUserDto.UserName == string.Empty || systemUserDto.Password == string.Empty)
            {
                throw new FalseException("Invalid inputs.Please check your input fields and ensure they are all filled.");
            }
        }
        public async Task RegisterUserAsync(RegisterLoginDto systemUserDto)
        {
            ValidateUserDetails(systemUserDto);
            SystemUser? systemUser = await _userRepository.RegisterUserAsync(_mapper.Map<SystemUser>(systemUserDto));
            if (systemUser == null)
            {
                throw new FalseException("Could not register user. Try again.");
            }
            ClaimsIdentity claimsIdentity = CreateClaimsIdentity(systemUser);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
        private ClaimsIdentity CreateClaimsIdentity(SystemUser systemUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, systemUser.UserName),
                new Claim("SysUserID", Convert.ToString(systemUser.SysUserID))
            };
            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);//serializes the principal, encrypts it, and stores it as a cookie.
        }
        public async Task<IEnumerable<SystemUserDto>> GetAllUsersAsync()
        {
            IEnumerable<SystemUser> users = await _userRepository.GetAllUsersAsync();
            if (!users.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<SystemUserDto>>(users);
        }
        public async Task<SystemUserDto> GetUserDetailsAsync(int userID)
        {
            await ValidateSystemUserId(userID);
            SystemUser? userDetails = await _userRepository.GetUserDetailsAsync(userID);
            if (userDetails == null)
            {
                throw new FalseException("User Details not found.");
            }
            return _mapper.Map<SystemUserDto>(userDetails);
        }
        public async Task CreateUpdateSystemUserAsync(SystemUserDto systemUserDto)
        {
            bool createUpdateUserSuccess = false;
            if (systemUserDto.SysUserID == 0) //create
            {
                createUpdateUserSuccess = await _userRepository.CreateSystemUserAsync(_mapper.Map<SystemUser>(systemUserDto));
            }
            else//Update
            {
                createUpdateUserSuccess = await _userRepository.UpdateSystemUserAsync(_mapper.Map<SystemUser>(systemUserDto));
            }

            if (!createUpdateUserSuccess)
            {
                throw new FalseException("Could not Create/Update User. Try Again.");
            }
        }
    }
}
