using MediatR;

namespace NilveraOdev.Features.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(int Id) : IRequest<bool>;
