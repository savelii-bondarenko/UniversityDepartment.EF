namespace ElectronicDepartment.Common.Exceptions;

public class UnauthorizedAccessException : BusinessLogicException
{
    public UnauthorizedAccessException(string message) : base(message) { }
}
