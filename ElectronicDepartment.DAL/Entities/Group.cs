namespace ElectronicDepartment.DAL.Entities;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public virtual List<Student> Students { get; set; } = new();
    public virtual List<Subject> Subjects { get; set; } = new();
}

