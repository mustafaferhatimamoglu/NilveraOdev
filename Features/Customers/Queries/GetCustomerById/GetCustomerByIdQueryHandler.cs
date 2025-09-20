using System.Data;
using Dapper;
using MediatR;
using NilveraOdev.Domain.Customers;
using NilveraOdev.Infrastructure.Database;
using NilveraOdev.Infrastructure.Database.Mappers;
using NilveraOdev.Infrastructure.Database.Records;

namespace NilveraOdev.Features.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Customer?>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public GetCustomerByIdQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Customer?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Id", request.Id, DbType.Int32);

        var record = await connection.QuerySingleOrDefaultAsync<CustomerRecord>(
            "usp_Customers_GetById",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return record is null ? null : CustomerMapper.ToDomain(record);
    }
}
