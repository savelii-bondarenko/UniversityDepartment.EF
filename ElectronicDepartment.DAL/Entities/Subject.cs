namespace ElectronicDepartment.DAL.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    public int TeacherId { get; set; }
    public virtual Teacher Teacher { get; set; }

    public int GroupId { get; set; }
    public virtual Group Group { get; set; }

    public virtual List<StudentSubject> StudentSubjects { get; set; } = new();
}

