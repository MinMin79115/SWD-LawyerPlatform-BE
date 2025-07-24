using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Models;
using BusinessObjects.DTO.Auth;
using BusinessObjects.DTO.User;
using Repositories.Interfaces;
using Services.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> RegisterUserAsync(RegisterRequest request)
        {
            // Kiểm tra email đã tồn tại chưa bằng UserRepository
            var isEmailExist = await _unitOfWork.UserRepository.IsEmailExistAsync(request.Email);
            
            if (isEmailExist)
            {
                return null; // Email đã tồn tại
            }

            // Tạo user mới (luôn là Customer)
            var newUser = new User
            {
                Email = request.Email,
                Password = BC.HashPassword(request.Password), // Hash password
                Name = request.Name,
                Phone = request.Phone,
                Role = UserRole.Customer,
                Createdat = DateTime.Now,
                Updatedat = DateTime.Now
            };

            await _unitOfWork.Repository<User>().AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return newUser;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.UserRepository.GetByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                user.Updatedat = DateTime.Now;
                _unitOfWork.Repository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(email);
            
            if (user == null)
            {
                return false;
            }

            return BC.Verify(password, user.Password);
        }

        // Implement new CRUD methods
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            
            return users.Select(u => new UserDto
            {
                UserId = u.Userid,
                Email = u.Email,
                Name = u.Name,
                Phone = u.Phone,
                Avatar = u.Avatar,
                Role = u.Role.ToString(),
                CreatedAt = u.Createdat,
                UpdatedAt = u.Updatedat
            });
        }

        public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
        {
            // Check if email already exists
            var isEmailExist = await _unitOfWork.UserRepository.IsEmailExistAsync(request.Email);
            
            if (isEmailExist)
            {
                return null; // Email already exists
            }

            // Create new user
            var newUser = new User
            {
                Email = request.Email,
                Password = BC.HashPassword(request.Password), // Hash password
                Name = request.Name,
                Phone = request.Phone,
                Avatar = request.Avatar,
                Role = request.Role,
                Createdat = DateTime.Now,
                Updatedat = DateTime.Now
            };

            await _unitOfWork.Repository<User>().AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            // Map to DTO
            return new UserDto
            {
                UserId = newUser.Userid,
                Email = newUser.Email,
                Name = newUser.Name,
                Phone = newUser.Phone,
                Avatar = newUser.Avatar,
                Role = newUser.Role.ToString(),
                CreatedAt = newUser.Createdat,
                UpdatedAt = newUser.Updatedat
            };
        }

        public async Task<UserDto> GetUserDtoByIdAsync(int userId)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            
            if (user == null)
                return null;
                
            return new UserDto
            {
                UserId = user.Userid,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                Avatar = user.Avatar,
                Role = user.Role.ToString(),
                CreatedAt = user.Createdat,
                UpdatedAt = user.Updatedat
            };
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
                
                if (user == null)
                    return false;
                    
                // Update user properties
                user.Name = request.Name ?? user.Name;
                user.Phone = request.Phone ?? user.Phone;
                user.Avatar = request.Avatar ?? user.Avatar;
                
                // Update password if provided
                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.Password = BC.HashPassword(request.Password);
                }
                
                user.Updatedat = DateTime.Now;
                
                _unitOfWork.Repository<User>().Update(user);
                await _unitOfWork.SaveChangesAsync();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                await _unitOfWork.Repository<User>().DeleteAsync(userId);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetUserRoleAsync(int userId)
        {
            return await _unitOfWork.UserRepository.GetUserRoleAsync(userId);
        }
    }
} 