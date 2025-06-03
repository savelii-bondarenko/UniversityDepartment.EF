namespace ElectronicDepartment.BLL.DTOs;

public class StudentFilterDto
{
    public string? SearchTerm { get; set; }
    public int? GroupId { get; set; }
    public int? DepartmentId { get; set; }
    public string? SortBy { get; set; } = "LastName";
    public bool SortDescending { get; set; } = false;
}
