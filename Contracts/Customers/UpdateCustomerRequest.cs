namespace NilveraOdev.Contracts.Customers;

public sealed record UpdateCustomerRequest(
    string FirstName,
    string LastName,
    CustomerContactInfoDto ContactInfo
);
