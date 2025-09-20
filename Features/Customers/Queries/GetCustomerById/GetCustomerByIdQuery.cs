using MediatR;
using NilveraOdev.Domain.Customers;

namespace NilveraOdev.Features.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(int Id) : IRequest<Customer?>;
