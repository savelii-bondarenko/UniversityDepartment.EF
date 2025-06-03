namespace ElectronicDepartment.BLL.DTOs;

public class GroupFilterDto
{
    public string? SearchTerm { get; set; }
    public int? DepartmentId { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
