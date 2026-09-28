using Asal.OrderManagementSystem.Api.Interfaces;
using Asal.OrderManagementSystem.Api.Requests.CustomerRequests;
using Asal.OrderManagementSystem.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Asal.OrderManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        [HttpGet]
        [Route("{customerId:guid}")]
        public async Task<IActionResult> GetCustomerById(Guid customerId,CancellationToken ct)
        {
            var customer =  await _customerRepository.GetCustomerByIdAsync(customerId, ct);
            if (customer is null)
                return NotFound(new ProblemDetails()
                {
                    Title="not found",
                    Detail=$"customer with the id {customerId} was not found"
                });
            else
            {
                return Ok(CustomerResponse.FromModel(customer));

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCusomters(CancellationToken ct)
        {
            var customers = await _customerRepository.GetAllCustomersAsync(ct);
            if (customers.Count==0)
                return NotFound(new ProblemDetails
                {
                    Title="no customers",
                    Detail="there is no customer right now "
                });
            else
            {
                return Ok(CustomerResponse.FromModels(customers));
            }
        }

        [HttpDelete]
        [Route("{customerId:guid}")]
        public async Task<IActionResult> DelteCustomer(Guid customerId,CancellationToken ct)
        {
            
            var isDeleted = await _customerRepository.DeleteCustomerByIdAsync(customerId,ct);
            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound(new ProblemDetails()
                {
                    Title= "customer  was not found",
                    Detail= "Customer with the id: {customerId} was not found"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CreateCustomerRequest request, CancellationToken ct)
        {
            try
            {
                Guid? id= await _customerRepository.CreateCustomerAsync(request.Name,request.Email,ct);
                if (id is null)
                    return Conflict(new ProblemDetails()
                    {
                        Title="duplicated email",
                        Detail= "two customers can't have the same email"
                    });

                var customer = await _customerRepository.GetCustomerByIdAsync(id.Value,ct);

                var response = CustomerResponse.FromModel(customer!);

                return CreatedAtAction(
                    nameof(GetCustomerById),
                    new { customerId = customer?.Id },
                    response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ProblemDetails()
                {
                    Title = ex.Message,
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails()
                {
                    Title = ex.Message,
                });

            }

        }

        [HttpGet]
        [Route("with-orders")]
        public async Task<IActionResult>GetCustomersWithOrders(CancellationToken ct)
        {
            var customers=await _customerRepository.GetCustomersWithOrdersAsync(ct);
            if (customers.Count == 0)
                return NotFound(new ProblemDetails()
                {
                    Title = "no customers",
                    Detail = "there is no customesr with orders right now "
                });
            else
            {
                return Ok(customers);
            }
        }
    }
}
