using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessObjects.Common;
using BusinessObjects.DTO.User;
using Services.Interfaces;

namespace Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        /// <returns>Danh sách người dùng</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new ApiResponse
            {
                Code = 200,
                Status = true,
                Message = "Lấy danh sách người dùng thành công",
                Data = users
            });
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông tin người dùng</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserDtoByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Code = 404,
                    Status = false,
                    Message = "Không tìm thấy người dùng",
                    Data = null
                });
            }

            return Ok(new ApiResponse
            {
                Code = 200,
                Status = true,
                Message = "Lấy thông tin người dùng thành công",
                Data = user
            });
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="request">Thông tin người dùng</param>
        /// <returns>Người dùng đã tạo</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Dữ liệu không hợp lệ",
                    Data = ModelState
                });
            }

            var user = await _userService.CreateUserAsync(request);
            
            if (user == null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Email đã tồn tại",
                    Data = null
                });
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new ApiResponse
            {
                Code = 201,
                Status = true,
                Message = "Tạo người dùng thành công",
                Data = user
            });
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Code = 400,
                    Status = false,
                    Message = "Dữ liệu không hợp lệ",
                    Data = ModelState
                });
            }

            var result = await _userService.UpdateUserAsync(id, request);
            
            if (!result)
            {
                return NotFound(new ApiResponse
                {
                    Code = 404,
                    Status = false,
                    Message = "Không tìm thấy người dùng hoặc cập nhật thất bại",
                    Data = null
                });
            }

            return Ok(new ApiResponse
            {
                Code = 200,
                Status = true,
                Message = "Cập nhật người dùng thành công",
                Data = null
            });
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            
            if (!result)
            {
                return NotFound(new ApiResponse
                {
                    Code = 404,
                    Status = false,
                    Message = "Không tìm thấy người dùng hoặc xóa thất bại",
                    Data = null
                });
            }

            return Ok(new ApiResponse
            {
                Code = 200,
                Status = true,
                Message = "Xóa người dùng thành công",
                Data = null
            });
        }
    }
}
