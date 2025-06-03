namespace ElectronicDepartment.Common.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, int id)
        : base($"{entityName} з ID {id} не знайдено") { }

    public EntityNotFoundException(string message) : base(message) { }
}