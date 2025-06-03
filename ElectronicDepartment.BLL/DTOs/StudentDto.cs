namespace ElectronicDepartment.BLL.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public List<GradeDto> Grades { get; set; } = new();
}

public class CreateStudentDto
{
   public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int GroupId { get; set; }
}

public class UpdateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int GroupId { get; set; }
}