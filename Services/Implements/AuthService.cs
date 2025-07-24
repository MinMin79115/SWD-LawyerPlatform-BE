using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using BusinessObjects.Common;
using BusinessObjects.DTO.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.Interfaces;
using BC = BCrypt.Net.BCrypt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;

        public AuthService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
            _userService = userService;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var authResult = new AuthResult();

            // Kiểm tra email và mật khẩu bằng UserService
            var isValidPassword = await _userService.CheckPasswordAsync(request.Email, request.Password);
            if (!isValidPassword)
            {
                authResult.Success = false;
                authResult.Errors.Add("Email hoặc mật khẩu không chính xác");
                return authResult;
            }

            // Lấy thông tin user
            var user = await _userService.GetUserByEmailAsync(request.Email);

            // Lấy role của user từ UserService
            string role = await _userService.GetUserRoleAsync(user.Userid);
            
            // Tạo JWT token và refresh token
            var jwtId = Guid.NewGuid().ToString();
            authResult.Token = GenerateJwtToken(user.Email, role, user.Userid, jwtId);
            
            // Tạo refresh token và lưu vào DB
            var refreshToken = GenerateRefreshtoken(jwtId, user.Userid);
            await _unitOfWork.Repository<Refreshtoken>().AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            
            authResult.RefreshToken = refreshToken.Token;
            authResult.Success = true;
            return authResult;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            var authResult = new AuthResult();

            // Đăng ký user mới bằng UserService
            var newUser = await _userService.RegisterUserAsync(request);
            
            if (newUser == null)
            {
                authResult.Success = false;
                authResult.Errors.Add("Email đã được sử dụng");
                return authResult;
            }

            try
            {
                // Không tạo token khi đăng ký nữa
                authResult.Success = true;
                return authResult;
            }
            catch (Exception ex)
            {
                authResult.Success = false;
                authResult.Errors.Add($"Đăng ký thất bại: {ex.Message}");
                return authResult;
            }
        }

        public async Task<AuthResult> RefreshTokenAsync(TokenRequest tokenRequest)
        {
            var authResult = new AuthResult();
            
            // Xác thực JWT token hiện tại (chỉ kiểm tra cấu trúc, không cần còn hiệu lực)
            var principal = GetPrincipalFromExpiredToken(tokenRequest.Token);
            if (principal == null)
            {
                authResult.Success = false;
                authResult.Errors.Add("JWT token không hợp lệ");
                return authResult;
            }

            // Lấy thông tin từ JWT token
            var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var email = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            var userIdClaim = principal.Claims.SingleOrDefault(x => x.Type == "UserId")?.Value;
            
            if (jti == null || email == null || userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                authResult.Success = false;
                authResult.Errors.Add("JWT token không chứa đủ thông tin cần thiết");
                return authResult;
            }

            // Tìm refresh token trong DB
            var refreshToken = await _unitOfWork.Repository<Refreshtoken>().GetFirstOrDefaultAsync(
                x => x.Token == tokenRequest.RefreshToken && x.Jwtid == jti && x.Userid == userId);

            if (refreshToken == null)
            {
                authResult.Success = false;
                authResult.Errors.Add("Refresh token không tồn tại");
                return authResult;
            }

            // Kiểm tra refresh token còn hiệu lực không
            if (refreshToken.Expirydate < DateTime.Now)
            {
                authResult.Success = false;
                authResult.Errors.Add("Refresh token đã hết hạn");
                return authResult;
            }

            // Kiểm tra refresh token đã bị sử dụng hay thu hồi chưa
            if (refreshToken.Isused == true || refreshToken.Isrevoked == true)
            {
                authResult.Success = false;
                authResult.Errors.Add("Refresh token đã được sử dụng hoặc bị thu hồi");
                return authResult;
            }

            // Đánh dấu token đã sử dụng
            refreshToken.Isused = true;
            _unitOfWork.Repository<Refreshtoken>().Update(refreshToken);
            
            // Lấy user để lấy role
            //var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            //if (user == null)
            //{
            //    authResult.Success = false;
            //    authResult.Errors.Add("Không tìm thấy người dùng");
            //    return authResult;
            //}
            
            // Lấy role từ UserService
            string role = await _userService.GetUserRoleAsync(userId);

            // Tạo JWT token mới và refresh token mới
            var newJwtId = Guid.NewGuid().ToString();
            authResult.Token = GenerateJwtToken(email, role, userId, newJwtId);
            
            // Tạo refresh token mới và lưu vào DB
            var newRefreshToken = GenerateRefreshtoken(newJwtId, userId);
            await _unitOfWork.Repository<Refreshtoken>().AddAsync(newRefreshToken);
            
            // Lưu thay đổi vào DB
            await _unitOfWork.SaveChangesAsync();
            
            authResult.RefreshToken = newRefreshToken.Token;
            authResult.Success = true;
            return authResult;
        }

        public string GenerateJwtToken(string email, string role, int userId, string jwtId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private Refreshtoken GenerateRefreshtoken(string jwtId, int userId)
        {
            return new Refreshtoken
            {
                Token = Guid.NewGuid().ToString(),
                Jwtid = jwtId,
                Userid = userId,
                Addeddate = DateTime.Now,
                Expirydate = DateTime.Now.AddDays(_jwtSettings.RefreshTokenDurationInDays),
                Isused = false,
                Isrevoked = false
            };
        }
        
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // Không kiểm tra thời hạn, vì token có thể đã hết hạn
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                // Kiểm tra token
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                
                // Kiểm tra loại token
                if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
} 