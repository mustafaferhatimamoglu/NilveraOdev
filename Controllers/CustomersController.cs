using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NilveraOdev.Contracts.Customers;
using NilveraOdev.Features.Customers.Commands.CreateCustomer;
using NilveraOdev.Features.Customers.Commands.DeleteCustomer;
using NilveraOdev.Features.Customers.Commands.UpdateCustomer;
using NilveraOdev.Features.Customers.Queries.GetCustomerById;
using NilveraOdev.Features.Customers.Queries.GetCustomers;

namespace NilveraOdev.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var customers = await _mediator.Send(new GetCustomersQuery(), cancellationToken);
        var response = customers.Select(CustomerResponse.FromDomain);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(id), cancellationToken);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(CustomerResponse.FromDomain(customer));
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new CreateCustomerCommand(
            request.FirstName,
            request.LastName,
            request.ContactInfo.ToDomain()
        );

        var createdId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = createdId }, createdId);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerCommand(
            id,
            request.FirstName,
            request.LastName,
            request.ContactInfo.ToDomain()
        );

        var isUpdated = await _mediator.Send(command, cancellationToken);
        if (!isUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteCustomerCommand(id);
        var isDeleted = await _mediator.Send(command, cancellationToken);
        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
