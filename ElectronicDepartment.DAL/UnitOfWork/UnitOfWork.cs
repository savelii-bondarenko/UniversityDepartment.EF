using ElectronicDepartment.DAL.Context;
using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories;
using ElectronicDepartment.DAL.Repositories.Interfaces;
using ElectronicDepartment.DAL.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ElectronicDepartment.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ElectronicDepartmentContext _context;
    private IDbContextTransaction? _transaction;

    public IDepartmentRepository Departments { get; }
    public IGroupRepository Groups { get; }
    public IStudentRepository Students { get; }
    public ITeacherRepository Teachers { get; }
    public IManagerRepository Managers { get; }
    public IAdminRepository Admins { get; }
    public ISubjectRepository Subjects { get; }
    public IStudentSubjectRepository StudentSubjects { get; }

    public UnitOfWork(ElectronicDepartmentContext context)
    {
        _context = context;
        Students = new StudentRepository(_context);
        Teachers = new TeacherRepository(_context);
        Departments = new DepartmentRepository(_context);
        Groups = new GroupRepository(_context);
        Subjects = new SubjectRepository(_context);
        StudentSubjects = new StudentSubjectRepository(_context);
        Managers = new ManagerRepository(_context);
        Admins = new AdminRepository(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
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

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}