using System;
using System.Threading.Tasks;
using BusinessObjects.Models;
using BusinessObjects.DTO.Auth;
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
    }
} 