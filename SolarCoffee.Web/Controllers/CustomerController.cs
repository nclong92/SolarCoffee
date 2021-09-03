using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Customer;
using SolarCoffee.Web.Serialization;
using SolarCoffee.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger,
            ICustomerService customerService)
        {
            this._logger = logger;
            this._customerService = customerService;
        }

        [HttpPost("/api/customer")]
        public ActionResult CreateCustomer([FromBody] CustomerModel customer)
        {
            _logger.LogInformation("Creating a new customer");

            customer.CreatedOn = DateTime.UtcNow;
            customer.UpdatedOn = DateTime.UtcNow;

            var customerData = CustomerMapper.SerializeCustomer(customer);
            var newCustomer = _customerService.CreateCustomer(customerData);

            return Ok(newCustomer);
        }

        [HttpGet("/api/customer")]
        public ActionResult GetCustomers()
        {
            _logger.LogInformation("Getting customers");

            var customers = _customerService.GetAllCustomer();

            var customerModels = customers.Select(item => new CustomerModel()
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                PrimaryAddress = CustomerMapper.MapCustomerAddress(item.PrimaryAddress),
                CreatedOn = item.CreatedOn,
                UpdatedOn = item.UpdatedOn
            })
                .OrderByDescending(m => m.CreatedOn)
                .ToList();

            return Ok(customerModels);
        }

        [HttpDelete("/api/customer/{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            _logger.LogInformation("Deleting a customer");

            var response = _customerService.DeleteCustomer(id);
            return Ok(response);
        }
    }
}
