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
        public IActionResult GetCustomerById(Guid customerId)
        {
            var customer = _customerRepository.GetCustomerById(customerId);
            if (customer is null)
                return NotFound();
            else
            {
                return Ok(CustomerResponse.FromModel(customer));

            }
        }

        [HttpGet]
        public IActionResult GetCusomters()
        {
            var customers = _customerRepository.GetAllCustomers();
            if (customers is null)
                return NotFound("there are no customers right now ");
            else
            {
                return Ok(CustomerResponse.FromModels(customers));
            }
        }

        [HttpDelete]
        [Route("{customerId:guid}")]
        public IActionResult DelteCustomer(Guid customerId)
        {
            var isDeleted = _customerRepository.DeleteCustomerById(customerId);
            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound($"Customer with the id: {customerId} was not found");
            }
        }

        [HttpPost]
        public IActionResult AddCustomer(CreateCustomerRequest request)
        {
            try
            {
                Guid? id = _customerRepository.CreateCustomer(
                    request.Name,
                    request.Email);

                if (id is null)
                    return BadRequest();

                var customer = _customerRepository.GetCustomerById(id.Value);

                var response = CustomerResponse.FromModel(customer!);

                return CreatedAtAction(
                    nameof(GetCustomerById),
                    new { customerId = customer?.Id },
                    response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpPut]
        [Route("{customerId:guid}")]
        public IActionResult UpdateCustomer(Guid customerId, UpdateCustomerRequest request)
        {
            try
            {
                var updateCustomer = _customerRepository.UpdateCustomer(customerId, request.Name, request.Email);
                if (updateCustomer)
                    return NoContent();
                else
                {
                    return NotFound();
                }
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
