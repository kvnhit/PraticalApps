using Microsoft.AspNetCore.Mvc;
using Northwind.EntityModels;
using Northwind.WebApi.Repositories;

namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _repo;

        public CustomersController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(string? country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return await _repo.RetriveAllAsync();
            }
            else
            {
                return (await _repo.RetriveAllAsync()).Where(customer => customer.Country == country);
            }
        }
        [HttpGet("{id}", Name = nameof(GetCustomer))]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(string id)
        {
            Customer? c = await _repo.RetrieveAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            return Ok(c);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Customer c)
        {
            if (c is null)
            {
                return BadRequest();
            }
            Customer? addedCustomer = await _repo.CreateAsync(c);

            if (addedCustomer is null)
            {
                return BadRequest("Repository failed to create Customer.");
            }
            else
            {
                return CreatedAtRoute(
                    routeName: nameof(GetCustomer),
                    routeValues: new { id = addedCustomer.CustomerId.ToLower() },
                    value: addedCustomer);
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] Customer c)
        {
            id = id.ToUpper();
            c.CustomerId = c.CustomerId.ToUpper();

            if(c is null || c.CustomerId != id)
            {
                return BadRequest();
            }
            Customer? existing = await _repo.RetrieveAsync(id);
            if(existing is null)
            {
                return NotFound();
            }
            await _repo.UpdateAsync(c);
            return new NoContentResult();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            if(id == "bad")
            {
                ProblemDetails problemDetails = new()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "http://localhost:5151/customers/failed-to-delete",
                    Title = $"Customer ID {id} found but failed to delete.",
                    Detail = "More details like Company Name, Country and so on.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
            Customer? existing = await _repo.RetrieveAsync(id);
            if(existing is null)
            {
                return NotFound();
            }
            bool? deleted = await _repo.DeleteAsync(id);
            if(deleted.HasValue && deleted.Value)
            {
                return new NoContentResult();
            }
            else
            {
                return BadRequest($"Customer {id} was found but failed to delete.");
            }
        }
    }
}
