using System.Text.RegularExpressions;

namespace ElectronicDepartment.DAL.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual List<Group> Groups { get; set; } = new();
    public virtual List<Subject> Subjects { get; set; } = new();
}

