using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BusinessObjects.Common;
using BusinessObjects.DTO.Auth;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập vào hệ thống
        /// </summary>
        /// <param name="request">Thông tin đăng nhập</param>
        /// <returns>Token JWT nếu đăng nhập thành công</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            
            if (result.Success)
            {
                return Ok(new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Đăng nhập thành công",
                    Data = new { token = result.Token, refreshToken = result.RefreshToken }
                });
            }
            
            return Unauthorized(new ApiResponse
            {
                Code = 401,
                Status = false,
                Message = "Đăng nhập thất bại",
                Data = result.Errors
            });
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới
        /// </summary>
        /// <param name="request">Thông tin đăng ký</param>
        /// <returns>Thông báo đăng ký thành công</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            
            if (result.Success)
            {
                return Ok(new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Đăng ký tài khoản khách hàng thành công"
                });
            }
            
            return BadRequest(new ApiResponse
            {
                Code = 400,
                Status = false,
                Message = "Đăng ký thất bại",
                Data = result.Errors
            });
        }
        
        /// <summary>
        /// Làm mới token JWT
        /// </summary>
        /// <param name="request">Token JWT cũ và refresh token</param>
        /// <returns>Token JWT mới và refresh token mới</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            
            if (result.Success)
            {
                return Ok(new ApiResponse
                {
                    Code = 200,
                    Status = true,
                    Message = "Làm mới token thành công",
                });
            }
            
            return BadRequest(new ApiResponse
            {
                Code = 400,
                Status = false,
                Message = "Làm mới token thất bại",
                Data = result.Errors
            });
        }
    }
} 