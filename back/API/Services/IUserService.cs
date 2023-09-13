using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;

namespace API.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync (RegisterDto model);
        Task<DataUserDto> GetTokenAsync (LogDto model);
        Task<string> AddRoleAsync (AddRoleDto model);
        Task<AuthTokenResponseDto> GetRefreshToken (RefreshTokenDto model, int idUser);
        
    }
}