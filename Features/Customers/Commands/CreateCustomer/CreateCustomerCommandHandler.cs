using System.Data;
using Dapper;
using MediatR;
using NilveraOdev.Infrastructure.Database;
using NilveraOdev.Infrastructure.Database.Mappers;

namespace NilveraOdev.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CreateCustomerCommandHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@FirstName", request.FirstName, DbType.String, ParameterDirection.Input, 100);
        parameters.Add("@LastName", request.LastName, DbType.String, ParameterDirection.Input, 100);
        parameters.Add("@ContactInfoJson", CustomerMapper.SerializeContactInfo(request.ContactInfo), DbType.String);

        var createdId = await connection.ExecuteScalarAsync<int>(
            "usp_Customers_Create",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return createdId;
    }
}
