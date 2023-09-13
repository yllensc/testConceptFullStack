using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Dtos;
using API.Helpers;
using Domine.Entities;
using Domine.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly JWT _jwt;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt, IPasswordHasher<User> passwordHasher)
        {
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }
        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
            };
            user.Password = _passwordHasher.HashPassword(user, registerDto.Password);
            var existUser = _unitOfWork.Users
                                            .Find(u => u.UserName.ToLower() == registerDto.UserName.ToLower())
                                            .FirstOrDefault();
            if (existUser == null)
            {
                try
                {
                    _unitOfWork.Users.Add(user);
                    await _unitOfWork.SaveAsync();
                    return $"El usuario {registerDto.UserName} ha sido registrado con éxito jiji";
                }
                catch (Exception ex)
                {
                    return $"Error : {ex.Message}";
                }
            }
            else
            {
                return $"El usuario {registerDto.UserName} ya se encuentra registrado";

            }
        }

        public async Task<string> AddRoleAsync(AddRoleDto userToAdd)
        {
            var user = await _unitOfWork.Users
                                    .GetUserName(userToAdd.UserName);
            if (user == null)
            {
                return $"No existe un usuario con la cuenta suministrada ¿Está seguro '{userToAdd.UserName}'";
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userToAdd.Password);
            if (result == PasswordVerificationResult.Success)
            {
                var existRol = _unitOfWork.GetRoles()
                                            .Find(r => r.RolName.ToLower() == userToAdd.Rol.ToLower())
                                            .FirstOrDefault();
                if (existRol != null)
                {
                    var userWithRol = user.Roles
                    .Any(r => r.Id == existRol.Id);

                    if (userWithRol == false)
                    {
                        user.Roles.Add(existRol);
                        _unitOfWork.Users.Update(user);
                        await _unitOfWork.SaveAsync();
                        return $"Rol {userToAdd.Rol} agregado a {userToAdd.UserName}";
                    }
                }
                return $"Rol {userToAdd.Rol} no encontrado";
            }
            return "Credenciales incorrectas, sorry";
        }

        public async Task<DataUserDto> GetTokenAsync(LogDto userLog)
        {
            DataUserDto dataUserDto = new DataUserDto();
            var user = await _unitOfWork.Users
                            .GetUserName(userLog.UserName);
            if (user == null)
            {
                dataUserDto.AuthStatus = false;
                dataUserDto.Message = $"No existe un usuario con el username {userLog.UserName}";
                return dataUserDto;
            }
            else
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userLog.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    dataUserDto.AuthStatus = true;
                    dataUserDto.Message = "OK";
                    JwtSecurityToken jwtSecurityToken = CreateJwtToken(user);
                    string refreshTokenCreated = GenerarRefreshToken();
                    string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                    dataUserDto.Token = token;
                    dataUserDto.Email = user.Email;
                    dataUserDto.UserName = user.UserName;
                    dataUserDto.Roles = user.Roles
                                        .Select(x => x.RolName)
                                        .ToList();
                    dataUserDto.RefreshToken = refreshTokenCreated;

                    await SaveHistorialRefreshToken(user.Id, token, refreshTokenCreated);
                    return dataUserDto;
                }
                else
                {
                    dataUserDto.AuthStatus = false;
                    dataUserDto.Message = $"credenciales incorrectas para el usuario {userLog.UserName}";
                    return dataUserDto;
                }
            }

        }

        private JwtSecurityToken CreateJwtToken(User user)
        {
            if (user != null)
            {


                var roles = user.Roles;
                var roleClaims = new List<Claim>();

                foreach (var role in roles)
                {
                    roleClaims.Add(new Claim("roles", role.RolName));
                }
                var claims = new[]{
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("uid", user.Id.ToString())
            }
                .Union(roleClaims);
                if (string.IsNullOrEmpty(_jwt.Key) || string.IsNullOrEmpty(_jwt.Issuer) || string.IsNullOrEmpty(_jwt.Audience))
                {
                    throw new ArgumentNullException($"La configuración del JWT es nula o vacía. 1.{_jwt.Key} 2.{_jwt.Issuer} 3.{_jwt.Audience}");
                }
                else
                {
                    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
                    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
                    var jwtSecurityToken = new JwtSecurityToken(
                        issuer: _jwt.Issuer,
                        audience: _jwt.Audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(_jwt.DurationOnMinutes),
                        signingCredentials: signingCredentials);
                    return jwtSecurityToken;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo");
            }
        }
        private string GenerarRefreshToken()
        {

            var byteArray = new byte[64];
            var refreshToken = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
                refreshToken = Convert.ToBase64String(byteArray);
            }
            return refreshToken;
        }
        private async Task<AuthTokenResponseDto> SaveHistorialRefreshToken(int idUser, string token, string refreshToken)
        {
            var historialRefreshToken = new HistorialRefreshToken
            {
                IdUser = idUser,
                Token = token,
                RefreshToken = refreshToken,
                DateCreated = DateTime.UtcNow,
                DateExpired = DateTime.UtcNow.AddMinutes(2)
            };

            _unitOfWork.HistorialTokens.Add(historialRefreshToken);
            await _unitOfWork.SaveAsync();

            return new AuthTokenResponseDto { Token = token, RefreshToken = refreshToken, Result = true, Msg = "Ok" };
        }



        public async Task<AuthTokenResponseDto> GetRefreshToken(RefreshTokenDto refreshRequest, int idUsuario)
        {
            var historialTokens = await _unitOfWork.HistorialTokens.GetAllAsync(); // Esperar la tarea y obtener la secuencia
            var refreshTokenFound = historialTokens.FirstOrDefault(x =>
                x.Token == refreshRequest.TokenExpired &&
                x.RefreshToken == refreshRequest.RefreshToken &&
                x.IdUser == idUsuario);

            if (refreshTokenFound == null)
            {
                return new AuthTokenResponseDto { Result = false, Msg = "No existe refreshToken" };
            }
            var user = await _unitOfWork.Users.GetSomeUserLogic(idUsuario.ToString());

            if (user == null)
            {
                return new AuthTokenResponseDto { Result = false, Msg = "No existe el usuario" };
            }
            var refreshTokenCreated = GenerarRefreshToken();
            var tokenCreated = CreateJwtToken(user);
            string token = new JwtSecurityTokenHandler().WriteToken(tokenCreated);
            var result = await SaveHistorialRefreshToken(idUsuario, token, refreshTokenCreated);
            return result;
        }

        public async Task<bool> ValidateToken(string token, IConfiguration configuration)
        {

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);

                //parámetros de validación del token
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                };

                SecurityToken securityToken;
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                //token es válido
                return true;
            }
            catch (SecurityTokenExpiredException ex)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenUser = tokenHandler.ReadJwtToken(token);
                var idUsuarioClaim = tokenUser.Claims.FirstOrDefault(claim => claim.Type == "idUsuario");

                if (idUsuarioClaim != null)
                {
                    var idUsuario = idUsuarioClaim.Value;
                    var user = await _unitOfWork.Users.GetSomeUserLogic(idUsuario);

                    if (user != null)
                    {
                        var tokenCreated = CreateJwtToken(user);
                        return true;
                    }
                }

                return false;
            }

            catch (SecurityTokenInvalidSignatureException ex)
            {
                //token corrupto
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}