using System;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Access to the DbContext
        BusinessObjects.DBContext.ApplicationDbContext Context { get; }
        
        // Phương thức để truy xuất repository
        IGenericRepository<T> Repository<T>() where T : class;
        
        // User Repository
        IUserRepository UserRepository { get; }
        
        // Appointment Repository
        IAppointmentRepository AppointmentRepository { get; }
        
        // Lawyer Repository
        ILawyerRepository LawyerRepository { get; }
        
        // Service Repository
        IServiceRepository ServiceRepository { get; }
        
        // Phương thức lưu các thay đổi vào database
        Task<int> SaveChangesAsync();
        
        // Bắt đầu transaction
        Task BeginTransactionAsync();
        
        // Commit transaction
        Task CommitTransactionAsync();
        
        // Rollback transaction
        Task RollbackTransactionAsync();
    }
} 