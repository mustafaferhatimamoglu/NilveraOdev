namespace NilveraOdev.Contracts.Customers;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    CustomerContactInfoDto ContactInfo
);
