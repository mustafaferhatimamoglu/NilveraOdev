using System.Data;
using Dapper;
using MediatR;
using NilveraOdev.Infrastructure.Database;

namespace NilveraOdev.Features.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DeleteCustomerCommandHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Id", request.Id, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(
            "usp_Customers_Delete",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return affectedRows > 0;
    }
}
