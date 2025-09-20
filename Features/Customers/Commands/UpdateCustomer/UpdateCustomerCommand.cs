using MediatR;
using NilveraOdev.Domain.Customers;

namespace NilveraOdev.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    int Id,
    string FirstName,
    string LastName,
    CustomerContactInfo ContactInfo) : IRequest<bool>;
