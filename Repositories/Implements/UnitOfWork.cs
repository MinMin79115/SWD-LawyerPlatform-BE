using System;
using System.Collections;
using System.Threading.Tasks;
using BusinessObjects.DBContext;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repositories.GenericRepository;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _repositories;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;
        private IUserRepository _userRepository;
        private ILawyerRepository _lawyerRepository;
        private IAppointmentRepository _appointmentRepository;
        private IPaymentRepository _paymentRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public ApplicationDbContext Context => _context;

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        
        public ILawyerRepository LawyerRepository => _lawyerRepository ??= new LawyerRepository(_context);
        
        public IAppointmentRepository AppointmentRepository => _appointmentRepository ??= new AppointmentRepository(_context);
        
        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);
        
        public IGenericRepository<Lawtype> LawtypeRepository => Repository<Lawtype>();
        
        // Đã xóa public IServiceRepository ServiceRepository => _serviceRepository ??= new ServiceRepository(_context);

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(T)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _transaction?.Dispose();
                    _context?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
} 