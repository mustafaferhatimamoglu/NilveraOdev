namespace NilveraOdev.Contracts.Customers;

public sealed record CustomerContactInfoDto(
    string Email,
    string? PhoneNumber,
    CustomerAddressDto? Address
);
