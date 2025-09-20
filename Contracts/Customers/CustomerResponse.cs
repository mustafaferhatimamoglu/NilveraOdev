using NilveraOdev.Domain.Customers;

namespace NilveraOdev.Contracts.Customers;

public sealed record CustomerResponse(
    int Id,
    string FirstName,
    string LastName,
    CustomerContactInfoDto ContactInfo,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static CustomerResponse FromDomain(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            ToDto(customer.ContactInfo),
            customer.CreatedAt,
            customer.UpdatedAt
        );
    }

    private static CustomerContactInfoDto ToDto(CustomerContactInfo contact)
    {
        return new CustomerContactInfoDto(
            contact.Email,
            contact.PhoneNumber,
            contact.Address is null
                ? null
                : new CustomerAddressDto(
                    contact.Address.Street,
                    contact.Address.City,
                    contact.Address.Country,
                    contact.Address.PostalCode)
        );
    }
}
