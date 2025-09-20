using System.Text.Json;
using NilveraOdev.Domain.Customers;
using NilveraOdev.Infrastructure.Database.Records;

namespace NilveraOdev.Infrastructure.Database.Mappers;

public static class CustomerMapper
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public static Customer ToDomain(CustomerRecord record)
    {
        return new Customer
        {
            Id = record.Id,
            FirstName = record.FirstName,
            LastName = record.LastName,
            ContactInfo = DeserializeContactInfo(record.ContactInfoJson),
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }

    public static string SerializeContactInfo(CustomerContactInfo contactInfo)
    {
        return JsonSerializer.Serialize(contactInfo, SerializerOptions);
    }

    public static CustomerContactInfo DeserializeContactInfo(string? contactInfoJson)
    {
        if (string.IsNullOrWhiteSpace(contactInfoJson))
        {
            return new CustomerContactInfo
            {
                Email = string.Empty,
                PhoneNumber = null,
                Address = null
            };
        }

        var model = JsonSerializer.Deserialize<CustomerContactInfo>(contactInfoJson, SerializerOptions);
        if (model is null)
        {
            return new CustomerContactInfo
            {
                Email = string.Empty,
                PhoneNumber = null,
                Address = null
            };
        }

        return model;
    }
}
