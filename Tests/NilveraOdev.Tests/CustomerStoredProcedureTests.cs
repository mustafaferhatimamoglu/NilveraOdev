using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NilveraOdev.Domain.Customers;
using NilveraOdev.Infrastructure.Database.Mappers;
using NilveraOdev.Infrastructure.Database.Records;

namespace NilveraOdev.Tests;

[CollectionDefinition("SqlIntegrationTests", DisableParallelization = true)]
public sealed class SqlIntegrationCollection : ICollectionFixture<SqlServerFixture>
{
}

public sealed class SqlServerFixture : IAsyncLifetime
{
    private const string ConnectionStringKey = "SqlServer";

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        ConnectionString = LoadConnectionString();

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        var scriptPath = ResolveCustomersScriptPath();
        var script = await File.ReadAllTextAsync(scriptPath);
        var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        foreach (var batch in batches)
        {
            var trimmed = batch.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            await using var command = connection.CreateCommand();
            command.CommandText = trimmed;
            await command.ExecuteNonQueryAsync();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static string LoadConnectionString()
    {
        var root = ResolveProjectRoot();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(root)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString(ConnectionStringKey)
            ?? Environment.GetEnvironmentVariable("NILVERA_SQLSERVER__CONNECTIONSTRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringKey}' was not found. Provide it in appsettings.json or via NILVERA_SQLSERVER__CONNECTIONSTRING environment variable.");
        }

        return connectionString;
    }

    private static string ResolveCustomersScriptPath()
    {
        var root = ResolveProjectRoot();
        var candidate = Path.Combine(root, "Database", "Scripts", "Customers.sql");

        if (!File.Exists(candidate))
        {
            throw new FileNotFoundException("Customers.sql script not found relative to project root.", candidate);
        }

        return candidate;
    }

    private static string ResolveProjectRoot()
    {
        var current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            var appSettingsPath = Path.Combine(current, "appsettings.json");
            if (File.Exists(appSettingsPath))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new DirectoryNotFoundException("Could not locate project root containing appsettings.json.");
    }
}

[Collection("SqlIntegrationTests")]
public sealed class CustomerStoredProcedureTests
{
    private readonly SqlServerFixture _fixture;

    public CustomerStoredProcedureTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAndGetProcedures_ShouldRoundtripCustomer()
    {
        var contact = BuildContact("ada@example.com", "+90 555 444 3322", "Istanbul");

        await using var connection = new SqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        var createdId = await connection.ExecuteScalarAsync<int>(
            "usp_Customers_Create",
            new
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                ContactInfoJson = CustomerMapper.SerializeContactInfo(contact)
            },
            commandType: CommandType.StoredProcedure);

        Assert.True(createdId > 0);

        try
        {
            var byId = await connection.QuerySingleOrDefaultAsync<CustomerRecord>(
                "usp_Customers_GetById",
                new { Id = createdId },
                commandType: CommandType.StoredProcedure);

            Assert.NotNull(byId);
            Assert.Equal("Ada", byId!.FirstName);
            Assert.Equal("Lovelace", byId.LastName);

            var deserialized = CustomerMapper.DeserializeContactInfo(byId.ContactInfoJson);
            Assert.Equal(contact.Email, deserialized.Email);
            Assert.NotNull(deserialized.Address);
            Assert.Equal(contact.Address!.City, deserialized.Address!.City);

            var all = await connection.QueryAsync<CustomerRecord>(
                "usp_Customers_GetAll",
                commandType: CommandType.StoredProcedure);

            Assert.Contains(all, record => record.Id == createdId);
        }
        finally
        {
            await connection.ExecuteAsync(
                "usp_Customers_Delete",
                new { Id = createdId },
                commandType: CommandType.StoredProcedure);
        }
    }

    [Fact]
    public async Task UpdateProcedure_ShouldModifyExistingCustomer()
    {
        var customerId = await SeedCustomerAsync();
        var updatedContact = BuildContact("grace@example.com", "+90 555 111 2222", "Ankara");

        await using var connection = new SqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        try
        {
            var affectedRows = await connection.ExecuteScalarAsync<int>(
                "usp_Customers_Update",
                new
                {
                    Id = customerId,
                    FirstName = "Grace",
                    LastName = "Hopper",
                    ContactInfoJson = CustomerMapper.SerializeContactInfo(updatedContact)
                },
                commandType: CommandType.StoredProcedure);

            Assert.Equal(1, affectedRows);

            var updated = await connection.QuerySingleAsync<CustomerRecord>(
                "usp_Customers_GetById",
                new { Id = customerId },
                commandType: CommandType.StoredProcedure);

            Assert.Equal("Grace", updated.FirstName);
            Assert.Equal("Hopper", updated.LastName);

            var contact = CustomerMapper.DeserializeContactInfo(updated.ContactInfoJson);
            Assert.Equal("grace@example.com", contact.Email);
            Assert.NotNull(contact.Address);
            Assert.Equal("Ankara", contact.Address!.City);
        }
        finally
        {
            await connection.ExecuteAsync(
                "usp_Customers_Delete",
                new { Id = customerId },
                commandType: CommandType.StoredProcedure);
        }
    }

    [Fact]
    public async Task DeleteProcedure_ShouldRemoveCustomer()
    {
        var customerId = await SeedCustomerAsync();

        await using var connection = new SqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        var affectedRows = await connection.ExecuteScalarAsync<int>(
            "usp_Customers_Delete",
            new { Id = customerId },
            commandType: CommandType.StoredProcedure);

        Assert.Equal(1, affectedRows);

        var deleted = await connection.QuerySingleOrDefaultAsync<CustomerRecord>(
            "usp_Customers_GetById",
            new { Id = customerId },
            commandType: CommandType.StoredProcedure);

        Assert.Null(deleted);
    }

    private async Task<int> SeedCustomerAsync()
    {
        var contact = BuildContact("seed@example.com", "+90 555 000 1111", "Izmir");

        await using var connection = new SqlConnection(_fixture.ConnectionString);
        await connection.OpenAsync();

        return await connection.ExecuteScalarAsync<int>(
            "usp_Customers_Create",
            new
            {
                FirstName = "Seed",
                LastName = "Customer",
                ContactInfoJson = CustomerMapper.SerializeContactInfo(contact)
            },
            commandType: CommandType.StoredProcedure);
    }

    private static CustomerContactInfo BuildContact(string email, string? phoneNumber, string? city)
    {
        return new CustomerContactInfo
        {
            Email = email,
            PhoneNumber = phoneNumber,
            Address = new CustomerAddress
            {
                Street = "Test Street",
                City = city,
                Country = "Turkey",
                PostalCode = "34000"
            }
        };
    }
}
