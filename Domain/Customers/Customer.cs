namespace NilveraOdev.Domain.Customers;

public class Customer
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required CustomerContactInfo ContactInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CustomerContactInfo
{
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public CustomerAddress? Address { get; set; }
}

public class CustomerAddress
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}
