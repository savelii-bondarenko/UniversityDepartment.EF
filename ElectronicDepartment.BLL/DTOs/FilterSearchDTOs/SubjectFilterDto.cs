namespace ElectronicDepartment.BLL.DTOs;

public class SubjectFilterDto
{
    public string? SearchTerm { get; set; }
    public int? DepartmentId { get; set; }
    public int? TeacherId { get; set; }
    public int? GroupId { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;
}
