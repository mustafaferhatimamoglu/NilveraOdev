namespace NilveraOdev.Infrastructure.Database.Records;

public sealed class CustomerRecord
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string ContactInfoJson { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
