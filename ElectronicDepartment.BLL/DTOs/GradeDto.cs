namespace ElectronicDepartment.BLL.DTOs;

public class GradeDto
{
    public string Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int? Grade { get; set; }
}

public class CreateGradeDto
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int Grade { get; set; }
}