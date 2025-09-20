namespace NilveraOdev.Contracts.Customers;

public sealed record CustomerAddressDto(
    string? Street,
    string? City,
    string? Country,
    string? PostalCode
);
