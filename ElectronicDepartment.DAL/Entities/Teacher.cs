using ElectronicDepartment.Common.Enums;

namespace ElectronicDepartment.DAL.Entities;

public class Teacher
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; }
    public virtual List<Subject> Subjects { get; set; } = new();
}