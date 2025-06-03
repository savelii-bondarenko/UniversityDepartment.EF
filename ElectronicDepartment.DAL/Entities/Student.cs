using ElectronicDepartment.Common.Enums;

namespace ElectronicDepartment.DAL.Entities;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; }
    public int GroupId { get; set; }
    public virtual Group Group { get; set; } = default!;
    public virtual List<StudentSubject> StudentSubjects { get; set; } = new();
}