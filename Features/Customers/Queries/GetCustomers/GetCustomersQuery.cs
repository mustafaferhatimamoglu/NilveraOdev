using MediatR;
using NilveraOdev.Domain.Customers;

namespace NilveraOdev.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery : IRequest<IReadOnlyCollection<Customer>>;
