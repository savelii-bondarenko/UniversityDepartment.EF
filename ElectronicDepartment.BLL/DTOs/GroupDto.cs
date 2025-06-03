namespace ElectronicDepartment.BLL.DTOs;

public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId {  get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public List<StudentDto> Students { get; set; } = new();
}

public class CreateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}

public class UpdateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}