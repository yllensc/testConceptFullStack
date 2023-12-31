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
                var existRol = _unitOfWork.Roles
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
                    string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                    dataUserDto.Token = token;
                    dataUserDto.Email = user.Email;
                    dataUserDto.UserName = user.UserName;
                    dataUserDto.Roles = user.Roles
                                        .Select(x => x.RolName)
                                        .ToList();
                     if (user.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                dataUserDto.RefreshToken = activeRefreshToken.Token;
                dataUserDto.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = CreateRefreshToken();
                dataUserDto.RefreshToken = refreshToken.Token;
                dataUserDto.RefreshTokenExpiration = refreshToken.Expires;
                user.RefreshTokens.Add(refreshToken);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }

            return dataUserDto;
        }
        dataUserDto.AuthStatus = false;
        dataUserDto.Message = $"Credenciales incorrectas para el usuario {user.UserName}.";
        return dataUserDto;
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
        private RefreshTokenOp2 CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return new RefreshTokenOp2
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow
            };
        }
    }
        private async Task<AuthTokenResponseDto> SaveHistorialRefreshToken(int idUser, string token, string refreshToken)
        {
            var existingHistorialRefreshToken = await _unitOfWork.HistorialTokens.GetAllAsync();
            var registerToken = existingHistorialRefreshToken.FirstOrDefault(h => h.IdUser == idUser);

            if (registerToken != null)
            {
                // Actualizar el registro existente
                registerToken.Token = token;
                registerToken.RefreshToken = refreshToken;
                registerToken.DateCreated = DateTime.UtcNow;
                registerToken.DateExpired = DateTime.UtcNow.AddMinutes(2);

                await _unitOfWork.SaveAsync();
            }
            else
            {
                // Agregar un nuevo registro
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
            }

            return new AuthTokenResponseDto { Token = token, RefreshToken = refreshToken, Result = true, Msg = "Ok" };
        }
         public async Task<DataUserDto> RefreshTokenAsync(string refreshToken)
    {
        var dataUserDto = new DataUserDto();

        var usuario = await _unitOfWork.Users
                        .GetByRefreshTokenAsync(refreshToken);

        if (usuario == null)
        {
            dataUserDto.AuthStatus = false;
            dataUserDto.Message = $"Token is not assigned to any user.";
            return dataUserDto;
        }

        var refreshTokenBd = usuario.RefreshTokens.Single(x => x.Token == refreshToken);

        if (!refreshTokenBd.IsActive)
        {
            dataUserDto.AuthStatus = false;
            dataUserDto.Message = $"Token is not active.";
            return dataUserDto;
        }
        //Revoque the current refresh token and
        refreshTokenBd.Revoked = DateTime.UtcNow;
        //generate a new refresh token and save it in the database
        var newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens.Add(newRefreshToken);
        _unitOfWork.Users.Update(usuario);
        await _unitOfWork.SaveAsync();
        //Generate a new Json Web Token
        dataUserDto.AuthStatus = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
        dataUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        dataUserDto.Email = usuario.Email;
        dataUserDto.UserName = usuario.UserName;
        dataUserDto.Roles = usuario.Roles
                                        .Select(u => u.RolName)
                                        .ToList();
        dataUserDto.RefreshToken = newRefreshToken.Token;
        dataUserDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return dataUserDto;
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

                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                //token es válido
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenUser = tokenHandler.ReadJwtToken(token);
                var idUsuarioClaim = tokenUser.Claims.FirstOrDefault(claim => claim.Type == "uid");

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

            catch (SecurityTokenInvalidSignatureException)
            {
                //token corrupto
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }


    }
}