using NilveraOdev.Domain.Customers;

namespace NilveraOdev.Contracts.Customers;

public static class CustomerContractMapper
{
    public static CustomerContactInfo ToDomain(this CustomerContactInfoDto dto)
    {
        return new CustomerContactInfo
        {
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address?.ToDomain()
        };
    }

    public static CustomerAddress ToDomain(this CustomerAddressDto dto)
    {
        return new CustomerAddress
        {
            Street = dto.Street,
            City = dto.City,
            Country = dto.Country,
            PostalCode = dto.PostalCode
        };
    }
}
