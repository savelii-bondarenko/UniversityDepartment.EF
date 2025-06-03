namespace ElectronicDepartment.BLL.DTOs;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
}

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int TeacherId { get; set; }
    public int GroupId { get; set; }
}

public class UpdateSubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int TeacherId { get; set; }
    public int GroupId { get; set; }
}