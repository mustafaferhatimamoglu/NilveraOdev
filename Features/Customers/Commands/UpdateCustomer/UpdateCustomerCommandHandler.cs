using System.Data;
using Dapper;
using MediatR;
using NilveraOdev.Infrastructure.Database;
using NilveraOdev.Infrastructure.Database.Mappers;

namespace NilveraOdev.Features.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UpdateCustomerCommandHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Id", request.Id, DbType.Int32);
        parameters.Add("@FirstName", request.FirstName, DbType.String, ParameterDirection.Input, 100);
        parameters.Add("@LastName", request.LastName, DbType.String, ParameterDirection.Input, 100);
        parameters.Add("@ContactInfoJson", CustomerMapper.SerializeContactInfo(request.ContactInfo), DbType.String);

        var affectedRows = await connection.ExecuteAsync(
            "usp_Customers_Update",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return affectedRows > 0;
    }
}
