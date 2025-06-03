using ElectronicDepartment.DAL.Entities;
using ElectronicDepartment.DAL.Repositories;
using ElectronicDepartment.DAL.Repositories.Interfaces;

namespace ElectronicDepartment.DAL.UnitOfWork.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDepartmentRepository Departments { get; }
    IGroupRepository Groups { get; }
    IStudentRepository Students { get; }
    ITeacherRepository Teachers { get; }
    IManagerRepository Managers { get; }
    IAdminRepository Admins { get; }
    ISubjectRepository Subjects { get; }
    IStudentSubjectRepository StudentSubjects { get; }

    Task<int> SaveAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}