using Dapper;
using System.Linq;
using MediatR;
using NilveraOdev.Domain.Customers;
using NilveraOdev.Infrastructure.Database;
using NilveraOdev.Infrastructure.Database.Mappers;
using NilveraOdev.Infrastructure.Database.Records;

namespace NilveraOdev.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, IReadOnlyCollection<Customer>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public GetCustomersQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<Customer>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var records = await connection.QueryAsync<CustomerRecord>(
            "usp_Customers_GetAll",
            commandType: System.Data.CommandType.StoredProcedure
        );

        return records
            .Select(CustomerMapper.ToDomain)
            .ToArray();
    }
}
