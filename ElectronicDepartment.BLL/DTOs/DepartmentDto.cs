namespace ElectronicDepartment.BLL.DTOs;

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<GroupDto> Groups { get; set; } = new();
    public List<SubjectDto> Subjects { get; set; } = new();
}

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
}
