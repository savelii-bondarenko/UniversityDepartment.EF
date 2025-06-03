namespace ElectronicDepartment.BLL.DTOs;

public class GradeFilterDto
{
    public int? StudentId { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int? MinGrade { get; set; }
    public int? MaxGrade { get; set; }
}
