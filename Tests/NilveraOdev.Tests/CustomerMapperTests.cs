using NilveraOdev.Domain.Customers;
using NilveraOdev.Infrastructure.Database.Mappers;

namespace NilveraOdev.Tests;

public class CustomerMapperTests
{
    [Fact]
    public void SerializeContactInfo_ShouldSerializeToJson()
    {
        var contact = new CustomerContactInfo
        {
            Email = "ada@example.com",
            PhoneNumber = "+90 555 444 3322",
            Address = new CustomerAddress
            {
                Street = "Baker Street 221B",
                City = "Istanbul",
                Country = "Turkey",
                PostalCode = "34000"
            }
        };

        var json = CustomerMapper.SerializeContactInfo(contact);

        Assert.Contains("\"email\":\"ada@example.com\"", json);
        Assert.Contains("\"city\":\"Istanbul\"", json);
    }

    [Fact]
    public void DeserializeContactInfo_ShouldReturnEquivalentObject()
    {
        const string json = "{\"email\":\"ada@example.com\",\"phoneNumber\":\"+90 555 444 3322\",\"address\":{\"street\":\"Baker Street 221B\",\"city\":\"Istanbul\",\"country\":\"Turkey\",\"postalCode\":\"34000\"}}";

        var contact = CustomerMapper.DeserializeContactInfo(json);

        Assert.NotNull(contact);
        Assert.Equal("ada@example.com", contact.Email);
        Assert.Equal("+90 555 444 3322", contact.PhoneNumber);
        Assert.NotNull(contact.Address);
        Assert.Equal("Istanbul", contact.Address!.City);
    }

    [Fact]
    public void DeserializeContactInfo_WithNull_ReturnsDefault()
    {
        var contact = CustomerMapper.DeserializeContactInfo(null);

        Assert.NotNull(contact);
        Assert.Equal(string.Empty, contact.Email);
        Assert.Null(contact.PhoneNumber);
        Assert.Null(contact.Address);
    }
}
