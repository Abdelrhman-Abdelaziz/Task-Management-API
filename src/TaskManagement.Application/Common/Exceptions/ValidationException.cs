namespace TaskManagement.Application.Common.Exceptions;

public sealed class ValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IReadOnlyList<string> errors)
        : base("One or more validation failures occurred.")
    {
        Errors = errors;
    }
}
