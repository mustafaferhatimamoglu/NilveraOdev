using MediatR;
using NilveraOdev.Domain.Customers;

namespace NilveraOdev.Features.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    CustomerContactInfo ContactInfo) : IRequest<int>;
