namespace ElectronicDepartment.DAL.Entities;

public class StudentSubject
{
    public int StudentId { get; set; }
    public Student Student { get; set; }

    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    // Оцінка чи інші дані:
    public int? Grade { get; set; }
}
